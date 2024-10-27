using GoldRoger.Entity.Entities.Security;
using GoldRogerServer.Business.Security;
using GoldRogerServer.DTOs.Security;
using GoldRogerServer.Helpers; // Cambiado a Helpers para SessionHelper
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using GoldRogerServer.Utils;

namespace GoldRogerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly SecurityBusiness _securityBusiness;

        public SecurityController(SecurityBusiness securityBusiness)
        {
            _securityBusiness = securityBusiness;
        }

        [KitAuthorize]
        [PermissionChecker("PERMISSION_READ")]
        [HttpGet("GetAllPermissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var response = new APIResponse<IEnumerable<Permission>> { Success = true };
            response.Data = await _securityBusiness.GetPermissions();
            return Ok(response);
        }

        [KitAuthorize]
        [PermissionChecker("PERMISSION_CREATE")]
        [HttpPost("AddPermission")]
        public async Task<IActionResult> AddPermission([FromBody] AddPermissionRequest addPRequest)
        {
            var response = new APIResponse<Permission> { Success = true };
            response.Data = await _securityBusiness.Add(addPRequest, SessionHelper.GetTokenUserId(User));
            return Ok(response);
        }

        [KitAuthorize]
        [PermissionChecker("PERMISSION_CREATE")]
        [HttpPost("AddMultiplePermissions")]
        public async Task<IActionResult> AddMultiplePermissions([FromBody] List<AddPermissionRequest> addPRequests)
        {
            var response = new APIResponse<List<Permission>> { Success = true };
            response.Data = await _securityBusiness.AddMultiple(addPRequests, SessionHelper.GetTokenUserId(User));
            return Ok(response);
        }

        [KitAuthorize]
        [PermissionChecker("PERMISSION_EDIT")]
        [HttpPost("UpdatePermission")]
        public async Task<IActionResult> UpdatePermission([FromBody] UpdatePermissionRequest updatePRequest)
        {
            var response = new APIResponse<Permission> { Success = true };
            response.Data = await _securityBusiness.Update(updatePRequest, SessionHelper.GetTokenUserId(User));
            return Ok(response);
        }

        [KitAuthorize]
        [PermissionChecker("PERMISSION_DELETE")]
        [HttpPost("DeletePermission")]
        public async Task<IActionResult> DeletePermission([FromBody] DeletePermissionRequest deletePRequest)
        {
            var response = new APIResponse<Permission> { Success = true };
            response.Data = await _securityBusiness.Delete(deletePRequest, SessionHelper.GetTokenUserId(User));
            return Ok(response);
        }

        [KitAuthorize]
        [PermissionChecker("PERMISSION_READ")]
        [HttpPost("CheckPermission")]
        public async Task<IActionResult> CheckPermission([FromBody] CheckPermissionRequest checkPermissionRequest)
        {
            try
            {
                var userPermissionKeys = await _securityBusiness.GetUserPermissionKeys(checkPermissionRequest);
                return Ok(userPermissionKeys);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error al verificar el permiso.");
            }
        }

        [KitAuthorize]
        [HttpGet("GetUserPermissions")]
        public async Task<IActionResult> GetUserPermissions()
        {
            var response = new APIResponse<IEnumerable<Permission>> { Success = true };
            response.Data = await _securityBusiness.GetUserPermissions(SessionHelper.GetTokenUserId(User));
            return Ok(response);
        }
    }
}