using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClickAndCollect.Filters
{
    public class RoleFilter : ActionFilterAttribute
    {
        private readonly string _role;

        public RoleFilter(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            string? role = session.GetString("Role");

            if (role != _role)
            {
                session.Clear();
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}