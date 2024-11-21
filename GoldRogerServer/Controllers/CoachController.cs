using Microsoft.AspNetCore.Mvc;
using GoldRogerServer.Business;
using GoldRogerServer.Utils;
using GoldRogerServer.DTOs.Coach;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using GoldRogerServer.Helpers;
using GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Tournament;


namespace GoldRogerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoachController : ControllerBase
    {
        private readonly CoachBusiness _coachBusiness;

        public CoachController(CoachBusiness coachBusiness)
        {
            _coachBusiness = coachBusiness;
        }


        [HttpGet("GetCoachLicenseNumberById")]
        public async Task<IActionResult> GetCoachLicenseNumberById(int coachId)
        {
            var response = new APIResponse<string?> { Success = true };

            try
            {
                response.Data = await _coachBusiness.GetCoachLicenseNumberById(coachId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("CreateTeam")]
        [Authorize]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequestDTO requestDTO)
        {
            try
            {
                // Obtener el UserId del usuario logueado
                int coachId = SessionHelper.GetTokenUserId(User);
                if (coachId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Crear el equipo
                var team = await _coachBusiness.CreateTeam(requestDTO, coachId);

                return Ok(new { Success = true, Data = team });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetTeamNames")]
        [Authorize]
        public async Task<IActionResult> GetTeamNames()
        {
            try
            {
                // Obtener el UserId del usuario logueado
                int coachId = SessionHelper.GetTokenUserId(User);
                if (coachId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Obtener los nombres de los equipos asociados al CoachId
                var teamNames = await _coachBusiness.GetTeamNamesByCoachId(coachId);

                return Ok(new { Success = true, Data = teamNames });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("UpdateTeamNames")]
        [Authorize]
        public async Task<IActionResult> UpdateTeamNames([FromBody] string newTeamName)
        {
            try
            {
                // Obtener el CoachId del usuario logueado
                int coachId = SessionHelper.GetTokenUserId(User);
                if (coachId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Validar el nuevo nombre de equipo
                if (string.IsNullOrWhiteSpace(newTeamName) || newTeamName.Length <= 3)
                    return BadRequest(new { Success = false, Message = "Nombre de equipo inválido." });

                // Actualizar los nombres de los equipos asociados al CoachId
                await _coachBusiness.UpdateTeamNameByCoachId(coachId, newTeamName);

                return Ok(new { Success = true, Message = "Nombre de equipo actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
        [HttpDelete("DeleteAllTeams")]
        [Authorize]
        public async Task<IActionResult> DeleteAllTeams()
        {
            try
            {
                // Obtener el CoachId del usuario logueado
                int coachId = SessionHelper.GetTokenUserId(User);
                if (coachId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Llamar al método del negocio para eliminar los equipos
                await _coachBusiness.DeleteTeamsByCoachId(coachId);

                return Ok(new { Success = true, Message = "Todos los equipos asociados al entrenador han sido eliminados correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetAllTournaments")]
        public async Task<IActionResult> GetAllTournaments()
        {
            var response = new APIResponse<List<TournamentDTO>> { Success = true };

            try
            {
                // Llamar al método que obtiene todos los torneos
                response.Data = await _coachBusiness.GetAllTournaments();
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response); // En caso de error, devolver respuesta BadRequest
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error inesperado: " + ex.Message;
                return StatusCode(500, response); // En caso de error inesperado, devolver código de error 500
            }

            return Ok(response); // Si todo sale bien, devolver los torneos con código 200
        }

    }
}
