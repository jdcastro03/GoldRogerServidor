using GoldRogerServer.Business.Core;
using GoldRoger.Data;
using System;
using Microsoft.EntityFrameworkCore;
using GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Organizer;

using GoldRogerServer.DTOs.Match;
using GoldRogerServer.DTOs.Player;

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



        //METODO ELIMINATORIA
        //metodo que toma de parametro el tournamenid y va generar los partidos de eliminatoria directa son 8 equipos por lo tanto generara 4 partidos en la primera ronda y 2 en la segunda y 1 en la final
        public async Task CreateQuarterFinals(int tournamentId)
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
                .ToListAsync();

            // Validar que haya al menos 8 equipos
            if (teams.Count < 8)
                throw new ArgumentException("Se necesitan al menos 8 equipos para crear los partidos");

            // Crear los partidos de la primera ronda
            var matches = new List<Match>();
            for (int i = 0; i < 4; i++)
            {
                var match = new Match
                {
                    TournamentId = tournamentId,
                    Team1Id = teams[i].TeamId,
                    Team2Id = teams[7 - i].TeamId,
                    Team1Goals = 0,
                    Team2Goals = 0,
                    Date = null,
                    IsFinished = false,
                    Stage = 4
                };

                matches.Add(match);
            }

            // Insertar los partidos en la base de datos
            uow.MatchRepository.InsertRange(matches);
            await uow.SaveAsync();
            //llama el metoo para getQuarterFinalsMatches



        }

        //metodo para obtener los nombres de los equipos de cada partido de los cuartos de final
        public async Task<List<MatchInfoDTO>> GetQuarterFinalsMatches(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener los partidos que tengan el del modelo match la propiedadd Stage con valor 4
            var currentRoundMatches = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 4)
                .ToListAsync();

           


            // Obtener los nombres de los equipos de los partidos
            var matches = new List<MatchInfoDTO>();
            foreach (var match in currentRoundMatches)
            {
                var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
                var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

                matches.Add(new MatchInfoDTO
                {
                    MatchId = match.MatchId,
                    Team1Name = team1.TeamName,
                    Team2Name = team2.TeamName
                });
            }

            return matches;
        }

       
        //metodo que recibira como parametro el torunamentid y verifica los partidos que se jugaron y los siguientes dos partidos con los teamdid que ganaron, pero primero debe verificar si los 4 partidos fueron finalizados, ten en cuenta que avanzaron los 4 equipos que hayan hecho mas goles que el contrario y se generaran los os partiodos con esos 4 


        
        public async Task CreateSemiFinals(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

           //verifica si todos los partidos que tengan stage 4 en isfinishe tengan 1
           var allMatchesFinished = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 4 && m.IsFinished == false)
                .AnyAsync();

            if (allMatchesFinished)
                throw new ArgumentException("No se han jugado todos los partidos de la ronda anterior");

            // Obtener los partidos de la ronda anterior
            var currentRoundMatches = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 4)
                .ToListAsync();


            // Obtener los 4 equipos que avanzan a la siguiente ronda
            var nextRoundTeams = new List<int>();
            foreach (var match in currentRoundMatches)
            {
                if (match.Team1Goals > match.Team2Goals)
                    nextRoundTeams.Add(match.Team1Id);
                else
                    nextRoundTeams.Add(match.Team2Id);
            }

            // Crear los partidos de la siguiente ronda
            var matches = new List<Match>();
            for (int i = 0; i < 2; i++)
            {
                var match = new Match
                {
                    TournamentId = tournamentId,
                    Team1Id = nextRoundTeams[i],
                    Team2Id = nextRoundTeams[3 - i],
                    Team1Goals = 0,
                    Team2Goals = 0,
                    Date = null,
                    IsFinished = false,
                    Stage = 2
                };

                matches.Add(match);
            }

            // Insertar los partidos en la base de datos
            uow.MatchRepository.InsertRange(matches);
            await uow.SaveAsync();

            //aqui mismo quiero que hagas 
        }

        
        //metoo para el getsemifinalsmatches
        public async Task<List<MatchInfoDTO>> GetSemiFinalsMatches(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener los partidos que tengan el del modelo match la propiedadd Stage con valor 2
            var currentRoundMatches = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 2)
                .ToListAsync();

            // Obtener los nombres de los equipos de los partidos
            var matches = new List<MatchInfoDTO>();
            foreach (var match in currentRoundMatches)
            {
                var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
                var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

                matches.Add(new MatchInfoDTO
                {
                    MatchId = match.MatchId,
                    Team1Name = team1.TeamName,
                    Team2Name = team2.TeamName
                });
            }

            return matches;
        }


        


   

        //metoddo que verifica si los dos partidos de la ronda anterior osea los ultimos 2 registros fueron finalizados y genera el partido final con los dos equipos que ganaron de esos ultimos dos partidos
        public async Task CreateFinal(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            //verificar que los matches que tengan stage 2 en isfinished tengan 1
            var allMatchesFinished = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 2 && m.IsFinished == false)
                .AnyAsync();

            if (allMatchesFinished)
                throw new ArgumentException("No se han jugado todos los partidos de la ronda anterior");

            // Obtener los partidos de la ronda anterior
            var previousRoundMatches = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 2)
                .ToListAsync();

            // Obtener los 2 equipos que avanzan a la final
            var finalTeams = new List<int>();
            foreach (var match in previousRoundMatches)
            {
                if (match.Team1Goals > match.Team2Goals)
                    finalTeams.Add(match.Team1Id);
                else
                    finalTeams.Add(match.Team2Id);
            }

            // Crear el partido final
            var finalMatch = new Match
            {
                TournamentId = tournamentId,
                Team1Id = finalTeams[0],
                Team2Id = finalTeams[1],
                Team1Goals = 0,
                Team2Goals = 0,
                Date = null,
                IsFinished = false,
                Stage = 1
            };

            // Insertar el partido final en la base de datos
            uow.MatchRepository.Insert(finalMatch);
            await uow.SaveAsync();
        }

        //getfinalmatch
        public async Task<MatchInfoDTO> GetFinalMatch(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener el partido que tenga el del modelo match la propiedadd Stage con valor 1
            var finalMatch = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 1)
                .FirstOrDefaultAsync();

            // Obtener los nombres de los equipos del partido
            var team1 = await uow.TeamRepository.Get(t => t.TeamId == finalMatch.Team1Id).FirstOrDefaultAsync();
            var team2 = await uow.TeamRepository.Get(t => t.TeamId == finalMatch.Team2Id).FirstOrDefaultAsync();

            return new MatchInfoDTO
            {
                MatchId = finalMatch.MatchId,
                Team1Name = team1.TeamName,
                Team2Name = team2.TeamName
            };
        }
    

        //FIN CREACION DE ELIMINATORIAS 8

        //OBTENCION DE RESULTADOS

        //metodo para obtener los resultados de los partiodos que en stage sea 4, y en isfinished sea 1 utilizano matchresultdto
        public async Task<List<MatchResultDTO>> GetQuarterFinalsResults(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener los partidos que tengan el del modelo match la propiedadd Stage con valor 4
            var currentRoundMatches = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 4 && m.IsFinished == true)
                .ToListAsync();

            // Obtener los resultados de los partidos
            var results = new List<MatchResultDTO>();
            foreach (var match in currentRoundMatches)
            {
                var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
                var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

                results.Add(new MatchResultDTO
                {
                    MatchId = match.MatchId,
                    Team1Name = team1.TeamName,
                    Team1Goals = match.Team1Goals,
                    Team2Name = team2.TeamName,
                    Team2Goals = match.Team2Goals
                });
            }

            return results;
        }


        //metodo para obtener los resultados de los partiodos que en stage sea 2, y en isfinished sea 1 utilizano matchresultdto
        public async Task<List<MatchResultDTO>> GetSemiFinalsResults(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener los partidos que tengan el del modelo match la propiedadd Stage con valor 2
            var currentRoundMatches = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 2 && m.IsFinished == true)
                .ToListAsync();

            // Obtener los resultados de los partidos
            var results = new List<MatchResultDTO>();
            foreach (var match in currentRoundMatches)
            {
                var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
                var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

                results.Add(new MatchResultDTO
                {
                    MatchId = match.MatchId,
                    Team1Name = team1.TeamName,
                    Team1Goals = match.Team1Goals,
                    Team2Name = team2.TeamName,
                    Team2Goals = match.Team2Goals
                });
            }

            return results;
        }


        //metodo para obtener los resultados de los partiodos que en stage sea 1, y en isfinished sea 1 utilizano matchresultdto
        public async Task<MatchResultDTO> GetFinalResult(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener el partido que tenga el del modelo match la propiedadd Stage con valor 1
            var finalMatch = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.Stage == 1 && m.IsFinished == true)
                .FirstOrDefaultAsync();

            // Obtener los nombres de los equipos del partido
            var team1 = await uow.TeamRepository.Get(t => t.TeamId == finalMatch.Team1Id).FirstOrDefaultAsync();
            var team2 = await uow.TeamRepository.Get(t => t.TeamId == finalMatch.Team2Id).FirstOrDefaultAsync();

            return new MatchResultDTO
            {
                MatchId = finalMatch.MatchId,
                Team1Name = team1.TeamName,
                Team1Goals = finalMatch.Team1Goals,
                Team2Name = team2.TeamName,
                Team2Goals = finalMatch.Team2Goals
            };
        }

        //metodo metoo para obtener las estadisticas de todos los jugadores de un torneo recibieno como parametro el tournamentid y de ahi obtenremos todos los equipos que en campo de tournamentid sea igual al parametro y de ahi obtendremos todos los jugadores que en campo de teamid sea igual al teamid de los equipos y de ahi obtendremos todos los stats de los jugadores utilizando playerstats, recuera que el firstname y lastname los obtendremos buscando en la tabla user con el userid de los jugadores y de ahi obtendremos el firstname y lastname y las stats de la tabla playerstats utilizando  onde tambien se obtendra el teamname de cada jugador usaremos este tournamentplayerstatsdto
        public async Task<List<TournamentPlayerStatsDTO>> GetPlayerStatsByTournamentId(int tournamentId)
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
                .ToListAsync();

            // Obtener las estadísticas de los jugadores de los equipos
            var playerStats = new List<TournamentPlayerStatsDTO>();
            foreach (var team in teams)
            {
                var players = await uow.PlayerRepository
                    .Get(player => player.TeamId == team.TeamId) // Filtro por TeamId
                    .Include(player => player.User)
                    .Include(player => player.PlayerStats)
                    .ToListAsync();

                foreach (var player in players)
                {
                    playerStats.Add(new TournamentPlayerStatsDTO
                    {
                        PlayerId = player.PlayerId,
                        FirstName = player.User.FirstName,
                        LastName = player.User.LastName,
                        TeamName = team.TeamName,
                        Goals = player.PlayerStats.Goals,
                        YellowCards = player.PlayerStats.YellowCards,
                        RedCards = player.PlayerStats.RedCards
                    });
                }
            }

            return playerStats;
        }


        //metodo para donde obtendras todos los equipos del toirnamentid que se apsara como parametro y generara partidos doned cada equipo entre si solo se enfrentara una vez y se generaran los partidos con los equipos que se enfrentaran, cada cuando el team1id sea el mismo stage aumentara en 1, si hay uno nuevo empieza en 1
        public async Task CreateLeagueMatches(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener los equipos asociados al torneo
            var teams = await uow.TeamRepository
                .Get(team => team.TournamentId == tournamentId)
                .ToListAsync();

            if (teams.Count % 2 != 0)
            {
                // Si el número de equipos es impar, añadir un "equipo fantasma"
                teams.Add(new Team { TeamId = -1 }); // Equipo ficticio
            }

            int totalRounds = teams.Count - 1; // Número total de rondas
            int matchesPerRound = teams.Count / 2; // Número de partidos por ronda

            var matches = new List<Match>();

            // Algoritmo de round-robin
            for (int round = 0; round < totalRounds; round++)
            {
                for (int match = 0; match < matchesPerRound; match++)
                {
                    int team1Index = match;
                    int team2Index = teams.Count - 1 - match;

                    var team1 = teams[team1Index];
                    var team2 = teams[team2Index];

                    // Ignorar el equipo fantasma
                    if (team1.TeamId != -1 && team2.TeamId != -1)
                    {
                        matches.Add(new Match
                        {
                            TournamentId = tournamentId,
                            Team1Id = team1.TeamId,
                            Team2Id = team2.TeamId,
                            Team1Goals = 0,
                            Team2Goals = 0,
                            Date = null,
                            IsFinished = false,
                            Stage = round + 1 // Jornada actual
                        });
                    }
                }

                // Rotar los equipos (mantener el primer equipo fijo)
                var lastTeam = teams[^1];
                teams.RemoveAt(teams.Count - 1);
                teams.Insert(1, lastTeam);
            }

            // Insertar los partidos en la base de datos
            uow.MatchRepository.InsertRange(matches);
            await uow.SaveAsync();
        }



        //getleaguematches onde obtendras el stage tambien de cada partido utlizlnao matchleagueinfodto
        public async Task<List<MatchLeagueInfoDTO>> GetLeagueMatches(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener los partidos asociados al torneo
            var matches = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId)
                .ToListAsync();

            // Obtener la información de los partidos
            var matchesInfo = new List<MatchLeagueInfoDTO>();
            foreach (var match in matches)
            {
                var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
                var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

                matchesInfo.Add(new MatchLeagueInfoDTO
                {
                    MatchId = match.MatchId,
                    Team1Name = team1.TeamName,
                    Team2Name = team2.TeamName,
                    Stage = match.Stage
                });
            }

            return matchesInfo;
        }



        //getleaguematchresults recibiedo el tournamentid y obtendras los resultados de los match que isfinished sea 1 obtenieno meiante matchleagueresultdto
        public async Task<List<MatchLeagueResultDTO>> GetLeagueMatchResults(int tournamentId)
        {
            // Verificar si el torneo existe
            var tournamentExists = await uow.TournamentRepository
                .Get(t => t.TournamentId == tournamentId)
                .AnyAsync();

            if (!tournamentExists)
                throw new ArgumentException("Torneo no encontrado");

            // Obtener los partidos que tengan el del modelo match la propiedadd IsFinished con valor true
            var matches = await uow.MatchRepository
                .Get(m => m.TournamentId == tournamentId && m.IsFinished == true)
                .ToListAsync();

            // Obtener los resultados de los partidos
            var results = new List<MatchLeagueResultDTO>();
            foreach (var match in matches)
            {
                var team1 = await uow.TeamRepository.Get(t => t.TeamId == match.Team1Id).FirstOrDefaultAsync();
                var team2 = await uow.TeamRepository.Get(t => t.TeamId == match.Team2Id).FirstOrDefaultAsync();

                results.Add(new MatchLeagueResultDTO
                {
                    MatchId = match.MatchId,
                    Team1Name = team1.TeamName,
                    Team1Goals = match.Team1Goals,
                    Team2Name = team2.TeamName,
                    Team2Goals = match.Team2Goals,
                    Stage = match.Stage
                });
            }

            return results;
        }



        //metodo para actualizar la date del matchid UTILIZANO updatematchdatedto
        public async Task UpdateMatchDate(UpdateMatchDateDTO updateMatchDateDTO)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == updateMatchDateDTO.MatchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Actualiza la fecha del partido
            match.Date = updateMatchDateDTO.NewDate;

            // Actualiza el partido en la base de datos
            uow.MatchRepository.Update(match);
            await uow.SaveAsync();
        }


        //metodo para obtener el la date del matchid UTILIZANO updatematchdatedto
        public async Task<DateTime?> GetMatchDate(int matchId)
        {
            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción o devuelve null
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Devuelve la fecha del partido
            return match.Date;
        }
































        //public async Task<List<PlayerStatsDTO>> GetPlayerStatsByTournamentId(int tournamentId)
        //{
        //    // Verificar si el torneo existe
        //    var tournamentExists = await uow.TournamentRepository
        //        .Get(t => t.TournamentId == tournamentId)
        //        .AnyAsync();

        //    if (!tournamentExists)
        //        throw new ArgumentException("Torneo no encontrado");

        //    // Obtener los equipos asociados al torneo
        //    var teams = await uow.TeamRepository
        //        .Get(team => team.TournamentId == tournamentId) // Filtro por TournamentId
        //        .ToListAsync();

        //    // Obtener las estadísticas de los jugadores de los equipos
        //    var playerStats = new List<PlayerStatsDTO>();
        //    foreach (var team in teams)
        //    {
        //        var players = await uow.PlayerRepository
        //            .Get(player => player.TeamId == team.TeamId) // Filtro por TeamId
        //            .Include(player => player.User)
        //            .Include(player => player.PlayerStats)
        //            .ToListAsync();

        //        foreach (var player in players)
        //        {
        //            playerStats.Add(new PlayerStatsDTO
        //            {
        //                PlayerId = player.PlayerId,
        //                FirstName = player.User.FirstName,
        //                LastName = player.User.LastName,
        //                Goals = player.PlayerStats.Goals,
        //                YellowCards = player.PlayerStats.YellowCards,
        //                RedCards = player.PlayerStats.RedCards
        //            });
        //        }
        //    }

        //    return playerStats;
        //}






    }
}

