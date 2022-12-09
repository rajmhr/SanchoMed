using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model;
using FutsalGoal.Model.AppModels.Doctors;
using FutsalGoal.Model.Models;
using FutsalGoal.Utility;
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
    public class MedicalDoctorController : BaseController
    {

        private readonly IOptions<ConfigApiValues> _configValues;
        public MedicalDoctorController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllMedicalDoctors(PaginationRequestMedicalDoctors paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/Admin/GetAllMedicalDoctors", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var medicals = JsonConvert.DeserializeObject<IEnumerable<MedicalDoctorSpModel>>(paginationResponseModel.Data);

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

        public IActionResult GetAllMappedMedicalDoctors(PaginationRequestMedicalDoctors paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/GetAllMappedMedicalDoctors", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var medicals = JsonConvert.DeserializeObject<IEnumerable<MedicalDoctorSpModel>>(paginationResponseModel.Data);

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

        [Ajax(HttpVerb = EnumCollection.HttpVerbs.POST)]
        public ActionResult UpdateSchedule(int medOrgDocId)
        {
            var medDoctorModel = new DoctorOrgModel();

            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/GetMedicalOrgDoctorById", Method.POST, JsonConvert.SerializeObject(medOrgDocId), UserSession?.Token);
            if (returnModel.Success)
                medDoctorModel = JsonConvert.DeserializeObject<DoctorOrgModel>(returnModel.ReturnData);

            medDoctorModel.OrgMedicalDoctorId = medOrgDocId;

            return PartialView("_updateSchedule", medDoctorModel);
        }



        public ActionResult AddUpdate(int medDoctorId)
        {
            var medDoctorModel = new DoctorsModel();

            ReturnModel returnModel;
            if (medDoctorId != 0)
            {
                returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/GetMedicalDoctorById", Method.POST, JsonConvert.SerializeObject(medDoctorId), UserSession?.Token);
                if (returnModel.Success)
                    medDoctorModel = JsonConvert.DeserializeObject<DoctorsModel>(returnModel.ReturnData);
            }

            returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Configchoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.District.ToString()), UserSession?.Token);
            if (returnModel.Success)
                medDoctorModel.DistrictList = JsonConvert.DeserializeObject<List<ConfigChoiceModel>>(returnModel.ReturnData);

            return PartialView("_addUpdate", medDoctorModel);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveMedicalDoctor(DoctorsModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/Admin/SaveMedicalDoctor", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        public IActionResult SaveMedicalDoctorSchedule(DoctorOrgModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/Admin/SaveMedicalDoctorSchedule", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult DeleteMedicalDoctor(int docId)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/Admin/DeleteMedicalDoctor", Method.POST, JsonConvert.SerializeObject(docId), UserSession?.Token);
            if (returnModel != null)
            {
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        public IActionResult MapDoctor(int mDocId, int medOrgId)
        {
            try
            {
                DoctorOrgModel model = new DoctorOrgModel()
                {
                    MedicalDoctorId = mDocId,
                    MedicalOrgId = medOrgId
                };

                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/Admin/MapDoctor", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel != null)
                {
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        [HttpPost]
        public IActionResult UnMapDoctor(int orgMDocId)
        {
            try
            {
                DoctorOrgModel model = new()
                {
                    OrgMedicalDoctorId = orgMDocId,
                };
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/Admin/UnMapDoctor", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel != null)
                {
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public IActionResult GetDoctorsSpecialist(string query)
        {
            try
            {
                ReturnModel returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalDoctor/GetDoctorsSpecialist", Method.POST, JsonConvert.SerializeObject(query), UserSession?.Token);
                if (returnModel != null && returnModel.Success)
                {
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, returnModel.ReturnData));
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