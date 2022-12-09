using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Model;
using FutsalGoal.Model.AppModels;
using FutsalGoal.Model.AppModels.Medical;
using FutsalGoal.Model.AppModels.Medical.ViewModel;
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
using System.Threading.Tasks;

namespace FutsalGoal.Controllers
{
    [Authorize]
    public class JobVacancyController : BaseController
    {
        private readonly IOptions<ConfigApiValues> _configValues;

        public JobVacancyController(IOptions<ConfigApiValues> config)
        {
            _configValues = config;
        }
        public IActionResult Index()
        {
            JobVacancyViewModel model = new()
            {
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now,
                isActiveOnly = true
            };
            return View(model);
        }

        public IActionResult GetAllJobVacancy(PaginationRequestJobVacancy paginationModel)
        {
            try
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "JobVacancy/GetAllJobVacancy", Method.POST, JsonConvert.SerializeObject(paginationModel), UserSession.Token);
                if (!returnModel.Success)
                    return Json(ReturnHelper.AjaxResponse(false, returnModel.Message, ""));

                var model = JsonConvert.DeserializeObject<IEnumerable<JobVacancySpModel>>(returnModel.ReturnData);

                return Json(new
                {
                    total = model.First().TotalRows,
                    rows = model
                });
            }
            catch (Exception ex)
            {
                return Json(ReturnHelper.GetSystemErrorAjaxResponse());
            }
        }

        public ActionResult AddUpdate(int jobId)
        {
            JobVacancyModel model = new()
            {
                ApplyBefore = DateTime.Now.AddDays(7)
            };

            if (jobId != 0)
            {
                var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "JobVacancy/GetJobVacancyById", Method.POST, JsonConvert.SerializeObject(jobId), UserSession?.Token);
                if (returnModel.Success)
                    model = JsonConvert.DeserializeObject<JobVacancyModel>(returnModel.ReturnData);
            }

            return PartialView("_AddUpdateJobVacancy", model);
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult SaveJobVacancy(JobVacancyModel model)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "JobVacancy/SaveJobVacancy", Method.POST, JsonConvert.SerializeObject(model), UserSession?.Token);
            if (returnModel != null)
            {
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }

        [HttpPost]
        [ValidationFilter]
        public IActionResult DeleteJobVacancy(int jobId)
        {
            var returnModel = ApiHelper.CallApi(_configValues.Value.ApiUrl, _configValues.Value.ApiKey, "JobVacancy/DeleteJobVacancy", Method.POST, JsonConvert.SerializeObject(jobId), UserSession?.Token);
            if (returnModel != null)
            {
                return Json(ReturnHelper.AjaxResponse(returnModel.Success, returnModel.Message, string.Empty));
            }
            return Json(ReturnHelper.GetSystemErrorAjaxResponse());
        }
    }
}
