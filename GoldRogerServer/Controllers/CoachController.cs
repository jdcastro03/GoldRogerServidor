using Microsoft.AspNetCore.Mvc;
using GoldRogerServer.Business;
using GoldRogerServer.Utils;
using GoldRogerServer.DTOs.Coach;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using GoldRogerServer.Helpers;


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

    }
}
