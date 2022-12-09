using HelperClass;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace FutsalGoal.Utility
{
    public class AjaxAttribute : ActionMethodSelectorAttribute
    {
        public EnumCollection.HttpVerbs HttpVerb { get; set; }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            return routeContext.HttpContext.Request.IsAjax(HttpVerb.ToString());
        }
    }
}
