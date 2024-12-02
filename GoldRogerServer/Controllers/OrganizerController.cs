using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GoldRogerServer.Business;
using GoldRogerServer.Utils;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Organizer;
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

    }

}
