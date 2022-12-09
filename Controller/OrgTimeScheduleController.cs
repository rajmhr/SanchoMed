using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model.AppModels;
using FutsalGoal.Model.AppModels.Sp_Models;
using FutsalGoal.Model.Models;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class OrgTimeScheduleController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        public OrgTimeScheduleController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }

        public IActionResult GetOrgTimeSchedule(int orgId)
        {
            var listSchedule = new List<OrgTimeScheduleModel>();

            ReturnModel returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "TimeSchedule/GetOrgSchedule", Method.POST, JsonConvert.SerializeObject(orgId), UserSession?.Token);
            if (returnModel.Success)
            {
                listSchedule = JsonConvert.DeserializeObject<List<OrgTimeScheduleModel>>(returnModel.ReturnData);
                foreach (var item in listSchedule)
                {
                    item.OrganizationId = orgId;
                }
            }
            return View("_orgTimeSchedule", listSchedule);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveOrgTimeSchedule(List<OrgTimeScheduleModel> model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "TimeSchedule/SaveOrgSchedule", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, returnModel.ReturnData));
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }
    }
}