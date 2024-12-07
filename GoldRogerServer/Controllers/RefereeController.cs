using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GoldRogerServer.Business;
using GoldRogerServer.Utils;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Referee;
using GoldRoger.Entity.Entities;
namespace GoldRogerServer.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class RefereeController : ControllerBase
    {
        private readonly RefereeBusiness _refereeBusiness;

        public RefereeController(RefereeBusiness refereeBusiness)
        {
            _refereeBusiness = refereeBusiness;
        }

        [HttpGet("GetRefereeLicenseNumberById")]
        public async Task<IActionResult> GetRefereeLicenseNumberById(int refereeId)
        {
            var response = new APIResponse<string?> { Success = true };

            try
            {
                response.Data = await _refereeBusiness.GetLicenseNumberById(refereeId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        //getallreferees
        [HttpGet("GetAllReferees")]
        public async Task<IActionResult> GetAllReferees()
        {
            var response = new APIResponse<List<RefereeDTO>> { Success = true };

            try
            {
                response.Data = await _refereeBusiness.GetAllReferees();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }


        //insertmatchrefere
        [HttpPost("InsertMatchReferee")]
        public async Task<IActionResult> InsertMatchReferee([FromBody] InsertRefereeDTO insertRefereeDTO)
        {
            var response = new APIResponse<MatchReferee> { Success = true };

            try
            {
                // Llamamos a la lógica del negocio para insertar la relación entre el partido y el árbitro
                await _refereeBusiness.InsertMatchReferee(insertRefereeDTO);

                // Devolvemos los datos insertados
                response.Data = new MatchReferee
                {
                    MatchId = insertRefereeDTO.MatchId,
                    RefereeId = insertRefereeDTO.RefereeId
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        //updatematchreferee tal cual como el de refereebusiness

        [HttpPut("UpdateMatchReferee/{matchId}")]
        public async Task<IActionResult> UpdateMatchReferee(int matchId, [FromBody] int newRefereeId)
        {
            try
            {
                // Llamamos al servicio para realizar la actualización
                await _refereeBusiness.UpdateMatchReferee(matchId, newRefereeId);

                // Si la actualización es exitosa, devolvemos un 200 OK
                return Ok(new { message = "Árbitro actualizado con éxito." });
            }
            catch (ArgumentException ex)
            {
                // Si ocurre un error en la actualización, devolvemos un error 400 con el mensaje de la excepción
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Si ocurre algún otro error, devolvemos un error 500
                return StatusCode(500, new { message = "Ocurrió un error interno.", details = ex.Message });
            }
        }




        //getrefereebymatchid
        [HttpGet("GetRefereeByMatchId")]
        public async Task<IActionResult> GetRefereeByMatchId(int matchId)
        {
            var response = new APIResponse<List<RefereeDTO>> { Success = true }; // Cambié RefereeDTO a List<RefereeDTO>

            try
            {
                response.Data = await _refereeBusiness.GetRefereeByMatchId(matchId);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
