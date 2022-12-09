using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Helper;
using FutsalGoal.Model;
using FutsalGoal.Model.AppModels;
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
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class FutsalOrgController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FutsalOrgController(IOptions<ConfigApiValues> config, IWebHostEnvironment hostingEnvironment)
        {
            _configValues = config;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            FutsalOrgModel futsalOrgModel = new FutsalOrgModel();

            ReturnModel returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Configchoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.District.ToString()), UserSession?.Token);
            if (returnModel.Success)
                futsalOrgModel.DistrictList = JsonConvert.DeserializeObject<List<ConfigChoiceModel>>(returnModel.ReturnData);

            return View(futsalOrgModel);
        }

        public IActionResult GetAllFutsalOrg(PaginationRequestFutsalOrg paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "FutsalOrg/GetAllFutsalOrg", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var userDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<FutsalOrgSpModel>>(paginationResponseModel.Data);

                return Json(new
                {
                    total = paginationResponseModel.Total,
                    rows = userDetailViewModels
                });
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        [HttpPost]
        public ActionResult LoadBasicInfo(int orgId)
        {
            FutsalOrgModel futsalOrgModel = new FutsalOrgModel();
            ReturnModel returnModel;
            if (orgId != 0)
            {
                returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "FutsalOrg/GetFutsalOrgById", Method.POST, JsonConvert.SerializeObject(orgId), UserSession?.Token);
                if (returnModel.Success)
                    futsalOrgModel = JsonConvert.DeserializeObject<FutsalOrgModel>(returnModel.ReturnData);
            }

            returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Configchoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.District.ToString()), UserSession?.Token);
            if (returnModel.Success)
                futsalOrgModel.DistrictList = JsonConvert.DeserializeObject<List<ConfigChoiceModel>>(returnModel.ReturnData);

            returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "Configchoice/GetConfigByCategory", Method.POST, JsonConvert.SerializeObject(EnumCollection.ConfigChoiceCategory.AppCategory.ToString()), UserSession?.Token);
            if (returnModel.Success)
                futsalOrgModel.AppTypeConfigList = JsonConvert.DeserializeObject<List<ConfigChoiceModel>>(returnModel.ReturnData);

            return PartialView("_futsalBasicInfo", futsalOrgModel);
        }

        public ActionResult LoadFutsalImages(int orgId)
        {
            List<ImageUploadModel> model = new List<ImageUploadModel>();
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "FutsalOrg/GetAllFutsalImages", Method.POST, JsonConvert.SerializeObject(orgId), UserSession?.Token);
            if (returnModel.Success)
                model = JsonConvert.DeserializeObject<List<ImageUploadModel>>(returnModel.ReturnData);
            return PartialView("_futsalImages", model);
        }


        public ActionResult AddUpdate(int orgId, string title)
        {
            FutsalOrgModel futsalOrgModel = new FutsalOrgModel()
            {
                OrganizationId = orgId,
                Title = title
            };
            return PartialView("_AddUpdate", futsalOrgModel);
        }

        public ActionResult ViewInMap()
        {
            return PartialView("_ViewInMap");
        }

        [HttpPost]
        public IActionResult DeleteOrgImage(int orgImgId)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "FutsalOrg/DeleteOrgImage", Method.POST, JsonConvert.SerializeObject(orgImgId), UserSession?.Token);
                if (returnModel != null)
                {
                    if (returnModel.Success)
                    {
                        var imgModel = JsonConvert.DeserializeObject<ImageUploadModel>(returnModel.ReturnData);
                        var imagePath = _hostingEnvironment.WebRootPath + string.Format("/{0}/{1}", "Uploads", "FutsalOrgImage");

                        var filePath = Utilities.GetImagePath(imgModel.OrganizationId, imgModel.ImageUrl, imagePath);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
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
        public IActionResult SaveFutsalOrg(FutsalOrgModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "FutsalOrg/SaveFutsalOrg", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                var data = JsonConvert.DeserializeObject<FutsalOrgModel>(returnModel.ReturnData);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult DeleteOrg(int orgId)
        {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "FutsalOrg/DeleteOrg", Method.POST, JsonConvert.SerializeObject(orgId), UserSession?.Token);
                if (returnModel != null)
                {
                    var data = JsonConvert.DeserializeObject<FutsalOrgModel>(returnModel.ReturnData);
                    return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, data));
                }
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        public IActionResult UploadImage(IEnumerable<IFormFile> files, FutsalOrgModel model)
        {
            try
            {
                ImageUploadModel imgUploadModel = new ImageUploadModel();
                var fileUtil = new FileUploadUtility(_hostingEnvironment);

                if (!files.Any())
                {
                    return Json(ReturnHelper.AjaxResponse(false, "Please select image to upload.", null));
                }
                else if (!fileUtil.IsvalidImage(files))
                {
                    return Json(ReturnHelper.AjaxResponse(false, "Please upload valid images (Jpeg, Gif, Png) .", string.Empty));
                }

                var fileName = TimeHelper.GetTimeStamp + "_" + files.First().FileName;
                model.ImageUrl = fileName;

                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "FutsalOrg/UploadImage", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel.Success)
                {
                    if (files.Any())
                    {
                        imgUploadModel = JsonConvert.DeserializeObject<ImageUploadModel>(returnModel.ReturnData.ToString());
                        fileUtil.UploadFile("FutsalOrgImage", model.OrganizationId, files, fileName);
                    }
                }
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, imgUploadModel));

            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
    }
}