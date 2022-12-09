using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutsalGoal.Model.Models;
using FutsalGoal.Controllers;
using FutsalGoal.Model;
using FutsalGoal.Model.AppModels;
using FutsalGoal.Model.AppModels.Doctors;
using FutsalGoal.Model.AppModels.Sp_Models;
using FutsalGoal.Model.Models;
using FutsalGoal.Utility;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using RestSharp;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class MedicalOrgController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        public MedicalOrgController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var medicalOrgModel = new MedicalOrgModel();

            ReturnModel returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Configchoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.District.ToString()), UserSession?.Token);
            if (returnModel.Success)
                medicalOrgModel.DistrictList = JsonConvert.DeserializeObject<List<ConfigChoiceModel>>(returnModel.ReturnData);

            return View(medicalOrgModel);
        }

        [HttpGet]
        public ActionResult LoadAdditionalMedicalInfo(int orgId)
        {
            MedicalOrgModel medicalOrgModel = new MedicalOrgModel();
            ReturnModel returnModel;
            if (orgId != 0)
            {
                returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalOrg/GetMedicalOrgById", Method.POST, JsonConvert.SerializeObject(orgId), UserSession?.Token);
                if (returnModel.Success)
                    medicalOrgModel = JsonConvert.DeserializeObject<MedicalOrgModel>(returnModel.ReturnData);
            }

            returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Configchoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.MedicalType.ToString()), UserSession?.Token);
            if (returnModel.Success)
                medicalOrgModel.MedicalTypeList = JsonConvert.DeserializeObject<List<ConfigChoiceModel>>(returnModel.ReturnData);

            return PartialView("_aditional_medicalInfo", medicalOrgModel);
        }

        public ActionResult AddUpdate(int orgId, int medOrgId, string title)
        {
            MedicalOrgModel medOrgModel = new MedicalOrgModel()
            {
                MedicalOrgId = medOrgId,
                OrganizationId = orgId,
                Title = title
            };
            return PartialView("_AddUpdate", medOrgModel);
        }

        [HttpPost]
        public ActionResult LoadMedicalDoctor(int orgMedId)
        {
            MedicalOrgModel medOrgModel = new MedicalOrgModel()
            {
                MedicalOrgId = orgMedId,
            };
            return PartialView("_doctors_org", medOrgModel);
        }

        public ActionResult LoadMedicine(int orgMedId)
        {
            MedicalOrgModel medOrgModel = new MedicalOrgModel()
            {
                MedicalOrgId = orgMedId,
            };
            return PartialView("_medicine_org", medOrgModel);
        }

        [HttpPost]
        public IActionResult SaveMedicalOrg(MedicalOrgModel model)
        {
            ModelState.Remove("model.Title");
            if (ModelState.IsValid)
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalOrg/Admin/SaveMedicalOrg", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel != null)
                {
                    var data = JsonConvert.DeserializeObject<MedicalOrgModel>(returnModel.ReturnData);
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
        [HttpGet]
        public IActionResult GetAllMedicalOrg(PaginationRequestMedicalOrg paginationModel)
        {
            try
            {
                paginationModel.appName = EnumCollection.AppName.Medical.ToString();

                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalOrg/Admin/GetAllMedicalOrg", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var medicals = JsonConvert.DeserializeObject<IEnumerable<MedicalOrgSpModel>>(paginationResponseModel.Data);

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

        public ActionResult AddMedicineToOrg(int medDetailId)
        {
            var mdModel = new MedicineOrgModel();
            if (medDetailId != 0)
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/GetMedicalDetail", Method.POST, JsonConvert.SerializeObject(medDetailId), UserSession?.Token);
                if (returnModel != null && returnModel.Success)
                    mdModel = JsonConvert.DeserializeObject<MedicineOrgModel>(returnModel.ReturnData);
            }
            return PartialView("_addDrugToOrg", mdModel);
        }

        public ActionResult GetMedicalOrgs(string medicalOrgName)
        {
            var mdModel = new List<MedicalOrgSpModel>();

            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalOrg/Admin/GetMedicalOrgs", Method.POST, JsonConvert.SerializeObject(medicalOrgName), UserSession?.Token);
            if (returnModel != null && returnModel.Success)
                mdModel = JsonConvert.DeserializeObject<List<MedicalOrgSpModel>>(returnModel.ReturnData);
            return Json(ReturnHelper.AjaxResponse(true, "Success", mdModel));
        }

        public IActionResult SaveRelatedMedicine(IEnumerable<IFormFile> files, int medOrgId)
        {
            if (files.Any())
            {
                var uId = UserSession.UserDetailId;
                ExcelWorksheet worksheet;
                try
                {
                    ExcelPackage package = new ExcelPackage(files.First().OpenReadStream());
                    worksheet = package.Workbook.Worksheets.FirstOrDefault();
                }
                catch (Exception)
                {
                    return Json(ReturnHelper.AjaxResponse(false, "Oops!! Your excel sheet doesnot look good. Please verfify data and try again.", null));
                }
                List<MedicineOrgSpModel> models = worksheet.ImportExcelToList<MedicineOrgSpModel>();
                foreach (var model in models)
                {
                    model.UserDetailId = uId;
                    model.MedicalOrgId = medOrgId;

                }
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalOrg/Admin/SaveRelatedMedicine", Method.POST, JsonConvert.SerializeObject(models), UserSession?.Token);
                if (returnModel != null)
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, ""));
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
            else
            {
                return Json(ReturnHelper.AjaxResponse(false, "Failed to parse file. Please verfify data and try again.", null));
            }
        }
    }
}