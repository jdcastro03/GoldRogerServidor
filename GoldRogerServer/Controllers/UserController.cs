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
                // Verificación específica según el tipo de usuario
                switch (requestDTO.UserType)
                {
                    case 1: // Jugador
                            // Solo 'Position' es obligatorio para los jugadores; otros campos pueden ser nulos
                        if (string.IsNullOrWhiteSpace(requestDTO.Position))
                        {
                            response.Success = false;
                            response.Message = "El campo 'Position' es obligatorio para usuarios del tipo 'Jugador'.";
                            return BadRequest(response);
                        }
                        break;

                    case 2: // Árbitro
                        if (string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
                        {
                            response.Success = false;
                            response.Message = "El campo 'LicenseNumber' es obligatorio para usuarios del tipo 'Árbitro'.";
                            return BadRequest(response);
                        }
                        break;

                    case 3: // Organizador
                        if (string.IsNullOrWhiteSpace(requestDTO.OrganizationName))
                        {
                            response.Success = false;
                            response.Message = "El campo 'OrganizationName' es obligatorio para usuarios del tipo 'Organizador'.";
                            return BadRequest(response);
                        }
                        break;

                    case 4: // Entrenador
                        if (string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
                        {
                            response.Success = false;
                            response.Message = "El campo 'LicenseNumber' es obligatorio para usuarios del tipo 'Entrenador'.";
                            return BadRequest(response);
                        }
                        break;

                    default:
                        response.Success = false;
                        response.Message = "Tipo de usuario no válido.";
                        return BadRequest(response);
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
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al crear el usuario.";
                return StatusCode(500, response);
            }
        }

        // Endpoint para actualizar un usuario usando un DTO

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDTO requestDTO)
        {
            var response = new APIResponse<User> { Success = true };

            try
            {
                // Llama al método de actualización en el servicio de negocio
                response.Data = await _userBusiness.UpdateUser(requestDTO.UserId, requestDTO);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                // Manejo de errores específicos de la lógica de negocio
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                response.Success = false;
                response.Message = "Error interno del servidor";
                return StatusCode(500, response);
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