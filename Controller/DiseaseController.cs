using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model;
using FutsalGoal.Model.AppModels.Doctors;
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
    public class DiseaseController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        public DiseaseController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAllDiseases(PaginationRequestModel paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Disease/GetAllDiseases", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (returnModel.Success)
                {
                    var model = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                    var choiceList = JsonConvert.DeserializeObject<IEnumerable<DiseaseSpModel>>(model.Data);
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
        public IActionResult AddUpdate(int diseaseId)
        {
            var data = new DiseaseModel();
            if (diseaseId != 0)
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Disease/GetDiseaseById", Method.POST, JsonConvert.SerializeObject(diseaseId), UserSession?.Token);
                if (returnModel.Success)
                {
                    data = JsonConvert.DeserializeObject<DiseaseModel>(returnModel.ReturnData);
                }
            }

            return PartialView("add_update", data);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveDiseaseInformation(DiseaseModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Disease/SaveDisease", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                var data = JsonConvert.DeserializeObject<DiseaseModel>(returnModel.ReturnData);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        public IActionResult SaveRelatedVideo(DiseaseModel model)
        {
            ModelState.Remove("Title");
            ModelState.Remove("Description");
            ModelState.Remove("DiseaseRelatedVideo.DiseaseRelatedVideoId");
            if (ModelState.IsValid)
            {
                var rModel = new DiseaseRelatedVideoModel()
                {
                    VideoTitle = model.DiseaseRelatedVideo.VideoTitle,
                    VideoUrl = model.DiseaseRelatedVideo.VideoUrl,
                    DiseaseId = model.DiseaseId,
                    DiseaseRelatedVideoId = model.DiseaseRelatedVideo.DiseaseRelatedVideoId
                };

                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Disease/SaveRelatedVideo", Method.POST, JsonConvert.SerializeObject(rModel), UserSession?.Token);
                if (returnModel != null)
                {
                    var data = JsonConvert.DeserializeObject<DiseaseRelatedVideoModel>(returnModel.ReturnData);
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(ReturnHelper.AjaxResponse(false, message, null));
            }
        }

        [HttpPost]
        public IActionResult DeleteRelatedVideo(int rId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Disease/DeleteRelatedVideo", Method.POST, JsonConvert.SerializeObject(rId), UserSession?.Token);
                if (returnModel != null)
                {
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, ""));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        [HttpPost]
        public IActionResult DeleteDisease(int dId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Disease/DeleteDisease", Method.POST, JsonConvert.SerializeObject(dId), UserSession?.Token);
                if (returnModel != null)
                {
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, ""));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
    }
}