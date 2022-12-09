using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model;
using FutsalGoal.Model.Models;
using FutsalGoal.Model.SpModels;
using FutsalGoal.Model.ViewModels;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class ControllerActionController : BaseController
    {
        private readonly IOptions<ConfigApiValues> ConfigValues;

        public ControllerActionController(IOptions<ConfigApiValues> config)
        {
            ConfigValues = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ControllerActionModel();
            ReturnModel returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, this.ConfigValues.Value.ApiKey, "ConfigChoice/GetAllUserRole", Method.POST, string.Empty, UserSession?.Token);
            if (returnModel.Success)
            {
                var roleList = JsonConvert.DeserializeObject<IEnumerable<ConfigChoiceModel>>(returnModel.ReturnData);
                model.ConfighchoiceList = roleList.ToList();
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult GetControllerActionList(ControllerActionPaginationRequestModel paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "ControllerAction/GetControllerActionList", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (returnModel.Success)
                {
                    var model = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                    var choiceList = JsonConvert.DeserializeObject<IEnumerable<ControllerActionSpModel>>(model.Data);
                    return Json(new { total = model.Total, rows = choiceList });
                }
                return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
        [HttpGet]
        public IActionResult AddUpdate(int controllerActionId = 0)
        {
            var data = new ControllerActionViewModel();
            if (controllerActionId != 0)
            {
                var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "ControllerAction/GetControllerActionById", Method.POST, JsonConvert.SerializeObject(controllerActionId), UserSession?.Token);
                if (returnModel.Success)
                {
                    data = JsonConvert.DeserializeObject<ControllerActionViewModel>(returnModel.ReturnData);
                }
            }
            return PartialView("_AddUpdate", data);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveControllerAction(ControllerActionViewModel model)
        {
            var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "ControllerAction/SaveControllerAction", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, null));
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveControllerActionMapping(List<ControllerActionSpModel> models)
        {
            var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "ControllerAction/SaveControllerActionMapping", Method.POST, JsonConvert.SerializeObject(models), UserSession?.Token);
            return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, null));
        }

        [HttpPost]
        public IActionResult DeleteControllerAction(int controllerActionId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "ControllerAction/DeleteControllerAction", Method.POST, JsonConvert.SerializeObject(controllerActionId), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, null));
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
    }
}