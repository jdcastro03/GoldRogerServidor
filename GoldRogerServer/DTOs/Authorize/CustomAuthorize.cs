using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;


public class KitAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{

    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!await IsUserActive(user).ConfigureAwait(false)) // Esperar y obtener el resultado de manera asincrónica
        {
            context.Result = new UnauthorizedObjectResult("El usuario no está autenticado"); // Devolver un mensaje de error
        }
    }

    private async Task<bool> IsUserActive(ClaimsPrincipal user)
    {
        if (user == null)
        {
            throw new ArgumentException(message: "El usuario no está autenticado"); // Usuario no autenticado
        }

        // Obtener el Id del usuario desde las reclamaciones del token
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException(message: "El usuario no está activo"); // Usuario autenticado pero no activo
        }

        return true; // El usuario está activo
    }
}
