using GoldRogerServer.Business.Core;
using GoldRoger.Data;
using System;
using Microsoft.EntityFrameworkCore;
using GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Coach;
using GoldRogerServer.DTOs.Tournament;

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







    }
}