using FutsalGoal.Controllers;
using FutsalGoal.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace RealInvest.Controllers
{
    [AllowAnonymous]
    [SkipFilterAction]
    public class ErrorController : BaseController
    {
        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [Route("error/{code:int}")]
        public IActionResult Error(int code)
        {
            // handle different codes or just return the default error view
            return View(code);
        }

        public IActionResult Test()
        {
            return View();
        }

    }
}
