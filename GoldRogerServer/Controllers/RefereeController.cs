using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GoldRogerServer.Business;
using GoldRogerServer.Utils;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Referee;
using GoldRogerServer.Helpers;
using System.Collections.Generic;

using GoldRoger.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

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


        //getmatchbyrefereeid usa authorize oara obtener el id del referee logeado
        [HttpGet("GetMatchesByRefereeId")]
        [Authorize]
        public async Task<IActionResult> GetMatchByRefereeId()
        {
            var response = new APIResponse<List<MatchRefereeDTO>> { Success = true };

            try
            {
                // Obtenemos el id del referee logeado
                int refereeId = SessionHelper.GetTokenUserId(User);
                if (refereeId == 0)
                    return Unauthorized(new { Success = false, Message = "Usuario no autorizado" });

                // Llamamos al servicio de negocio para obtener los partidos del referee
                response.Data = await _refereeBusiness.GetMatchesByRefereeId(refereeId);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }


        //getmatchbymatchid

        [HttpGet("GetMatchByMatchId")]
        public async Task<IActionResult> GetMatchByMatchId(int matchId)
        {
            var response = new APIResponse<MatchRefereeDTO> { Success = true };

            try
            {
                response.Data = await _refereeBusiness.GetMatchByMatchId(matchId);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }
        
        //getplayersbymatchid
        [HttpGet("GetPlayersByMatchId")]
        public async Task<IActionResult> GetPlayersByMatchId(int matchId)
        {
            var response = new APIResponse<List<MatchTeamDTO>> { Success = true };

            try
            {
                response.Data = await _refereeBusiness.GetPlayers1ByMatchId(matchId);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }



        //getplayers2bymatchid
        [HttpGet("GetPlayers2ByMatchId")]
        public async Task<IActionResult> GetPlayers2ByMatchId(int matchId)
        {
            var response = new APIResponse<List<MatchTeamDTO>> { Success = true };

            try
            {
                response.Data = await _refereeBusiness.GetPlayers2ByMatchId(matchId);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        //addgoal sin r
        [HttpPost("AddGoal")]
        public async Task<IActionResult> AddGoal(int playerId, int matchId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio para agregar un gol
                await _refereeBusiness.AddGoal(playerId, matchId);
                response.Message = "Gol añadido con éxito.";
                response.Data = true;
            }
            catch (ArgumentException ex)
            {
                // Capturamos errores específicos de negocio
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                // Capturamos errores generales
                response.Success = false;
                response.Message = "Ocurrió un error inesperado.";
                response.Data = false;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //ismatchactive
        [HttpGet("IsMatchActive")]
        public async Task<IActionResult> IsMatchActive(int matchId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                response.Data = await _refereeBusiness.IsMatchActive(matchId);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }


        //getgoalsbymatchid
        [HttpGet("GetGoalsByMatchId")]
        public async Task<IActionResult> GetGoalsByMatchId(int matchId)
        {
            var response = new APIResponse<MatchGoalsDTO> { Success = true };

            try
            {
                response.Data = await _refereeBusiness.GetGoalsByMatchId(matchId);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        //startmatch
        [HttpPost("StartMatch")]
        public async Task<IActionResult> StartMatch(int matchId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio para iniciar el partido
                await _refereeBusiness.StartMatch(matchId);
                response.Message = "Partido iniciado con éxito.";
                response.Data = true;
            }
            catch (ArgumentException ex)
            {
                // Capturamos errores específicos de negocio
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                // Capturamos errores generales
                response.Success = false;
                response.Message = "Ocurrió un error inesperado.";
                response.Data = false;
                return StatusCode(500, response);
            }

            return Ok(response);
        }



        //endmatch
        [HttpPost("EndMatch")]
        public async Task<IActionResult> EndMatch(int matchId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio para finalizar el partido
                await _refereeBusiness.EndMatch(matchId);
                response.Message = "Partido finalizado con éxito.";
                response.Data = true;
            }
            catch (ArgumentException ex)
            {
                // Capturamos errores específicos de negocio
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                // Capturamos errores generales
                response.Success = false;
                response.Message = "Ocurrió un error inesperado.";
                response.Data = false;
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        //Getactivematches
        [HttpGet("GetActiveMatches")]
        public async Task<IActionResult> GetActiveMatches()
        {
            var response = new APIResponse<List<MatchHomeDTO>> { Success = true };

            try
            {
                response.Data = await _refereeBusiness.GetActiveMatches();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        //addyellowcard
        [HttpPost("AddYellowCard")]
        public async Task<IActionResult> AddYellowCard(int playerId, int matchId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio para agregar una tarjeta amarilla
                await _refereeBusiness.AddYellowCard(playerId, matchId);
                response.Message = "Tarjeta amarilla añadida con éxito.";
                response.Data = true;
            }
            catch (ArgumentException ex)
            {
                // Capturamos errores específicos de negocio
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                // Capturamos errores generales
                response.Success = false;
                response.Message = "Ocurrió un error inesperado.";
                response.Data = false;
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        //addredcard
        [HttpPost("AddRedCard")]
        public async Task<IActionResult> AddRedCard(int playerId, int matchId)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                // Llamamos al método de negocio para agregar una tarjeta roja
                await _refereeBusiness.AddRedCard(playerId, matchId);
                response.Message = "Tarjeta roja añadida con éxito.";
                response.Data = true;
            }
            catch (ArgumentException ex)
            {
                // Capturamos errores específicos de negocio
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                // Capturamos errores generales
                response.Success = false;
                response.Message = "Ocurrió un error inesperado.";
                response.Data = false;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

    }
}
