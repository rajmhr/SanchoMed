using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model;
using FutsalGoal.Model.AppModels;
using FutsalGoal.Model.AppModels.Doctors;
using FutsalGoal.Model.AppModels.Medical;
using FutsalGoal.Model.Models;
using FutsalGoal.Utility;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class MedicineController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        public MedicineController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexGeneric()
        {
            return View("Index_Generic");
        }

        public IActionResult GetAllMedicines(PaginationRequestMedicines paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/GetAllMedicines", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var medicals = JsonConvert.DeserializeObject<IEnumerable<MedicineSpModel>>(paginationResponseModel.Data);

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

        public IActionResult GetAllGenericDrug(PaginationRequestMedicines paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/GetAllGenericDrug", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var medicals = JsonConvert.DeserializeObject<IEnumerable<DrugGenericSpModel>>(paginationResponseModel.Data);

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

        public IActionResult GetAllMedicineOrgDetail(PaginationRequestMedicines paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/GetAllMedicineOrgDetail", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var drugs = JsonConvert.DeserializeObject<IEnumerable<DrugOrgDetailSpModel>>(paginationResponseModel.Data);

                return Json(new
                {
                    total = paginationResponseModel.Total,
                    rows = drugs
                });
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
        public IActionResult GetMedicine(string medName)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/GetMedicine", Method.POST, JsonConvert.SerializeObject(medName), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, returnModel.ReturnData));
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public ActionResult AddUpdate(int medId)
        {
            var mdModel = new MedicineDetailModel();

            ReturnModel returnModel;
            if (medId != 0)
            {
                returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/GetMedicineById", Method.POST, JsonConvert.SerializeObject(medId), UserSession?.Token);
                if (returnModel.Success)
                    mdModel = JsonConvert.DeserializeObject<MedicineDetailModel>(returnModel.ReturnData);
            }
            return PartialView("_addUpdate", mdModel);
        }


        public ActionResult AddUpdateGeneric(int genericId)
        {
            var mdModel = new DrugGenericModel()
            {
                DrugGenericId = 0
            };

            ReturnModel returnModel;
            if (genericId != 0)
            {
                returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/GetGenericById", Method.POST, JsonConvert.SerializeObject(genericId), UserSession?.Token);
                if (returnModel.Success)
                    mdModel = JsonConvert.DeserializeObject<DrugGenericModel>(returnModel.ReturnData);
            }
            return PartialView("_addUpdateGeneric", mdModel);
        }

        [HttpPost]
        public IActionResult AddMedicineOrg(MedicineOrgModel model)
        {
            ModelState.Remove("model.Composition");
            if (ModelState.IsValid)
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/AddMedicineOrg", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel != null)
                {
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
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
        [ValidationFilter]
        public IActionResult SaveMedicine(MedicineDetailModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/SaveMedicine", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveGeneric(DrugGenericModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/SaveGeneric", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        public IActionResult DeleteMedicine(int medId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/DeleteMedicine", Method.POST, JsonConvert.SerializeObject(medId), UserSession?.Token);
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
        public IActionResult DeleteGeneric(int genericId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/DeleteGeneric", Method.POST, JsonConvert.SerializeObject(genericId), UserSession?.Token);
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
        public IActionResult DeleteMedicineOrg(int medOrgDetailId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/DeleteMedicineOrg", Method.POST, JsonConvert.SerializeObject(medOrgDetailId), UserSession?.Token);
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

        public IActionResult GetMedicineDataFromFile(IEnumerable<IFormFile> files, int medOrgId)
        {
            try
            {
                if (files.Any())
                {
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

                    var models = worksheet.ImportExcelToList<MedicineOrgSpModel>();
                    foreach (var model in models)
                    {
                        model.MedicalOrgId = medOrgId;
                    }

                    var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "MedicalOrg/Admin/ValidateMedicine", Method.POST, JsonConvert.SerializeObject(models), UserSession?.Token);
                    if (returnModel.Success)
                        models = JsonConvert.DeserializeObject<List<MedicineOrgSpModel>>(returnModel.ReturnData);

                    return Json(new
                    {
                        total = models.Count,
                        rows = models
                    });
                }
                else
                {
                    return Json(ReturnHelper.AjaxResponse(false, "Failed to parse file. Please verfify data and try again.", null));
                }
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public ActionResult GetGenerics(string generics)
        {
            var mdModel = new List<DrugGenericSpModel>();

            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Medicine/Admin/GetGenerics", Method.POST, JsonConvert.SerializeObject(generics), UserSession?.Token);
            if (returnModel != null && returnModel.Success)
                mdModel = JsonConvert.DeserializeObject<List<DrugGenericSpModel>>(returnModel.ReturnData);
            return Json(ReturnHelper.AjaxResponse(true, "Success", mdModel));
        }

    }
}
