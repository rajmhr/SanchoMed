using FutsalGoal.Model.Models;
using FutsalGoal.Model.Models;
using HelperClass;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Linq;

namespace FutsalGoal.Filters
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        private readonly IOptions<ConfigApiValues> _configValues;

        private const string PageNotFound = "PageNotFound";
        private const string UnAuthorizedAccess = "UnAuthorizedAccess";
        public AuthorizationFilter(IOptions<ConfigApiValues> configValues)
        {
            _configValues = configValues;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var skipFilter = context.Filters.FirstOrDefault(a => a.GetType() == typeof(SkipFilterAction));
            if (skipFilter != null)
                return;

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var actionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;
                var controllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ControllerName;

                var model = new AuthenticationModel()
                {
                    ActionName = actionName,
                    ControllerName = controllerName,
                };

                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "user/IsAuthenticated", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(model));
                if (returnModel.Success)
                {
                    if (UnAuthorizedAccess.Equals(returnModel.ReturnData))
                    {
                        context.Result = new ForbidResult();
                    }
                    else if (PageNotFound.Equals(returnModel.ReturnData))
                    {
                        context.Result = new ForbidResult();
                    }
                }
                else
                {
                    context.Result = new ChallengeResult(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        }
    }
}
