using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManager.Filters;

public class AuthorizationFilter : IAuthorizationFilter
{
    private readonly string? _requiredRole;

    public AuthorizationFilter(string? requiredRole = null)
    {
        _requiredRole = requiredRole;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.Session.GetInt32("UserId");
        var role = context.HttpContext.Session.GetString("Role");

        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (_requiredRole != null && role != _requiredRole)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}
