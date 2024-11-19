
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GoldRogerServer.Business;
using GoldRogerServer.Utils;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;



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
    }
}



