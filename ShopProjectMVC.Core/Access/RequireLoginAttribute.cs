using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace ShopProjectMVC.Core.Access;

public class RequireLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userName = context.HttpContext.Session.GetString("user");
        if (string.IsNullOrEmpty(userName))
        {
            context.Result = new RedirectToActionResult("Login", "User", null);
        }
        
        base.OnActionExecuting(context);
    }
}