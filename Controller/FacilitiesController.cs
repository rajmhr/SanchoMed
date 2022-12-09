using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model.AppModels;
using FutsalGoal.Model.AppModels.Sp_Models;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class FacilitiesController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        public FacilitiesController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }
        [HttpGet]
        public IActionResult GetFacilitiesView(FacilitiesSpModel model)
        {
            return PartialView("_facilities", model);
        }

        [HttpPost]
        public IActionResult GetAvailableFacilities(string search)
        {
            var facilitiesList = new List<FacilitiesSpModel>();
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Facilities/GetAvailableFacilities", Method.POST, JsonConvert.SerializeObject(search), UserSession?.Token);
            if (returnModel.Success)
                facilitiesList = JsonConvert.DeserializeObject<List<FacilitiesSpModel>>(returnModel.ReturnData);

            return PartialView("_availableFacilities", facilitiesList);
        }

        [HttpPost]
        public IActionResult GetMappedFacilities(int orgId)
        {
            var facilitiesList = new List<FacilitiesSpModel>();
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Facilities/GetMappedFacilities", Method.POST, JsonConvert.SerializeObject(orgId), UserSession?.Token);
            if (returnModel.Success)
                facilitiesList = JsonConvert.DeserializeObject<List<FacilitiesSpModel>>(returnModel.ReturnData);

            return PartialView("_mappedFacilities", facilitiesList);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveFacilities(FacilitiesModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Facilities/SaveFacilities", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                var data = JsonConvert.DeserializeObject<FacilitiesSpModel>(returnModel.ReturnData);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        public IActionResult DeleteFacilities(int fId)
        {

            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Facilities/DeleteFacilities", Method.POST, JsonConvert.SerializeObject(fId), UserSession?.Token);
            if (returnModel != null)
            {
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, returnModel.ReturnData));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());

        }
        [HttpPost]
        public ActionResult AddUpdate(string desc, int facilitesId = 0)
        {
            var model = new FacilitiesModel()
            {
                FacilitiesId = facilitesId,
                Description = desc
            };
            return PartialView("_addupdate_facilities", model);
        }

        [HttpPost]
        public IActionResult AddToOrgFacilities(int facilityId, int orgId, string title)
        {
            try
            {
                var model = new FacilitiesSpModel()
                {
                    FacilitiesId = facilityId,
                    OrganizationId = orgId,
                    Description = title
                };
                
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Facilities/AddToOrgFacilities", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel != null)
                {
                    var data = JsonConvert.DeserializeObject<FacilitiesSpModel>(returnModel.ReturnData);
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        [HttpPost]
        public IActionResult RemoveFromOrgFacilities(int orgFacilityId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Facilities/RemoveFromOrgFacilities", Method.POST, JsonConvert.SerializeObject(orgFacilityId), UserSession?.Token);
                if (returnModel != null)
                {
                    var data = JsonConvert.DeserializeObject<FacilitiesSpModel>(returnModel.ReturnData);
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }

        }
    }
}