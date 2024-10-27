using GoldRogerServer.DTOs.Session;
using GoldRogerServer.Business;
using GoldRogerServer.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRogerServer.Utils;

namespace GoldRogerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserBusiness _userBusiness;
        private readonly SessionBusiness _sessionBusiness;

        public SessionController(UserBusiness userBusiness, IConfiguration configuration, SessionBusiness sessionBusiness)
        {
            _userBusiness = userBusiness;
            _configuration = configuration;
            _sessionBusiness = sessionBusiness;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                    return BadRequest(new APIResponse<User> { Success = false, Message = "Usuario y/o contraseña incorrectos" });

                var user = await _userBusiness.ValidateUserCredentials(loginRequest.Email, loginRequest.Password);
                if (user == null)
                    return BadRequest(new APIResponse<User> { Success = false, Message = "Usuario y/o contraseña incorrectos" });

                // Obtener los permisos del usuario utilizando GetUserPermissionKeys
                var permissionKeys = await _sessionBusiness.GetUserPermissionKeys(user.UserId);

                var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("Username", user.Username),
               
                };

                // Agregar las claves de permiso como claims en el token JWT
                foreach (var permissionKey in permissionKeys.Values)
                {
                    claims.Add(new Claim("Permission", permissionKey));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: signIn);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                user.JWToken = tokenString;

                var response = new APIResponse<User>
                {
                    Success = true,
                    Data = user
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse<User> { Success = false, Message = ex.Message });
            }
        }
    }
}