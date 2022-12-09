using HelperClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using FutsalGoal.Model.Models;
using FutsalGoal.Controllers;
using FutsalGoal.Model.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using FutsalGoal.Utility;
using SKLib.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FutsalGoal.Filters
{
    public class ActionFilter : ActionFilterAttribute
    {
        private readonly IOptions<ConfigApiValues> _configValues;

        private const string PageNotFound = "PageNotFound";

        private const string UnAuthorizedAccess = "UnAuthorizedAccess";
        public ActionFilter(IOptions<ConfigApiValues> configValues)
        {
            _configValues = configValues;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //skip all skipfilters
            var skipFilter = context.Filters.FirstOrDefault(a => a.GetType() == typeof(SkipFilterAction));
            if (skipFilter != null)
                return;

            var auth = context.HttpContext.User.Identity.IsAuthenticated;
            if (!auth)
            {
                context.Result = (context.Controller as Controller).RedirectToAction("Login", "User");
                return;
            }

            var isAjax = context.HttpContext.Request.IsAjax(context.HttpContext.Request.Method);
            if (isAjax)
                return;

            var roleId = HttpHelper.GetClaimsFromAccessToken(context.HttpContext).RoleId;

            var actionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;
            var controllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ControllerName;

            var model = new AuthenticationModel
            {
                ActionName = actionName,
                ControllerName = controllerName,
                UserRoleId = roleId
            };

            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "user/IsAuthenticated", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(model));
            if (returnModel.Success)
            {
                var data = returnModel.ReturnData.Replace("\"", "");

                if (UnAuthorizedAccess.Equals(data, StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = (context.Controller as Controller).RedirectToAction("Index", "UnAuthorized");
                    return;
                }
                else if (PageNotFound.Equals(data, StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = (context.Controller as Controller).RedirectToAction("Index", "PageNotFound");
                    return;
                }
            }
            else
            {
                context.Result = (context.Controller as Controller).RedirectToAction("Login", "User");
                return;
            }
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}
