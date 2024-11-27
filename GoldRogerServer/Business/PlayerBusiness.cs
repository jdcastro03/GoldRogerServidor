
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

        //metodo para obtener el campo de temaid de la tabla de player tomando en cuenta el playerid, del usuario logeado
        public async Task<int?> GetPlayerTeamId(int playerId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Devuelve el TeamId del jugador, que puede ser null
            return player.TeamId;
        }

        //metodo para obtener el teamname del basandose en el teamid que tenga el playerid logeado
        public async Task<string?> GetPlayerTeamName(int playerId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Si el jugador no tiene un equipo asignado, devuelve null
            if (player.TeamId == null)
                return null;

            // Busca el equipo en la base de datos usando el TeamId del jugador
            var team = await uow.TeamRepository.Get(t => t.TeamId == player.TeamId).FirstOrDefaultAsync();

            // Si no se encuentra el equipo, lanza una excepción
            if (team == null)
                throw new ArgumentException("Equipo no encontrado");

            // Devuelve el nombre del equipo
            return team.TeamName;
        }

        //metodo para obtener el firstname y lastname de la tabla users tomando el coachid que coincida con el teamname del playerid logeado
        public async Task<string?> GetCoachName(int playerId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Si el jugador no tiene un equipo asignado, devuelve null
            if (player.TeamId == null)
                return null;

            // Busca el equipo en la base de datos usando el TeamId del jugador
            var team = await uow.TeamRepository.Get(t => t.TeamId == player.TeamId).FirstOrDefaultAsync();

            // Si no se encuentra el equipo, lanza una excepción
            if (team == null)
                throw new ArgumentException("Equipo no encontrado");

            // Busca el coach en la base de datos usando el CoachId del equipo
            var coach = await uow.CoachRepository.Get(c => c.CoachId == team.CoachId).FirstOrDefaultAsync();

            // Si no se encuentra el coach, lanza una excepción
            if (coach == null)
                throw new ArgumentException("Entrenador no encontrado");

            // Busca el usuario en la base de datos usando el coachid pero en la tabla users
            var user = await uow.UserRepository.Get(u => u.UserId == coach.CoachId).FirstOrDefaultAsync();

            // Si no se encuentra el usuario, lanza una excepción
            if (user == null)
                throw new ArgumentException("Usuario no encontrado");

            // Devuelve el nombre completo del coach
            return $"{user.FirstName} {user.LastName}";
        }

        //metodo donde se recibira el teamname como parametro, y se tomara el teamid de ese teamname en la tabla teams, despues me traeras a todos los players que tengan ese teamid
        public async Task<List<PlayerDTO>> GetPlayersByTeamName(string teamName)
        {
            // Busca el equipo en la base de datos usando el TeamName
            var team = await uow.TeamRepository.Get(t => t.TeamName == teamName).FirstOrDefaultAsync();

            // Si no se encuentra el equipo, lanza una excepción
            if (team == null)
                throw new ArgumentException("Equipo no encontrado");

            // Busca los jugadores en la base de datos usando el TeamId del equipo
            var players = await uow.PlayerRepository.Get(p => p.TeamId == team.TeamId)
                .Select(p => new PlayerDTO
                {
                    PlayerId = p.PlayerId,
                    FirstName = p.User.FirstName,
                    LastName = p.User.LastName,
                    Position = p.Position
                })
                .ToListAsync();

            // Si no se encuentran jugadores, lanza una excepción
            if (players == null || !players.Any())
                throw new ArgumentException("No se encontraron jugadores en el equipo");

            // Devuelve la lista de jugadores
            return players;
        }

        //metodo para obtener los goles, tarjetas amarillas y rojas del jugador logeado con su playerid, ademas de obtener su firstname y lastname
        public async Task<PlayerStatsDTO> GetPlayerStats(int playerId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId)
                .Include(p => p.User) // Incluye la relación con User
                .Include(p => p.PlayerStats) // Incluye la relación con PlayerStats
                .FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Si el jugador no tiene asociado un usuario, lanza una excepción
            if (player.User == null)
                throw new ArgumentException("Usuario no encontrado para el jugador");

            // Si el jugador no tiene estadísticas, lanza una excepción
            if (player.PlayerStats == null)
                throw new ArgumentException("Estadísticas del jugador no encontradas");

            // Crea un objeto PlayerStatsDTO con los datos del jugador
            var playerStats = new PlayerStatsDTO
            {
                PlayerId = player.PlayerId,
                FirstName = player.User.FirstName,
                LastName = player.User.LastName,
                Goals = player.PlayerStats.Goals,
                YellowCards = player.PlayerStats.YellowCards,
                RedCards = player.PlayerStats.RedCards
            };

            // Devuelve las estadísticas del jugador
            return playerStats;
        }
    }

}
