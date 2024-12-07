using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GoldRogerServer.Business;
using GoldRogerServer.Utils;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Organizer;
using GoldRogerServer.DTOs.Match;
using GoldRogerServer.Helpers;
using Microsoft.AspNetCore.Authorization;
using GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Player;

namespace GoldRogerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrganizerController : ControllerBase
    {
        private readonly OrganizerBusiness _organizerBusiness;

        public OrganizerController(OrganizerBusiness organizerBusiness)
        {
            _organizerBusiness = organizerBusiness;
        }

        [HttpGet("GetOrganizerNameById")]
        public async Task<IActionResult> GetOrganizerNameById(int organizerId)
        {
            var response = new APIResponse<string?> { Success = true };

            try
            {
                response.Data = await _organizerBusiness.GetOrganizerNameById(organizerId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("CreateTournament")]
        [Authorize]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentRequestDTO requestDTO)
        {
            try
            {
                // Obtener el UserId del usuario logueado
                int organizerId = SessionHelper.GetTokenUserId(User);
                if (organizerId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Crear el torneo
                var tournament = await _organizerBusiness.CreateTournament(requestDTO, organizerId);

                return Ok(new { Success = true, Data = tournament });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
        [HttpGet("GetTournamentsByOrganizerId")]
        public async Task<IActionResult> GetTournamentsByOrganizerId(int organizerId)
        {
            var response = new APIResponse<List<TournamentInfoDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene los torneos por organizerId
                response.Data = await _organizerBusiness.GetTournamentsByOrganizerId(organizerId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener los torneos.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //get tournamentNameById
        [HttpGet("GetTournamentNameById")]
        public async Task<IActionResult> GetTournamentNameById(int tournamentId)
        {
            var response = new APIResponse<string?> { Success = true };

            try
            {
                response.Data = await _organizerBusiness.GetTournamentNameById(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        //get tournamentInfoById
        [HttpGet("GetTournamentInfoById")]
        public async Task<IActionResult> GetTournamentInfoById(int tournamentId)
        {
            var response = new APIResponse<GlobalTournamentInfoDTO> { Success = true };

            try
            {
                response.Data = await _organizerBusiness.GetTournamentInfoById(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }


        //getteamtournamentsbyid

        [HttpGet("GetTeamsByTournamentId")]
        public async Task<IActionResult> GetTeamsByTournamentId(int tournamentId)
        {
            var response = new APIResponse<List<TeamInfoDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene los equipos por TournamentId
                response.Data = await _organizerBusiness.GetTeamsByTournamentId(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener los equipos del torneo.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //ELIMINATORIA
        //createRandomMatchesTournament2

        [HttpPost("CreateQuarterFinals")]
        public async Task<IActionResult> CreateQuarterFinals(int tournamentId)
        {

            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio que crea los partidos aleatorios
                await _organizerBusiness.CreateQuarterFinals(tournamentId);

                // Si llegamos aquí sin excepciones, significa que todo salió bien
                response.Data = true; // Puedes indicar que la operación fue exitosa.
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al crear los partidos aleatorios.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getquarterfinalsmatches

        [HttpGet("GetQuarterFinalsMatches")]
        public async Task<IActionResult> GetQuarterFinalsMatches(int tournamentId)
        {
            var response = new APIResponse<List<MatchInfoDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene los partidos de cuartos de final
                response.Data = await _organizerBusiness.GetQuarterFinalsMatches(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener los partidos de cuartos de final.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //createnextRoundMatches

        [HttpPost("CreateSemiFinals")]
        public async Task<IActionResult> CreateSemiFinals(int tournamentId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio que crea los partidos de la siguiente ronda
                await _organizerBusiness.CreateSemiFinals(tournamentId);

                // Si llegamos aquí sin excepciones, significa que todo salió bien
                response.Data = true; // Puedes indicar que la operación fue exitosa.
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al crear los partidos de la siguiente ronda.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getsemifinalsmatches

        [HttpGet("GetSemiFinalsMatches")]
        public async Task<IActionResult> GetSemiFinalsMatches(int tournamentId)
        {
            var response = new APIResponse<List<MatchInfoDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene los partidos de semifinales
                response.Data = await _organizerBusiness.GetSemiFinalsMatches(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener los partidos de semifinales.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //createfinalmatch

        [HttpPost("CreateFinal")]
        public async Task<IActionResult> CreateFinal(int tournamentId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio que crea el partido final
                await _organizerBusiness.CreateFinal(tournamentId);

                // Si llegamos aquí sin excepciones, significa que todo salió bien
                response.Data = true; // Puedes indicar que la operación fue exitosa.
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al crear el partido final.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        //getfinalmatch

        [HttpGet("GetFinalMatch")]
        public async Task<IActionResult> GetFinalMatch(int tournamentId)
        {
            var response = new APIResponse<MatchInfoDTO> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene el partido final
                response.Data = await _organizerBusiness.GetFinalMatch(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener el partido final.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        //getquarterfinalsresults

        [HttpGet("GetQuarterFinalsResults")]
        public async Task<IActionResult> GetQuarterFinalsResults(int tournamentId)
        {
            var response = new APIResponse<List<MatchResultDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene los resultados de cuartos de final
                response.Data = await _organizerBusiness.GetQuarterFinalsResults(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener los resultados de cuartos de final.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getsemifinalsresults

        [HttpGet("GetSemiFinalsResults")]
        public async Task<IActionResult> GetSemiFinalsResults(int tournamentId)
        {
            var response = new APIResponse<List<MatchResultDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene los resultados de semifinales
                response.Data = await _organizerBusiness.GetSemiFinalsResults(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener los resultados de semifinales.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getfinalresult

        [HttpGet("GetFinalResult")]
        public async Task<IActionResult> GetFinalResult(int tournamentId)
        {
            var response = new APIResponse<MatchResultDTO> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene el resultado de la final
                response.Data = await _organizerBusiness.GetFinalResult(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener el resultado de la final.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        //getplayerstatsbytournamentid utilizaremos tournamentplayerstatsdto

        [HttpGet("GetPlayerStatsByTournamentId")]
        public async Task<IActionResult> GetPlayerStatsByTournamentId(int tournamentId)
        {
            var response = new APIResponse<List<TournamentPlayerStatsDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene las estadísticas de los jugadores por TournamentId
                response.Data = await _organizerBusiness.GetPlayerStatsByTournamentId(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener las estadísticas de los jugadores del torneo.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //createleaguematches

        [HttpPost("CreateLeagueMatches")]
        public async Task<IActionResult> CreateLeagueMatches(int tournamentId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio que crea los partidos de la liga
                await _organizerBusiness.CreateLeagueMatches(tournamentId);

                // Si llegamos aquí sin excepciones, significa que todo salió bien
                response.Data = true; // Puedes indicar que la operación fue exitosa.
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al crear los partidos de la liga.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getleaguematches

        [HttpGet("GetLeagueMatches")]
        public async Task<IActionResult> GetLeagueMatches(int tournamentId)
        {
            var response = new APIResponse<List<MatchLeagueInfoDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene los partidos de la liga
                response.Data = await _organizerBusiness.GetLeagueMatches(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener los partidos de la liga.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getleaguematchresults

        [HttpGet("GetLeagueMatchResults")]
        public async Task<IActionResult> GetLeagueMatchResults(int tournamentId)
        {
            var response = new APIResponse<List<MatchLeagueResultDTO>> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene los resultados de los partidos de la liga
                response.Data = await _organizerBusiness.GetLeagueMatchResults(tournamentId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener los resultados de los partidos de la liga.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //updatematchdate   

        [HttpPost("UpdateMatchDate")]
        public async Task<IActionResult> UpdateMatchDate([FromBody] UpdateMatchDateDTO updateMatchDateDTO)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio que actualiza la fecha de un partido
                await _organizerBusiness.UpdateMatchDate(updateMatchDateDTO);

                // Si llegamos aquí sin excepciones, significa que todo salió bien
                response.Data = true; // Puedes indicar que la operación fue exitosa.
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al actualizar la fecha del partido.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getmatchdate

        [HttpGet("GetMatchDate")]
        public async Task<IActionResult> GetMatchDate(int matchId)
        {
            var response = new APIResponse<DateTime?> { Success = true };

            try
            {
                // Llamamos al método de negocio que obtiene la fecha de un partido
                response.Data = await _organizerBusiness.GetMatchDate(matchId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al obtener la fecha del partido.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }

    }

}
