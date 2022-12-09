using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static HelperClass.EnumCollection;

namespace FutsalGoal.Helpers
{
    public static class Utilities
    {
        public static string IsSelected(this IHtmlHelper htmlHelper, string controllers, string actions, string cssClass = "activenav")
        {
            string currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            IEnumerable<string> acceptedActions = (actions ?? currentAction).Split(',');
            IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : string.Empty;
        }
        public static bool IsUserInRole(this IHtmlHelper htmlHelper, List<string> roles)
        {
            var jwtToken = htmlHelper.ViewContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.AccessToken.ToString())).Value;
            var userModel = new HelperClass.JwtHelper().GetuserFromToken(jwtToken);

            if (roles.Any(a => a.Equals(userModel.UserRole, StringComparison.OrdinalIgnoreCase)))
                return true;
            return false;
        }
    }
}
