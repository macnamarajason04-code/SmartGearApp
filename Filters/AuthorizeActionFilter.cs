using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartGearApp.Filters
{
    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            
            bool isAuthorized = true;

            if (!isAuthorized)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
