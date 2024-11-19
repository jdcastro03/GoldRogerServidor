using GoldRogerServer.Business.Core;
using GoldRoger.Data;
using System;
using Microsoft.EntityFrameworkCore;
using GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Coach;

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
                TournamentId = requestDTO.TournamentId
            };

            // Insertar el equipo en la base de datos
            uow.TeamRepository.Insert(newTeam);
            await uow.SaveAsync();

            return newTeam;
        }


    }
}