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
    }
}
