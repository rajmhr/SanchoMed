using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutsalGoal.Model.Models;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class CacheController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;

        public CacheController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ClearCache(string clearCode)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "CacheConfig/ClearCacheConfig", Method.POST, JsonConvert.SerializeObject(clearCode), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, null));
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
    }
}