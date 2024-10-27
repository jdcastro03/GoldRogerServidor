using Microsoft.AspNetCore.Mvc;
using GoldRogerServer.Business;
using GoldRogerServer.DTOs;
using GoldRoger.Entity.Entities;
using GoldRogerServer.Utils;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRogerServer.Business;
using GoldRogerServer.DTOs.User;
using GoldRogerServer.Utils;

namespace GoldRogerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserBusiness _userBusiness;

        public UserController(UserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }


        // Endpoint para crear un nuevo usuario usando un DTO
        //[KitAuthorize]
        //[PermissionChecker("USER_CREATE")]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(CreateUserRequestDTO requestDTO)
        {
            var response = new APIResponse<User> { Success = true };

            try
            {
                // Verificación adicional basada en el tipo de usuario
                if (requestDTO.UserType == 1) // Asumiendo que 1 es el tipo "Jugador"
                {
                    // Valida que 'Position' esté presente y no sea vacío o nulo
                    if (string.IsNullOrWhiteSpace(requestDTO.Position))
                    {
                        response.Success = false;
                        response.Message = "El campo 'Position' es obligatorio para usuarios del tipo 'Jugador'.";
                        return BadRequest(response);
                    }
                }

                // Llamada a la lógica de creación de usuario en UserBusiness
                response.Data = await _userBusiness.CreateUser(requestDTO);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        // Endpoint para actualizar un usuario usando un DTO
        [KitAuthorize]
        [PermissionChecker("USER_UPDATE")]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequestDTO requestDTO)
        {
            var response = new APIResponse<User> { Success = true };

            try
            {
                response.Data = await _userBusiness.UpdateUser(id, requestDTO);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        // Endpoint para eliminar un usuario por ID
        [KitAuthorize]
        [PermissionChecker("USER_DELETE")]
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = new APIResponse<bool> { Success = true };

            try
            {
                await _userBusiness.DeleteUser(id);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
        }

        // Endpoint para obtener todos los usuarios
       
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var response = new APIResponse<IEnumerable<User>> { Success = true };
            try
            {
                response.Data = await _userBusiness.GetAllUsers();
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

    }
}

// Endpoint para obtener todos los usuarios
//[HttpGet("GetUsers")]
//public async Task<IActionResult> GetUsers()
//{
//    var users = await _userBusiness.GetAllUsers();
//    return Ok(users);
//}

//// Endpoint para crear un nuevo usuario
//[HttpPost("CreateUser")]
//public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDTO requestDTO)
//{
//    try
//    {
//        var newUser = await _userBusiness.CreateUser(requestDTO);
//        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
//    }
//    catch (ArgumentException ex)
//    {
//        return BadRequest(ex.Message);
//    }
//}

//// Endpoint para eliminar un usuario por ID
//[HttpDelete("DeleteUser")]
//public async Task<IActionResult> DeleteUser(int id)
//{
//    try
//    {
//        await _userBusiness.DeleteUser(id);
//        return NoContent();
//    }
//    catch (ArgumentException ex)
//    {
//        return NotFound(ex.Message);
//    }
//}

//// Endpoint para actualizar un usuario existente
//[HttpPut("UpdateUser")]
//public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDTO requestDTO)
//{
//    try
//    {
//        var updatedUser = await _userBusiness.UpdateUser(id, requestDTO);
//        return Ok(updatedUser);
//    }
//    catch (ArgumentException ex)
//    {
//        return BadRequest(ex.Message);
//    }
//}

//// Endpoint para obtener un usuario por ID
//[HttpGet("GetUserById")]
//public async Task<IActionResult> GetUserById(int id)
//{
//    var user = await _userBusiness.GetAllUsers(); // Implementar un método específico en UserBusiness si es necesario
//    var userToReturn = user.FirstOrDefault(u => u.Id == id);
//    if (userToReturn == null)
//        return NotFound();

//    return Ok(userToReturn);
//}