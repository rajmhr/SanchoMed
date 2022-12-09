using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model;
using FutsalGoal.Model.AppModels.Medical;
using FutsalGoal.Model.AppModels.Medical.SpModel;
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
    public class MedicalEmergencyController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        public MedicalEmergencyController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }
        public IActionResult Index()
        {
            var model = new MedicalEmergencyModel();
            ReturnModel returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "ConfigChoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.EmergencyMsgDeliveryStatus.ToString()), UserSession?.Token);
            if (returnModel.Success)
            {
                var requestDeliveryStatus = JsonConvert.DeserializeObject<IEnumerable<ConfigChoiceModel>>(returnModel.ReturnData);
                model.EmergencyRequestDeliveryStatus = requestDeliveryStatus.ToList();
            }

            returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "ConfigChoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.EmergencyRequestType.ToString()), UserSession?.Token);
            if (returnModel.Success)
            {
                var reqEmergencyStatus = JsonConvert.DeserializeObject<IEnumerable<ConfigChoiceModel>>(returnModel.ReturnData);
                model.EmergencyRequestTypes = reqEmergencyStatus.ToList();
            }
            return View(model);
        }

        public IActionResult GetAllMedicalEmergency(MedicalEmergencyPaginationRequestModel paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Emergency/Admin/GetAllEmergencyMsgRequests", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var medicals = JsonConvert.DeserializeObject<IEnumerable<EmergencyMsgSpModel>>(paginationResponseModel.Data);

                return Json(new
                {
                    total = paginationResponseModel.Total,
                    rows = medicals
                });
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public ActionResult AddUpdate(int emergencyId)
        {
            var mdModel = new MedicalEmergencyModel();

            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Emergency/Admin/GetEmergencyMsgById", Method.POST, JsonConvert.SerializeObject(emergencyId), UserSession?.Token);
            if (returnModel.Success)
            {
                mdModel = JsonConvert.DeserializeObject<MedicalEmergencyModel>(returnModel.ReturnData);
                if (mdModel != null)
                {
                    if (mdModel.ExpiringDate != null)
                    {
                        mdModel.ExpirationAfterDays = Convert.ToInt32((mdModel.ExpiringDate - TimeHelper.GetUTCTime).Value.TotalDays);
                    }
                }
            }

            return PartialView("_addUpdate", mdModel);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveEmergencyRequest(MedicalEmergencyModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Emergency/Admin/SaveEmergencyRequest", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                var data = JsonConvert.DeserializeObject<MedicalEmergencyModel>(returnModel.ReturnData);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        public IActionResult CompleteExpiredRequests()
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Emergency/Admin/CompleteExpiredRequests", Method.POST, string.Empty, UserSession?.Token);
                if (returnModel != null)
                {
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, ""));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }

        }


        [HttpPost]
        public IActionResult DeliverMsg(MedicalEmergencyModel model)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Emergency/Admin/DeliverMsg", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel != null)
                {
                    var data = JsonConvert.DeserializeObject<MedicalEmergencyModel>(returnModel.ReturnData);
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public ActionResult ViewInMap(decimal lat, decimal lon)
        {
            var mdModel = new MedicalEmergencyModel()
            {
                Lattitude = lat,
                Longitude = lon
            };
            return PartialView("_ViewInMap", mdModel);
        }

    }
}
