using GoldRogerServer.Business.Core;
using GoldRoger.Data;
using System;
using Microsoft.EntityFrameworkCore;
using GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Organizer;

namespace GoldRogerServer.Business
{
    public class OrganizerBusiness : BaseBusiness
    {
        public OrganizerBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<string?> GetOrganizerNameById(int organizerId)
        {
            // Busca el organizador en la base de datos usando el OrganizerId
            var organizer = await uow.OrganizerRepository.Get(o => o.OrganizerId == organizerId).FirstOrDefaultAsync();

            // Si no se encuentra el organizador, lanza una excepción o devuelve null
            if (organizer == null)
                throw new ArgumentException("Organizador no encontrado");

            // Devuelve el nombre del organizador
            return organizer.OrganizationName;
        }

        //creame el metodo de CreateTournament 
        public async Task<Tournament> CreateTournament(CreateTournamentRequestDTO requestDTO, int organizerId)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(requestDTO.TournamentName) || requestDTO.TournamentName.Trim().Length <= 3)
                throw new ArgumentException("Nombre de torneo inválido");

            if (requestDTO.StartDate >= requestDTO.EndDate)
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de finalización");

            // Verificar si el organizador existe
            var organizerExists = await uow.OrganizerRepository.Get(o => o.OrganizerId == organizerId).AnyAsync();
            if (!organizerExists)
                throw new ArgumentException("Organizador no encontrado");

            // Crear el torneo con los datos del DTO y el OrganizerId del usuario logueado
            var newTournament = new Tournament
            {
                OrganizerId = organizerId,
                TournamentName = requestDTO.TournamentName,
                StartDate = requestDTO.StartDate,
                EndDate = requestDTO.EndDate,
                TournamentTypeId = requestDTO.TournamentTypeId,

            };

            // Insertar el torneo en la base de datos
            uow.TournamentRepository.Insert(newTournament);
            await uow.SaveAsync();

            return newTournament;
        }

        public async Task<List<TournamentInfoDTO>> GetTournamentsByOrganizerId(int organizerId)
        {
            // Verificar si el organizador existe
            var organizerExists = await uow.OrganizerRepository.Get(o => o.OrganizerId == organizerId).AnyAsync();
            if (!organizerExists)
                throw new ArgumentException("Organizador no encontrado");

            // Obtener la lista de torneos asociados al organizerId y proyectarlos a TournamentInfoDTO
            var tournaments = await uow.TournamentRepository
                .Get(t => t.OrganizerId == organizerId)  // Filtro por organizerId
                .Select(t => new TournamentInfoDTO
                {
                    TournamentId = t.TournamentId,
                    TournamentName = t.TournamentName,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    TournamentTypeId = t.TournamentTypeId
                })
                .ToListAsync();  // Ejecutar la consulta y obtener la lista de torneos

            return tournaments;
        }


        //metodo para obtener el tournamentName con el tournamentId
        public async Task<string?> GetTournamentNameById(int tournamentId)
        {
            // Busca el torneo en la base de datos usando el TournamentId
            var tournament = await uow.TournamentRepository.Get(t => t.TournamentId == tournamentId).FirstOrDefaultAsync();

            // Si no se encuentra el torneo, lanza una excepción o devuelve null
            if (tournament == null)
                throw new ArgumentException("Torneo no encontrado");

            // Devuelve el nombre del torneo
            return tournament.TournamentName;
        }


        //metoo para obtener info de un torneo con el tournamentId
        public async Task<GlobalTournamentInfoDTO> GetTournamentInfoById(int tournamentId)
        {
            // Busca el torneo en la base de datos usando el TournamentId
            var tournament = await uow.TournamentRepository.Get(t => t.TournamentId == tournamentId).FirstOrDefaultAsync();

            // Si no se encuentra el torneo, lanza una excepción o devuelve null
            if (tournament == null)
                throw new ArgumentException("Torneo no encontrado");

            // Devuelve el nombre del torneo
            return new GlobalTournamentInfoDTO
            {
                OrganizerId = tournament.OrganizerId,
                TournamentName = tournament.TournamentName,
                StartDate = tournament.StartDate,
                EndDate = tournament.EndDate,
                TournamentTypeId = tournament.TournamentTypeId
            };



        }


        //metodo para obtener el todos los teamnames, y teamid recibiendo de parametro el tournamentid



        public async Task<List<TeamInfoDTO>> GetTeamsByTournamentId(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener los equipos asociados al torneo
            var teams = await uow.TeamRepository
                .Get(team => team.TournamentId == tournamentId) // Filtro por TournamentId
                .Select(team => new TeamInfoDTO
                {
                    TeamId = team.TeamId,
                    TeamName = team.TeamName
                })
                .ToListAsync();

            return teams;
        }









    }
}

