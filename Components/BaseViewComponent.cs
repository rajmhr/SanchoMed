using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutsalGoal.Model.SpModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FutsalGoal.Components
{
    public class BaseViewComponent : ViewComponent
    {
        IHttpContextAccessor _contextAccessor;
        public BaseViewComponent(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public UserLoginSpModel UserSession
        {
            get
            {
                var strSession = _contextAccessor.HttpContext.Session.GetString("UserSession");
                if (strSession != null)
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<UserLoginSpModel>(strSession);
                return null;
            }
        }
        public virtual IViewComponentResult Invoke()
        {
            return View();
        }
    }
}