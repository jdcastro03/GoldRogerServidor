
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GoldRogerServer.Business;
using GoldRogerServer.Utils;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.Player;
using GoldRogerServer.DTOs.Tournament;
using GoldRogerServer.Helpers;
using Microsoft.AspNetCore.Authorization;



namespace GoldRogerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class PlayerController : ControllerBase
    {
        private readonly PlayerBusiness _playerBusiness;

        public PlayerController(PlayerBusiness playerBusiness)
        {
            _playerBusiness = playerBusiness;
        }

        [HttpGet("GetPlayerPositionById")]
        public async Task<IActionResult> GetPlayerPositionById(int playerId)
        {
            var response = new APIResponse<string?> { Success = true };

            try
            {
                response.Data = await _playerBusiness.GetPlayerPositionById(playerId);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("GetAllTeams")]
        public async Task<IActionResult> GetAllTeams()
        {
            var response = new APIResponse<List<TeamDTO>> { Success = true };

            try
            {
                // Llamar al método que obtiene todos los equipos
                response.Data = await _playerBusiness.GetAllTeams();
            }
            catch (ArgumentException ex)
            {
                // Manejar errores específicos, como cuando no hay equipos
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response); // Respuesta con código 400 (BadRequest)
            }
            catch (Exception ex)
            {
                // Manejar errores no controlados
                response.Success = false;
                response.Message = "Ocurrió un error inesperado: " + ex.Message;
                return StatusCode(500, response); // Respuesta con código 500 (Error interno del servidor)
            }

            // Devolver respuesta exitosa con datos
            return Ok(response); // Respuesta con código 200 (OK)
        }
        [HttpPut("UpdatePlayerTeam/{teamId}")]
        [Authorize]
        public async Task<IActionResult> UpdatePlayerTeam(int teamId)
        {
            try
            {
                // Obtener el UserId del usuario logueado (playerId)
                int playerId = SessionHelper.GetTokenUserId(User);
                if (playerId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Llamar al servicio de negocio para actualizar el equipo del jugador
                await _playerBusiness.UpdatePlayerTeam(playerId, teamId);

                return Ok(new { Success = true, Message = "Equipo actualizado con éxito" });
            }
            catch (ArgumentException ex)
            {
                // Manejar errores como jugador o equipo no encontrados
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        //controller de getplayer teamid
        [HttpGet("GetPlayerTeamId")]
        [Authorize]
        public async Task<IActionResult> GetPlayerTeamId()
        {
            var response = new APIResponse<int?> { Success = true };

            try
            {
                // Obtener el UserId del usuario logueado (playerId)
                int playerId = SessionHelper.GetTokenUserId(User);
                if (playerId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Llamar al servicio de negocio para obtener el equipo del jugador
                response.Data = await _playerBusiness.GetPlayerTeamId(playerId);
            }
            catch (ArgumentException ex)
            {
                // Manejar errores como jugador no encontrado
                response.Success = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //controller de getplayer teamname
        [HttpGet("GetPlayerTeamName")]
        [Authorize]
        public async Task<IActionResult> GetPlayerTeamName()
        {
            var response = new APIResponse<string?> { Success = true };

            try
            {
                // Obtener el UserId del usuario logueado (playerId)
                int playerId = SessionHelper.GetTokenUserId(User);
                if (playerId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Llamar al servicio de negocio para obtener el nombre del equipo del jugador
                response.Data = await _playerBusiness.GetPlayerTeamName(playerId);
            }
            catch (ArgumentException ex)
            {
                // Manejar errores como jugador no encontrado
                response.Success = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //controller de getcoachname
        [HttpGet("GetCoachName")]
        [Authorize]
        public async Task<IActionResult> GetTeamName()
        {
            var response = new APIResponse<string?> { Success = true };

            try
            {
                // Obtener el UserId del usuario logueado (playerId)
                int playerId = SessionHelper.GetTokenUserId(User);
                if (playerId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Llamar al servicio de negocio para obtener el nombre del entrenador del jugador
                response.Data = await _playerBusiness.GetCoachName(playerId);
            }
            catch (ArgumentException ex)
            {
                // Manejar errores como jugador no encontrado
                response.Success = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        //getplayersbyteamname
        [HttpGet("GetPlayersByTeamName")]
        [Authorize]
        public async Task<IActionResult> GetPlayersByTeamName(string teamName)
        {
            var response = new APIResponse<List<PlayerDTO>> { Success = true };

            try
            {
                // Llamar al servicio de negocio para obtener los jugadores del equipo
                response.Data = await _playerBusiness.GetPlayersByTeamName(teamName);
            }
            catch (ArgumentException ex)
            {
                // Manejar errores como equipo no encontrado
                response.Success = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getplayerstats 
        [HttpGet("GetPlayerStats")]
        [Authorize]
        public async Task<IActionResult> GetPlayerStats()
        {
            var response = new APIResponse<PlayerStatsDTO> { Success = true };

            try
            {
                // Obtener el UserId del usuario logueado (playerId)
                int playerId = SessionHelper.GetTokenUserId(User);
                if (playerId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Llamar al servicio de negocio para obtener las estadísticas del jugador
                response.Data = await _playerBusiness.GetPlayerStats(playerId);
            }
            catch (ArgumentException ex)
            {
                // Manejar errores como jugador no encontrado
                response.Success = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //getplayerTournament
        [HttpGet("GetPlayerTournament")]
        [Authorize]
        public async Task<IActionResult> GetPlayerTournament()
        {
            var response = new APIResponse<TournamentDTO> { Success = true };

            try
            {
                // Obtener el UserId del usuario logueado (playerId)
                int playerId = SessionHelper.GetTokenUserId(User);
                if (playerId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Llamar al servicio de negocio para obtener el torneo del jugador
                response.Data = await _playerBusiness.GetPlayerTournament(playerId);
            }
            catch (ArgumentException ex)
            {
                // Manejar errores como jugador no encontrado
                response.Success = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //removeplayerfromteam
        [HttpDelete("RemovePlayerFromTeam")]
        [Authorize]
        public async Task<IActionResult> RemovePlayerFromTeam()
        {
            try
            {
                // Obtener el UserId del usuario logueado (playerId)
                int playerId = SessionHelper.GetTokenUserId(User);
                if (playerId == 0)
                    return Unauthorized("Usuario no autorizado");

                // Llamar al servicio de negocio para remover al jugador del equipo
                await _playerBusiness.RemovePlayerFromTeam(playerId);

                return Ok(new { Success = true, Message = "Jugador removido del equipo con éxito" });
            }
            catch (ArgumentException ex)
            {
                // Manejar errores como jugador no encontrado
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}



