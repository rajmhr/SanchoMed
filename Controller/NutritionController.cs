using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Helper;
using FutsalGoal.Model;
using FutsalGoal.Model.AppModels;
using FutsalGoal.Model.AppModels.Doctors;
using FutsalGoal.Model.Models;
using FutsalGoal.Utility;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class NutritionController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        private readonly IWebHostEnvironment _hostingEnvironment;
        const string nutritionLoc = "NutritionImage";
        public NutritionController(IOptions<ConfigApiValues> config, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configValues = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllNutritions(PaginationRequestNutrition paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Nutrition/GetAllNutritions", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var medicals = JsonConvert.DeserializeObject<IEnumerable<NutritionSpModel>>(paginationResponseModel.Data);

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

        public ActionResult AddUpdate(int nutId)
        {
            var mdModel = new NutritionModel();

            ReturnModel returnModel;
            if (nutId != 0)
            {
                returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Nutrition/GetNutritionById", Method.POST, JsonConvert.SerializeObject(nutId), UserSession?.Token);
                if (returnModel.Success)
                    mdModel = JsonConvert.DeserializeObject<NutritionModel>(returnModel.ReturnData);
            }
            returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Configchoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.FoodType.ToString()), UserSession?.Token);
            if (returnModel.Success)
                mdModel.FoodTypes = JsonConvert.DeserializeObject<List<ConfigChoiceModel>>(returnModel.ReturnData);
            return PartialView("_addUpdate", mdModel);
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
        public IActionResult DeleteNutrition(int nutId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Nutrition/DeleteNutrition", Method.POST, JsonConvert.SerializeObject(nutId), UserSession?.Token);
                if (returnModel != null)
                {
                    var data = JsonConvert.DeserializeObject<NutritionSpModel>(returnModel.ReturnData.ToString());

                    var imagePath = _hostingEnvironment.WebRootPath + string.Format("/{0}/{1}", "Uploads", EnumCollection.FolderResName.NutritionImg.ToString());
                    var filePath = Utilities.GetImagePath(data.NutritionId, data.ImageUrl, imagePath, EnumCollection.FolderResName.NutritionImg.ToString());
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

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
        [ValidationFilter]
        public IActionResult SaveNutrition(IEnumerable<IFormFile> files, NutritionModel model)
        {
            var fileName = string.Empty;
            try
            {
                var fileUtil = new FileUploadUtility(_hostingEnvironment);
                if (files.Any() && !fileUtil.IsvalidImage(files))
                {
                    return Json(ReturnHelper.AjaxResponse(false, "Please upload valid images (Jpeg, Gif, Png) .", string.Empty));
                }
                var oldImg = model.ImageUrl?.Split("/").Last();
                if (files.Any())
                {
                    fileName = TimeHelper.GetTimeStamp + "_" + files.First().FileName;
                    model.ImageUrl = fileName;
                }
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Nutrition/SaveNutrition", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel.Success)
                {
                    if (files.Any())
                    {
                        var data = JsonConvert.DeserializeObject<NutritionSpModel>(returnModel.ReturnData.ToString());
                        model.NutritionId = data.NutritionId;
                        if (!string.IsNullOrEmpty(model.ImageUrl))
                        {
                            var imagePath = _hostingEnvironment.WebRootPath + string.Format("/{0}/{1}", "Uploads", EnumCollection.FolderResName.NutritionImg.ToString());
                            var filePath = Utilities.GetImagePath(data.NutritionId, oldImg, imagePath, EnumCollection.FolderResName.NutritionImg.ToString());
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }

                        fileUtil.UploadFile(EnumCollection.FolderResName.NutritionImg.ToString(), model.NutritionId, files, fileName);
                    }
                }
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));

            }

            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
    }
}