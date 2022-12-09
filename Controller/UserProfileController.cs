using FutsalGoal.Model.Models;
using FutsalGoal.Model.ViewModels;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class UserProfileController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _config;

        public UserProfileController(IOptions<ConfigApiValues> config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            var model = new UserProfileViewModel();
            try
            {

                var result = ApiHelper.CallApi(_config.Value.ApiUrl, _config.Value.ApiKey, "UserProfile/GetUserProfile", Method.POST,"", UserSession?.Token);
                if (result.Success)
                {
                    model = JsonConvert.DeserializeObject<UserProfileViewModel>(result.ReturnData);
                    return View(model); ;

                }
                return View(model); ;
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
    }
}
