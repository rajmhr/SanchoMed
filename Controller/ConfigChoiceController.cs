using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model;
using FutsalGoal.Model.Models;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class ConfigChoiceController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;


        public ConfigChoiceController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }

        public IActionResult Index()
        {
            var model = new ConfigChoiceModel();
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "ConfigChoiceCategory/GetAllCategory", RestSharp.Method.POST, string.Empty, UserSession?.Token);
                var modelCategoryModel = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ConfigChoiceCategoryModel>>(returnModel.ReturnData);
                model.DrpConfigCategory = modelCategoryModel.ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        public IActionResult GetConfigChoiceList(ConfigchoicePaginationRequestModel paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "configChoice/ConfigChoiceList", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (returnModel.Success)
                {
                    var model = Newtonsoft.Json.JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                    var choiceList = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ConfigChoiceModel>>(model.Data);
                    return Json(new { total = model.Total, rows = choiceList });
                }
                return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public IActionResult AddUpdate(int configChoiceId = 0)
        {
            var data = new ConfigChoiceModel();
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "configChoice/GetConfigChoice", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(configChoiceId), UserSession?.Token);
            if (returnModel.Success)
            {
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigChoiceModel>(returnModel.ReturnData);
            }
            return PartialView("_AddUpdate", data);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveConfigChoice(ConfigChoiceModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "configChoice/SaveConfigChoice", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(model), UserSession?.Token);
            return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, null));
        }

        [HttpPost]
        public IActionResult DeleteConfigChoice(int configChoiceId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "configChoice/DeleteConfigChoice", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(configChoiceId), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, null));
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
    }
}