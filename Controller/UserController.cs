using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model.Models;
using FutsalGoal.Model.SpModels;
using HelperClass;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static HelperClass.EnumCollection;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;
        private readonly IOptions<CaptchaConfig> _configCaptcha;
        public UserController(IOptions<ConfigApiValues> config, IOptions<CaptchaConfig> configCaptcha)
        {
            _configValues = config;
            _configCaptcha = configCaptcha;
        }

        [SkipFilterAction]
        public IActionResult Register()
        {
            return View();
        }

        [SkipFilterAction]
        [HttpPost]
        public IActionResult Register(UserRegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "User/RegisterUserPublic", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(model), UserSession?.Token);
                    ViewBag.Message = returnModel.Message;
                    ViewBag.Success = returnModel.Success;
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values
                              .SelectMany(v => v.Errors)
                              .Select(e => e.ErrorMessage));
                    ViewBag.Message = message;
                    ViewBag.Success = false;
                }
                return View(model);
            }
            catch (Exception e)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        [SkipFilterAction]
        [HttpPost]
        public IActionResult SendAccountInfo(string emailId)
        {
            try
            {
                if (!new EmailAddressAttribute().IsValid(emailId))
                {
                    return Json(ReturnHelper.AjaxResponse(false, "Please enter valid email address.", string.Empty));
                }
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "user/SendAccountInfo", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(emailId), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        [SkipFilterAction]
        [HttpGet]
        public IActionResult ForgetPassword(string emailId)
        {
            return View("ForgetPassword");
        }

        [SkipFilterAction]
        [HttpGet]
        public IActionResult ResendEmail(string emailId)
        {
            return View("ResendEmail");
        }

        [SkipFilterAction]
        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordModel model)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "User/ForgetPassword", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(model), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            catch (Exception e)
            {
                ViewBag.Message = "Something went wrong. Please try again later.";
                return View(model);
            }
        }

        [SkipFilterAction]
        [HttpPost]
        public IActionResult SendCode(ForgetPasswordModel model)
        {
            try
            {
                var returnModel = new ReturnModel();
                returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "User/SendCode", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(model), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            catch (Exception e)
            {
                ViewBag.Message = "Error on Login. Please try again later";
                return View(model);
            }
        }

        [SkipFilterAction]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SendActivateEmail(string emailId)
        {
            try
            {
                if (!new EmailAddressAttribute().IsValid(emailId))
                {
                    return Json(ReturnHelper.AjaxResponse(false, "Please enter valid email address.", string.Empty));
                }
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "user/SendActivateEmail", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(emailId), UserSession?.Token);
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            catch (Exception)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        //[SkipFilterAction]
        //[HttpGet]
        //public IActionResult ResendVerificationCode(string emailId)
        //{
        //    var activateModel = new ActivateModel
        //    {
        //        UserDetailId = id,
        //        activationCode = confirmKey
        //    };
        //    var returnModel = ApiHelper.CallApi(ConfigValues, "user/ActivateEmail", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(activateModel));
        //    return View(returnModel);
        //}


        [SkipFilterAction]
        [HttpGet]
        public IActionResult ActivateEmail(int id, string confirmKey)
        {
            var activateModel = new ActivateModel
            {
                UserDetailId = id,
                activationCode = confirmKey
            };
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "user/ActivateEmail", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(activateModel), UserSession?.Token);
            return View(returnModel);
        }

        [SkipFilterAction]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string url = "")
        {
            ViewData["ReCaptchaKey"] = _configCaptcha.Value.SiteKey;
            var model = new LoginModel { ReturnUrl = url };

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }

        [SkipFilterAction]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                var returnModel = new ReturnModel();
                returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "User/Login", RestSharp.Method.POST, Newtonsoft.Json.JsonConvert.SerializeObject(model), UserSession?.Token);
                if (returnModel.Success)
                {
                    var spMOdel = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLoginSpModel>(returnModel.ReturnData);

                    var claims = new List<Claim> { new Claim(MyClaimTypes.AccessToken.ToString(), Encoding.UTF8.GetString(Convert.FromBase64String(spMOdel.Token.Substring(1)))) };

                    var userIdentity = new ClaimsIdentity("custom");
                    userIdentity.AddClaims(claims);
                    ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);

                    HttpContext.SignOutAsync("Cookies");
                    HttpContext.SignInAsync("Cookies", userPrincipal,
                        new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddMonths(30),
                            IsPersistent = true,
                            AllowRefresh = true
                        });


                    return RedirectToAction("Index", "Dashboard");
                }
                ViewBag.Message = returnModel.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error on Login. Please try again later";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync("Cookies");
            //return RedirectToPage("/Index");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult RoleAuthentication()
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "user/GetRoles", RestSharp.Method.GET, string.Empty, UserSession?.Token);
            if (returnModel.Success)
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ConfigChoiceModel>>(returnModel.ReturnData);
                TempData["Roles"] = data;
            }
            else
            {
                TempData["Roles"] = new List<ConfigChoiceModel>();
            }
            return View();
        }


        public bool ReCaptchaPassed(string gRecaptchaResponse)
        {
            var secret = _configCaptcha.Value.SecretKey;
            HttpClient httpClient = new HttpClient();
            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={gRecaptchaResponse}").Result;
            if (res.StatusCode != HttpStatusCode.OK)
            {
                // logger.LogError("Error while sending request to ReCaptcha");
                return false;
            }

            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
            {
                return false;
            }
            return true;
        }
    }

}