using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model;
using FutsalGoal.Model.Models;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class ConfigChoiceCategoryController : BaseController
    {
        private readonly IOptions<ConfigApiValues> ConfigValues;


        public ConfigChoiceCategoryController(IOptions<ConfigApiValues> config)
        {
            ConfigValues = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetConfigChoiceCategoryList(PaginationRequestModel paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "configChoiceCategory/ConfigChoiceCategoryList", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (returnModel.Success)
                {
                    var model = Newtonsoft.Json.JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                    var categoryList = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ConfigChoiceCategoryModel>>(model.Data);
                    return Json(new { total = model.Total, rows = categoryList });
                }
                return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public IActionResult AddUpdate(int configChoiceCategoryId = 0)
        {
            var data = new ConfigChoiceCategoryModel();
            if (configChoiceCategoryId > 0)
            {
                var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "configChoiceCategory/GetConfigChoiceCategory", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(configChoiceCategoryId), UserSession?.Token);
                if (returnModel.Success)
                {

                    data = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigChoiceCategoryModel>(returnModel.ReturnData);
                }
            }
            return PartialView("_AddUpdate", data);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveConfigChoiceCategory(ConfigChoiceCategoryModel model)
        {
            var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "configChoiceCategory/SaveConfigChoiceCategory", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(model), UserSession?.Token);
            return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, null));
        }

        [HttpPost]
        public IActionResult DeleteConfigChoiceCategory(int configChoiceCategoryId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "configChoiceCategory/DeleteConfigChoiceCategory", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(configChoiceCategoryId), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, null));
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
    }
}