using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class PermissionChecker : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionChecker(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if user is authenticated
        if (context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check if user has the required permission
        if (!string.IsNullOrEmpty(_permission) && !context.HttpContext.User.HasClaim(c => c.Type == "Permission" && c.Value == _permission))
        {
            // User does not have the required permission, return a forbidden result with an error message
            context.Result = new ObjectResult(new { message = $"No tienes permiso para ejecutar {_permission}" })
            {
                StatusCode = 403
            };
            return;
        }
    }
}