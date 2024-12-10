
using GoldRoger.Data;
using GoldRogerServer.Business.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldRoger.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using GoldRogerServer.DTOs.Match;
using System.Text.RegularExpressions;
using GoldRogerServer.DTOs.Referee;
using GoldRogerServer.Utils;





namespace GoldRogerServer.Business
{
    public class RefereeBusiness : BaseBusiness
    {
        public RefereeBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<string?> GetLicenseNumberById(int refereeId)
        {
            // Busca el árbitro en la base de datos usando el RefereeId
            var referee = await uow.RefereeRepository.Get(r => r.RefereeId == refereeId).FirstOrDefaultAsync();

            // Si no se encuentra el árbitro, lanza una excepción o devuelve null
            if (referee == null)
                throw new ArgumentException("Árbitro no encontrado");

            // Devuelve el número de licencia del árbitro
            return referee.LicenseNumber;
        }

        //metodo para obtener todos los refereeid de la tabla referees y con es id devolver el firstname y lastname, me devolveras los refereeid, y con ese id en la tabla users me devolveras el firstname y lastname usa apiresponse

   public async Task<List<RefereeDTO>> GetAllReferees()
{
    // Busca todos los árbitros en la base de datos, incluyendo la relación con 'User'
    var referees = await uow.RefereeRepository
        .Get()
        .Include(r => r.User) // Asegura que se cargue la relación 'User'
        .ToListAsync();

    // Crea una lista de DTOs de árbitros
    var refereeDTOs = new List<RefereeDTO>();

    // Por cada árbitro en la lista de árbitros
    foreach (var referee in referees)
    {
        // Verificar si referee.User no es null
        if (referee.User != null)
        {
            // Crea un DTO de árbitro
            var refereeDTO = new RefereeDTO
            {
                RefereeId = referee.RefereeId,
                FirstName = referee.User.FirstName,
                LastName = referee.User.LastName
            };

            // Agrega el DTO de árbitro a la lista de DTOs de árbitros
            refereeDTOs.Add(refereeDTO);
        }
    }

    // Devuelve la lista de DTOs de árbitros
    return refereeDTOs;
}




        //metodo para insertar matchid y refereeid usa apiresponse y insertrefereedto
        public async Task InsertMatchReferee(InsertRefereeDTO insertRefereeDTO)
        {
            // Busca el árbitro en la base de datos usando el RefereeId
            var referee = await uow.RefereeRepository.Get(r => r.RefereeId == insertRefereeDTO.RefereeId).FirstOrDefaultAsync();

            // Si no se encuentra el árbitro, lanza una excepción
            if (referee == null)
                throw new ArgumentException("Árbitro no encontrado");

            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == insertRefereeDTO.MatchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Crea una nueva relación entre el árbitro y el partido
            var matchReferee = new MatchReferee
            {
                MatchId = insertRefereeDTO.MatchId,
                RefereeId = insertRefereeDTO.RefereeId
            };

            // Agrega la relación entre el árbitro y el partido a la base de datos
            uow.MatchRefereeRepository.Insert(matchReferee);

            // Guarda los cambios en la base de datos
            await uow.SaveAsync();
        }



        //metodo para actualizar el refereeid de la tabla matchreferee utiliza el insertrefereedto y apiresponse
        public async Task UpdateMatchReferee(int matchId, int newRefereeId)
        {
            // Busca la relación entre el partido y el árbitro en la base de datos usando el MatchId
            var matchReferee = await uow.MatchRefereeRepository.Get(mr => mr.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra la relación entre el partido y el árbitro, lanza una excepción
            if (matchReferee == null)
                throw new ArgumentException("Relación entre partido y árbitro no encontrada");

            // Busca el árbitro en la base de datos usando el RefereeId
            var referee = await uow.RefereeRepository.Get(r => r.RefereeId == newRefereeId).FirstOrDefaultAsync();

            // Si no se encuentra el árbitro, lanza una excepción
            if (referee == null)
                throw new ArgumentException("Árbitro no encontrado");

            // Actualiza el árbitro en la relación entre el partido y el árbitro
            matchReferee.RefereeId = newRefereeId;

            // Asegúrate de que la entidad esté siendo rastreada correctamente
            uow.dbcontext.Entry(matchReferee).State = EntityState.Modified;

            // Actualiza la relación entre el partido y el árbitro en la base de datos
            uow.MatchRefereeRepository.Update(matchReferee);

            // Guarda los cambios
            await uow.SaveAsync();
        }












        //metodo pasara de parametro matchid, y buscaras que refereeid coincide con ese en matchreferees, posteriormente usaras ese refereeid para buscar en la tabla user el firstname y lastname, haciendo una relacion entre las tablas, usa apiresponse y refereedto
        public async Task<List<RefereeDTO>> GetRefereeByMatchId(int matchId)
        {
            // Busca las relaciones entre partidos y árbitros en la base de datos usando el MatchId
            var matchReferees = await uow.MatchRefereeRepository.Get(mr => mr.MatchId == matchId).ToListAsync();

            // Crea una lista de DTOs de árbitros
            var refereeDTOs = new List<RefereeDTO>();

            // Por cada relación entre partido y árbitro en la lista de relaciones
            foreach (var matchReferee in matchReferees)
            {
                // Busca el árbitro en la base de datos usando el RefereeId
                var referee = await uow.RefereeRepository.Get(r => r.RefereeId == matchReferee.RefereeId).FirstOrDefaultAsync();

                // Si no se encuentra el árbitro, continúa con la siguiente relación
                if (referee == null)
                    continue;

                // Busca el usuario en la base de datos usando el RefereeId
                var user = await uow.UserRepository.Get(u => u.UserId == referee.RefereeId).FirstOrDefaultAsync();

                // Si no se encuentra el usuario, continúa con la siguiente relación
                if (user == null)
                    continue;

                // Crea un DTO de árbitro
                var refereeDTO = new RefereeDTO
                {
                    RefereeId = referee.RefereeId,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                // Agrega el DTO de árbitro a la lista de DTOs de árbitros
                refereeDTOs.Add(refereeDTO);
            }

            // Devuelve la lista de DTOs de árbitros
            return refereeDTOs;
        }

        //metodo que tomara el refeeid del usuario logeado y buscara en la tabla matchreferee ese refereeid y tomara el matchid asignado a el y con ese matchid buscara en la tabla match el team1id y team2id, posteriormente con esos ids buscara en la tabla team el nombre de los equipos, ademas de que con ese matchid, obtendras la date de la tabla match, usa apiresponse y matchrefereedto
        public async Task<List<MatchRefereeDTO>> GetMatchesByRefereeId(int refereeId)
        {
            // Busca todas las relaciones entre partidos y el árbitro en la base de datos usando el RefereeId
            var matchReferees = await uow.MatchRefereeRepository.Get(mr => mr.RefereeId == refereeId).ToListAsync();

            // Si no se encuentra ninguna relación, lanza una excepción
            if (!matchReferees.Any())
                throw new ArgumentException("No se encontraron partidos asociados al árbitro");

            var matchRefereeDTOs = new List<MatchRefereeDTO>();

            foreach (var matchReferee in matchReferees)
            {
                // Busca el partido correspondiente usando el MatchId
                var match = await uow.MatchRepository.Get(m => m.MatchId == matchReferee.MatchId).FirstOrDefaultAsync();

                // Si no se encuentra el partido, omite este registro
                if (match == null)
                    continue;

                // Busca los equipos correspondientes
                var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
                var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

                // Si no se encuentran ambos equipos, omite este registro
                if (team1 == null || team2 == null)
                    continue;

                // Crea el DTO con la información del partido
                matchRefereeDTOs.Add(new MatchRefereeDTO
                {
                    MatchId = match.MatchId,
                    Team1Name = team1.TeamName,
                    Team2Name = team2.TeamName,
                    Date = match.Date
                });
            }

            // Devuelve la lista de DTOs
            return matchRefereeDTOs;
        }



        //metodo que recibira de parametro matchid, y obtendra el teamname del team1 y team2, ademas de la fecha del partido, usa apiresponse y matchrefereedto
        public async Task<MatchRefereeDTO> GetMatchByMatchId(int matchId)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Busca los equipos correspondientes
            var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
            var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

            // Si no se encuentran ambos equipos, lanza una excepción
            if (team1 == null || team2 == null)
                throw new ArgumentException("Equipos no encontrados");

            // Crea el DTO con la información del partido
            var matchRefereeDTO = new MatchRefereeDTO
            {
                MatchId = match.MatchId,
                Team1Name = team1.TeamName,
                Team2Name = team2.TeamName,
                Date = match.Date
            };

            // Devuelve el DTO
            return matchRefereeDTO;
        }


        //metodo que tomaras de parametro un matchid y buscaras el team1id y buscaras todos los jugadores de ese equipo obteniendo su playerid, firstname y position, usa apiresponse y matchteamdto ten en cuenta que el firstname lo obtendras con el playerid buscando en la tabla user

        public async Task<List<MatchTeamDTO>> GetPlayers1ByMatchId(int matchId)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Busca el equipo 1 en la base de datos usando el Team1Id
            var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();

            // Si no se encuentra el equipo 1, lanza una excepción
            if (team1 == null)
                throw new ArgumentException("Equipo 1 no encontrado");

            // Busca todos los jugadores del equipo 1
            var players = await uow.PlayerRepository.Get(p => p.TeamId == team1.TeamId).ToListAsync();

            // Crea una lista de DTOs de jugadores
            var matchTeamDTOs = new List<MatchTeamDTO>();

            // Por cada jugador en la lista de jugadores
            foreach (var player in players)
            {
                // Verificar si player.User es null
                var user = await uow.UserRepository.Get(u => u.UserId == player.PlayerId).FirstOrDefaultAsync();
                if (user == null)
                    continue;

                // Crea un DTO de jugador
                var matchTeamDTO = new MatchTeamDTO
                {
                    PlayerId = player.PlayerId,
                    FirstName = user.FirstName,
                    Position = player.Position
                };

                // Agrega el DTO de jugador a la lista de DTOs de jugadores
                matchTeamDTOs.Add(matchTeamDTO);
            }

            // Devuelve la lista de DTOs de jugadores
            return matchTeamDTOs;
        }

        //el mismo pero para el teamid2

        public async Task<List<MatchTeamDTO>> GetPlayers2ByMatchId(int matchId)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Busca el equipo 2 en la base de datos usando el Team2Id
            var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

            // Si no se encuentra el equipo 2, lanza una excepción
            if (team2 == null)
                throw new ArgumentException("Equipo 2 no encontrado");

            // Busca todos los jugadores del equipo 2
            var players = await uow.PlayerRepository.Get(p => p.TeamId == team2.TeamId).ToListAsync();

            // Crea una lista de DTOs de jugadores
            var matchTeamDTOs = new List<MatchTeamDTO>();

            // Por cada jugador en la lista de jugadores
            foreach (var player in players)
            {
                // Verificar si player.User es null
                var user = await uow.UserRepository.Get(u => u.UserId == player.PlayerId).FirstOrDefaultAsync();
                if (user == null)
                    continue;

                // Crea un DTO de jugador
                var matchTeamDTO = new MatchTeamDTO
                {
                    PlayerId = player.PlayerId,
                    FirstName = user.FirstName,
                    Position = player.Position
                };

                // Agrega el DTO de jugador a la lista de DTOs de jugadores
                matchTeamDTOs.Add(matchTeamDTO);
            }

            // Devuelve la lista de DTOs de jugadores
            return matchTeamDTOs;
        }



        //metodo que recibira como parametro el playerid y matchid y aumentara 1 al team1goals o team2goals dependiendo del teamid del playerid, ademas de aumentar en 1 la cantidad de goles del jugador usando el playerid en la tabla de playerstats el campo de goal asi como lo hace e metoo addgoala

        public async Task AddGoal(int playerId, int matchId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Busca el registro de estadísticas del jugador en la base de datos usando el PlayerId
            var playerStats = await uow.PlayerStatsRepository.Get(ps => ps.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el registro de estadísticas del jugador, lanza una excepción
            if (playerStats == null)
                throw new ArgumentException("Estadísticas del jugador no encontradas");

            // Aumenta en 1 el número de goles del jugador
            playerStats.Goals++;

            // Asegúrate de que la entidad esté siendo rastreada correctamente
            uow.dbcontext.Entry(playerStats).State = EntityState.Modified;

            // Actualiza el registro de estadísticas del jugador en la base de datos
            uow.PlayerStatsRepository.Update(playerStats);

            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Aumenta en 1 el número de goles del equipo correspondiente
            if (player.TeamId == match.Team1Id)
                match.Team1Goals++;
            else if (player.TeamId == match.Team2Id)
                match.Team2Goals++;

            // Asegúrate de que la entidad esté siendo rastreada correctamente
            uow.dbcontext.Entry(match).State = EntityState.Modified;

            // Actualiza el partido en la base de datos
            uow.MatchRepository.Update(match);

            // Guarda los cambios
            await uow.SaveAsync();
        }

        //metodo para addyellow card, el cual recibira un playerid y matchid, y aumentara en 1 el campo de yellowcards en la tabla playerstats solamente

        public async Task AddYellowCard(int playerId, int matchId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Busca el registro de estadísticas del jugador en la base de datos usando el PlayerId
            var playerStats = await uow.PlayerStatsRepository.Get(ps => ps.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el registro de estadísticas del jugador, lanza una excepción
            if (playerStats == null)
                throw new ArgumentException("Estadísticas del jugador no encontradas");

            // Aumenta en 1 el número de tarjetas amarillas del jugador
            playerStats.YellowCards++;

            // Asegúrate de que la entidad esté siendo rastreada correctamente
            uow.dbcontext.Entry(playerStats).State = EntityState.Modified;

            // Actualiza el registro de estadísticas del jugador en la base de datos
            uow.PlayerStatsRepository.Update(playerStats);

            // Guarda los cambios
            await uow.SaveAsync();
        }


        //addredcard, el cual recibira un playerid y matchid, y aumentara en 1 el campo de redcards en la tabla playerstats solamente

        public async Task AddRedCard(int playerId, int matchId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Busca el registro de estadísticas del jugador en la base de datos usando el PlayerId
            var playerStats = await uow.PlayerStatsRepository.Get(ps => ps.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el registro de estadísticas del jugador, lanza una excepción
            if (playerStats == null)
                throw new ArgumentException("Estadísticas del jugador no encontradas");

            // Aumenta en 1 el número de tarjetas rojas del jugador
            playerStats.RedCards++;

            // Asegúrate de que la entidad esté siendo rastreada correctamente
            uow.dbcontext.Entry(playerStats).State = EntityState.Modified;

            // Actualiza el registro de estadísticas del jugador en la base de datos
            uow.PlayerStatsRepository.Update(playerStats);

            // Guarda los cambios
            await uow.SaveAsync();
        }


        //metodo que de parametro le an un matchid y obtendra el team1goals y team2goals del partido solamente

        public async Task<MatchGoalsDTO> GetGoalsByMatchId(int matchId)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Crea un DTO con la cantidad de goles de cada equipo
            var matchGoalsDTO = new MatchGoalsDTO
            {
                Team1Goals = match.Team1Goals,
                Team2Goals = match.Team2Goals,
                IsFinished = match.IsFinished
            };

            // Devuelve el DTO
            return matchGoalsDTO;
        }
          

            
        //metodo llamado startgame el cualm recibe un matchid, y se actuzlizara el campo de active a 1 en la tabla match, pero primera verifica si isfinished es igual a 1, si es asi lanza una excepcion, usa apiresponse
        public async Task StartMatch(int matchId)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Si el partido ya ha terminado, lanza una excepción
            if (match.IsFinished == true)
                throw new ArgumentException("El partido ya ha terminado");

            // Actualiza el campo 'Active' del partido a 1
            match.Active = true;

            // Asegúrate de que la entidad esté siendo rastreada correctamente
            uow.dbcontext.Entry(match).State = EntityState.Modified;

            // Actualiza el partido en la base de datos
            uow.MatchRepository.Update(match);

            // Guarda los cambios
            await uow.SaveAsync();
        }



        //metodo llamado endgame el cual recibe un matchid, y se actualizara el campo de isfinished a 1 en la tabla match, pero primero verifica si active es igual a 0, si es asi lanza una excepcion, usa apiresponse
        public async Task EndMatch(int matchId)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Si el partido no está activo, lanza una excepción
            if (match.Active == false)
                throw new ArgumentException("El partido no está activo");

            // Actualiza el campo 'IsFinished' del partido a 1
            match.IsFinished = true;
            match.Active = false;

            // Asegúrate de que la entidad esté siendo rastreada correctamente
            uow.dbcontext.Entry(match).State = EntityState.Modified;

            // Actualiza el partido en la base de datos
            uow.MatchRepository.Update(match);

            // Guarda los cambios
            await uow.SaveAsync();
        }
        


        //metodo donde se recibe el matchid, y devulve true o false depeniendo si el campo de active es 1 o 0 en la tabla match, usa apiresponse
        public async Task<bool> IsMatchActive(int matchId)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Devuelve si el partido está activo
            return match.Active;
        }


        //metodo que devolvera todos los campos de de matchhomeDTO obtendra todos los match que en el campo active sea 1 o true, y de cada match obtendra los team1id y team2id, y con esos ids obtendra el teamname de la tabla team, usa apiresponse y matchhomedto

        public async Task<List<MatchHomeDTO>> GetActiveMatches()
        {
            // Busca todos los partidos activos en la base de datos
            var matches = await uow.MatchRepository.Get(m => m.Active == true).ToListAsync();

            // Crea una lista de DTOs de partidos
            var matchHomeDTOs = new List<MatchHomeDTO>();

            // Por cada partido en la lista de partidos
            foreach (var match in matches)
            {
                // Busca los equipos correspondientes
                var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
                var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

                // Verifica si los equipos fueron encontrados
                if (team1 != null && team2 != null)
                {
                    // Mapea la información a MatchHomeDTO
                    var matchHomeDTO = new MatchHomeDTO
                    {
                        MatchId = match.MatchId,
                        Team1Name = team1.TeamName,
                        Team2Name = team2.TeamName,
                        Team1Goals = match.Team1Goals,
                        Team2Goals = match.Team2Goals,
                    };

                    // Agrega el DTO a la lista
                    matchHomeDTOs.Add(matchHomeDTO);
                    
                }
            }

            // Devuelve la lista de DTOs
            return matchHomeDTOs;
        }

        //public async Task<APIResponse<List<MatchHomeDTO>>> GetActiveMatches()
        //{
        //    // Busca todos los partidos activos en la base de datos
        //    var matches = await uow.MatchRepository.Get(m => m.Active == true).ToListAsync();

        //    // Crea una lista de DTOs de partidos
        //    var matchHomeDTOs = new List<MatchHomeDTO>();

        //    // Por cada partido en la lista de partidos
        //    foreach (var match in matches)
        //    {
        //        // Busca los equipos correspondientes
        //        var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
        //        var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

        //        // Verifica si los equipos fueron encontrados
        //        if (team1 != null && team2 != null)
        //        {
        //            // Mapea la información a MatchHomeDTO
        //            var matchHomeDTO = new MatchHomeDTO
        //            {
        //                MatchId = match.MatchId,
        //                Team1Name = team1.TeamName,
        //                Team2Name = team2.TeamName,
        //                Team1Goals = match.Team1Goals,
        //                Team2Goals = match.Team2Goals,
        //            };

        //            // Agrega el DTO a la lista
        //            matchHomeDTOs.Add(matchHomeDTO);
        //        }
        //    }

        //    // Devuelve la lista de DTOs
           
        //}



































        //public async Task AddGoal(int playerId, int matchId)
        //    {
        //        // Busca el jugador en la base de datos usando el PlayerId
        //        var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

        //        // Si no se encuentra el jugador, lanza una excepción
        //        if (player == null)
        //            throw new ArgumentException("Jugador no encontrado");

        //        // Busca el registro de estadísticas del jugador en la base de datos usando el PlayerId
        //        var playerStats = await uow.PlayerStatsRepository.Get(ps => ps.PlayerId == playerId).FirstOrDefaultAsync();

        //        // Si no se encuentra el registro de estadísticas del jugador, lanza una excepción
        //        if (playerStats == null)
        //            throw new ArgumentException("Estadísticas del jugador no encontradas");

        //        // Aumenta en 1 el número de goles del jugador
        //        playerStats.Goals++;

        //        // Asegúrate de que la entidad esté siendo rastreada correctamente
        //        uow.dbcontext.Entry(playerStats).State = EntityState.Modified;

        //        // Actualiza el registro de estadísticas del jugador en la base de datos
        //        uow.PlayerStatsRepository.Update(playerStats);



        //        // Guarda los cambios
        //        await uow.SaveAsync();
        //    }



        //public async Task<List<MatchTeamDTO>> GetPlayersByMatchId(int matchId)
        //{
        //    // Busca el partido en la base de datos usando el MatchId
        //    var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

        //    // Si no se encuentra el partido, lanza una excepción
        //    if (match == null)
        //        throw new ArgumentException("Partido no encontrado");

        //    // Busca el equipo 1 en la base de datos usando el Team1Id
        //    var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();

        //    // Si no se encuentra el equipo 1, lanza una excepción
        //    if (team1 == null)
        //        throw new ArgumentException("Equipo 1 no encontrado");

        //    // Busca todos los jugadores del equipo 1
        //    var players = await uow.PlayerRepository.Get(p => p.TeamId == team1.TeamId).ToListAsync();

        //    // Crea una lista de DTOs de jugadores
        //    var matchTeamDTOs = new List<MatchTeamDTO>();

        //    // Por cada jugador en la lista de jugadores
        //    foreach (var player in players)
        //    {
        //        // Verificar si player.User es null
        //        var firstName = player.User?.FirstName ?? "Nombre no disponible"; // Se asigna un valor por defecto si es null

        //        // Crea un DTO de jugador
        //        var matchTeamDTO = new MatchTeamDTO
        //        {
        //            PlayerId = player.PlayerId,
        //            FirstName = firstName.User.Firstname,
        //            Position = player.Position
        //        };

        //        // Agrega el DTO de jugador a la lista de DTOs de jugadores
        //        matchTeamDTOs.Add(matchTeamDTO);
        //    }

        //    // Devuelve la lista de DTOs de jugadores
        //    return matchTeamDTOs;
        //}













    }
}
