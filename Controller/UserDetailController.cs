using FutsalGoal.Model.Models;
using FutsalGoal.Model;
using FutsalGoal.Model.Models;
using FutsalGoal.Model.SpModels;
using FutsalGoal.Model.ViewModels;
using HelperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class UserDetailController : BaseController
    {
        private readonly IOptions<ConfigApiValues> ConfigValues;

        public UserDetailController(IOptions<ConfigApiValues> config)
        {
            ConfigValues = config;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = new UserDetailDisplayViewModel();
            ReturnModel returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "ConfigChoice/GetAllUserRole", Method.POST, string.Empty, UserSession?.Token);
            if (returnModel.Success)
            {
                var roleList = JsonConvert.DeserializeObject<IEnumerable<ConfigChoiceModel>>(returnModel.ReturnData);
                model.ListConfigChoiceRole = roleList.ToList();
            }
            return View(model);
        }

        public ActionResult ViewUserDetail(int userDetailId)
        {
            try
            {
                UserDetailViewModel userDetailViewModel = new UserDetailViewModel();
                ReturnModel returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "UserDetail/GetUserDetailById", Method.POST, JsonConvert.SerializeObject(userDetailId), UserSession?.Token);
                if (returnModel.Success)
                    userDetailViewModel = JsonConvert.DeserializeObject<UserDetailViewModel>(returnModel.ReturnData);
                return PartialView("_UserDetail", userDetailViewModel);
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public IActionResult GetAllUsers(PaginationRequestUserDetailModel paginationModel)
        {
            try
            {
                paginationModel.UserRole = UserSession.UserRole;

                var returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "UserDetail/GetAllUsers", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession?.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var paginationResponseModel = JsonConvert.DeserializeObject<PaginationResponseModel>(returnModel.ReturnData);
                var userDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<UserDetailSpModel>>(paginationResponseModel.Data);

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

        public ActionResult AddUpdate(int userDetailId)
        {
            UserRegisterModel userRegisterModel = new UserRegisterModel();
            if (userDetailId == 0)
            {
                ReturnModel returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "UserDetail/GetAllUserRoles", Method.POST, JsonConvert.SerializeObject(userDetailId), UserSession?.Token);
                if (returnModel.Success)
                    userRegisterModel.UserRoles = JsonConvert.DeserializeObject<List<UserRoleViewModel>>(returnModel.ReturnData);
            }
            else
            {
                ReturnModel returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "UserDetail/GetUserByUserId", Method.POST, JsonConvert.SerializeObject(userDetailId), UserSession?.Token);
                if (returnModel.Success)
                    userRegisterModel = JsonConvert.DeserializeObject<UserRegisterModel>(returnModel.ReturnData);
            }
            return PartialView("_AddUpdate", userRegisterModel);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(PasswordModel model)
        {
            try
            {
                var result = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "UserDetail/ChangePassword", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(result.Success, result.Message, result.ReturnData));
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        [HttpPost]
        public IActionResult Register(UserRegisterModel model)
        {
            try
            {
                ModelState.Remove("Password");
                ModelState.Remove("RePassword");
                if (!ModelState.IsValid)
                    return Json(ReturnHelper.AjaxResponse(false, string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), null));
                model.CreatedBy = UserSession.UserDetailId;
                ReturnModel returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "user/RegisterUserPublic", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        private void RemoveValidation()
        {
            ModelState.Remove("Password");
            ModelState.Remove("Organization");
        }

        [HttpPost]
        public IActionResult Delete(int userDetailId)
        {
            try
            {
                return Json(ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "UserDetail/DeleteUser", Method.POST, JsonConvert.SerializeObject(userDetailId), UserSession?.Token));
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public IActionResult GetOrganizations(string searchTerm)
        {
            try
            {
                ReturnModel returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "Organization/GetOrganizations", Method.POST, JsonConvert.SerializeObject(searchTerm), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, returnModel.ReturnData));
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.AjaxResponse(false, "Something went wrong.Please try again later!", ""));
            }
        }

        public IActionResult FindUser(string searchTerm)
        {
            try
            {
                ReturnModel returnModel = ApiHelper.CallApi(ConfigValues.Value.ApiUrl, ConfigValues.Value.ApiKey, "UserDetail/FindUser", Method.POST, JsonConvert.SerializeObject(searchTerm), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, returnModel.ReturnData));
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }
    }
}
