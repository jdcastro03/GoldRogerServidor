using GoldRogerServer.Business.Core;
using GoldRoger.Data;
using System;
using Microsoft.EntityFrameworkCore;
using GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Coach;
using GoldRogerServer.DTOs.Tournament;
using System.Formats.Asn1;

namespace GoldRogerServer.Business
{
    public class CoachBusiness : BaseBusiness
    {
        public CoachBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        //get coachlicensenumber by id
        public async Task<string?> GetCoachLicenseNumberById(int coachId)
        {
            // Busca el entrenador en la base de datos usando el CoachId
            var coach = await uow.CoachRepository.Get(c => c.CoachId == coachId).FirstOrDefaultAsync();

            // Si no se encuentra el entrenador, lanza una excepción o devuelve null
            if (coach == null)
                throw new ArgumentException("Entrenador no encontrado");

            // Devuelve el número de licencia del entrenador
            return coach.LicenseNumber;
        }


        //create team
        public async Task<Team> CreateTeam(CreateTeamRequestDTO requestDTO, int coachId)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(requestDTO.TeamName) || requestDTO.TeamName.Trim().Length <= 3)
                throw new ArgumentException("Nombre de equipo inválido");

            // Verificar si el entrenador existe
            var coachExists = await uow.CoachRepository.Get(c => c.CoachId == coachId).AnyAsync();
            if (!coachExists)
                throw new ArgumentException("Entrenador no encontrado");

            // Crear el equipo con los datos del DTO y el CoachId del usuario logueado
            var newTeam = new Team
            {
                CoachId = coachId,
                TeamName = requestDTO.TeamName,
                TournamentId = requestDTO.TournamentId > 0 ? requestDTO.TournamentId : null
            };

            // Insertar el equipo en la base de datos
            uow.TeamRepository.Insert(newTeam);
            await uow.SaveAsync();

            return newTeam;
        }

        public async Task<List<string>> GetTeamNamesByCoachId(int coachId)
        {
            // Verificar si el entrenador existe
            var coachExists = await uow.CoachRepository.Get(c => c.CoachId == coachId).AnyAsync();
            if (!coachExists)
                throw new ArgumentException("Entrenador no encontrado");

            // Obtener los nombres de los equipos asociados al CoachId
            var teamNames = await uow.TeamRepository
                .Get(t => t.CoachId == coachId)
                .Select(t => t.TeamName)
                .ToListAsync();

            return teamNames;
        }


        //metodo para actualizar el torunamentname del campo que contenga el mismo coachid(coachid)
        public async Task UpdateTeamNameByCoachId(int coachId, string newTeamName)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(newTeamName) || newTeamName.Length <= 3)
                throw new ArgumentException("El nombre del equipo es inválido.");

            // Verificar si el entrenador existe
            var coachExists = await uow.CoachRepository.Get(c => c.CoachId == coachId).AnyAsync();
            if (!coachExists)
                throw new ArgumentException("Entrenador no encontrado.");

            // Obtener los equipos asociados al CoachId
            var teams = await uow.TeamRepository.Get(t => t.CoachId == coachId).ToListAsync();

            if (teams == null || !teams.Any())
                throw new ArgumentException("No se encontraron equipos asociados al entrenador.");

            // Actualizar el nombre del equipo para los equipos encontrados
            foreach (var team in teams)
            {
                team.TeamName = newTeamName;
                uow.TeamRepository.Update(team);
            }

            // Guardar los cambios en la base de datos
            await uow.SaveAsync();
        }

        public async Task DeleteTeamsByCoachId(int coachId)
        {
            // Verificar si el entrenador existe
            var coachExists = await uow.CoachRepository.Get(c => c.CoachId == coachId).AnyAsync();
            if (!coachExists)
                throw new ArgumentException("Entrenador no encontrado.");

            // Obtener los equipos asociados al CoachId
            var teams = await uow.TeamRepository.Get(t => t.CoachId == coachId).ToListAsync();

            if (teams == null || !teams.Any())
                throw new ArgumentException("No se encontraron equipos asociados al entrenador.");

            // Eliminar los equipos encontrados
            foreach (var team in teams)
            {
                uow.TeamRepository.Delete(team);
            }

            // Guardar los cambios en la base de datos
            await uow.SaveAsync();
        }

        //metoo para botener todos los torneos

        public async Task<List<TournamentDTO>> GetAllTournaments()
        {
            // Realizar el join entre torneos y usuarios para obtener el username
            var tournaments = await uow.TournamentRepository.Get()
                .Include(t => t.Organizer) // Asumiendo que tienes una relación de navegación en el modelo
                .Select(t => new TournamentDTO
                {
                    TournamentId = t.TournamentId,
                    TournamentName = t.TournamentName,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    TournamentTypeId = t.TournamentTypeId,
                    OrganizerUsername = t.Organizer.User.Username// Relación de navegación
                })
                .ToListAsync();

            if (tournaments == null || !tournaments.Any())
            {
                throw new ArgumentException("No se encontraron torneos.");
            }

            return tournaments;
        }

        //metodo para actualizar el tournamentid del equipo tomando en cuenta el coachid, del user logeado y de parametro al tournament id al que se unira
        public async Task UpdateTournamentIdByCoachId(int coachId, int newTournamentId)
        {
            // Validar entrada
            if (newTournamentId <= 0)
                throw new ArgumentException("El ID del torneo es inválido.");

            // Verificar si el entrenador existe
            var coachExists = await uow.CoachRepository.Get(c => c.CoachId == coachId).AnyAsync();
            if (!coachExists)
                throw new ArgumentException("Entrenador no encontrado.");

            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository.Get(t => t.TournamentId == newTournamentId).AnyAsync();
            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado.");

            // Obtener el único equipo asociado al CoachId
            var team = await uow.TeamRepository.Get(t => t.CoachId == coachId).FirstOrDefaultAsync();

            if (team == null)
                throw new ArgumentException("No se encontró un equipo asociado al entrenador.");

            // Actualizar el TournamentId del equipo encontrado
            team.TournamentId = newTournamentId;
            uow.TeamRepository.Update(team);

            // Guardar los cambios en la base de datos
            await uow.SaveAsync();

            //al momento de unirse se inserta en la tabla de leaguestandings el teamid y el tournamentid y los valores en 0 
            //para los campos de wins, losses, ties, goalsfor, goalsagainst, goaldifference y points
            var leagueStanding = new LeagueStanding
            {
                TeamId = team.TeamId,
                TournamentId = newTournamentId,
                Wins = 0,
                Losses = 0,
                Draws = 0,
                GoalsFor = 0,
                GoalsAgainst = 0,
                Points = 0,
                MatchesPlayed = 0
            };

            uow.LeagueStandingRepository.Insert(leagueStanding);
            await uow.SaveAsync();
        }
        //metoo que edvuelve el valor de tournamentid del equipo que coincide con el coachid del usuario logeado


        public async Task<int?> GetTeamTournamentIdByCoachId(int coachId)
        {
            // Verificar si el entrenador existe
            var coachExists = await uow.CoachRepository.Get(c => c.CoachId == coachId).AnyAsync();
            if (!coachExists)
                throw new ArgumentException("Entrenador no encontrado.");

            // Obtener el único equipo asociado al CoachId
            var team = await uow.TeamRepository.Get(t => t.CoachId == coachId).FirstOrDefaultAsync();

            if (team == null)
                throw new ArgumentException("No se encontró un equipo asociado al entrenador.");

            return team.TournamentId;
        }


       //metodo para obtener los datos de tournamentDTO del equipo del coachlogeado
       public async Task<TournamentDTO> GetTeamTournamentInfoByCoachId(int coachId)
        {
            // Verificar si el entrenador existe
            var coachExists = await uow.CoachRepository.Get(c => c.CoachId == coachId).AnyAsync();
            if (!coachExists)
                throw new ArgumentException("Entrenador no encontrado.");

            // Obtener el único equipo asociado al CoachId
            var team = await uow.TeamRepository.Get(t => t.CoachId == coachId).FirstOrDefaultAsync();

            if (team == null)
                throw new ArgumentException("No se encontró un equipo asociado al entrenador.");

            // Obtener el torneo asociado al equipo
            var tournament = await uow.TournamentRepository.Get(t => t.TournamentId == team.TournamentId).FirstOrDefaultAsync();

            if (tournament == null)
                throw new ArgumentException("No se encontró un torneo asociado al equipo.");

            // Crear el DTO con los datos del torneo
            // Busca el torneo en la base de datos usando el TournamentId del equipo
            var tournamentDTO = await uow.TournamentRepository.Get(t => t.TournamentId == team.TournamentId)
                .Include(t => t.Organizer) // Incluye la relación con Organizer
                .ThenInclude(o => o.User) // Incluye la relación con User a través de Organizer
                .Select(t => new TournamentDTO
                {
                    TournamentId = t.TournamentId,
                    TournamentName = t.TournamentName,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    TournamentTypeId = t.TournamentTypeId,
                    OrganizerUsername = t.Organizer.User.Username // Relación de navegación para obtener el username
                })
                .FirstOrDefaultAsync();

            // Si no se encuentra el torneo, lanza una excepción
            if (tournamentDTO == null)
                throw new ArgumentException("Torneo no encontrado");

            // Devuelve el torneo
            return tournamentDTO;
        }



        //metoo el 

    }
}