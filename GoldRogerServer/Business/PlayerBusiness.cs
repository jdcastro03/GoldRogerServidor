
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldRoger.Data;
using GoldRoger.Entity.Entities;
using GoldRogerServer.Business.Core;
using GoldRogerServer.DTOs.Player;
using Microsoft.EntityFrameworkCore;
    




namespace GoldRogerServer.Business

{
    public class PlayerBusiness : BaseBusiness
    {

        public PlayerBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<string?> GetPlayerPositionById(int playerId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción o devuelve null
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Devuelve la posición del jugador
            return player.Position;
        }


        public async Task<List<TeamDTO>> GetAllTeams()
        {
            var teams = await uow.TeamRepository.Get()
                .Include(t => t.Coach) // Incluye la relación con Coach
                .ThenInclude(c => c.User) // Incluye la relación con User a través de Coach
                .Select(t => new TeamDTO
                {
                    TeamId = t.TeamId, // Obtiene el TeamId
                    TeamName = t.TeamName,
                    CoachUsername = t.Coach.User.Username // Relación de navegación para obtener el username
                })
                .ToListAsync();

            if (teams == null || !teams.Any())
            {
                throw new ArgumentException("No se encontraron equipos.");
            }

            return teams;
        }


        public async Task UpdatePlayerTeam(int playerId, int teamId)
    {
    // Busca el jugador en la base de datos usando el PlayerId
    var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

    // Si no se encuentra el jugador, lanza una excepción
    if (player == null)
    {
        throw new ArgumentException("Jugador no encontrado");
    }

    // Verifica si el TeamId es válido (puedes agregar más validaciones si es necesario)
    var team = await uow.TeamRepository.Get(t => t.TeamId == teamId).FirstOrDefaultAsync();

    // Si no se encuentra el equipo, lanza una excepción
    if (team == null)
    {
        throw new ArgumentException("Equipo no encontrado");
    }

    // Actualiza el TeamId del jugador
    player.TeamId = teamId;

    // Guarda los cambios en la base de datos
    uow.PlayerRepository.Update(player);

    await uow.SaveAsync();
}





    }
}
