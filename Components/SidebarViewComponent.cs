
using FutsalGoal.Components;
using FutsalGoal.Model.SpModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using static HelperClass.EnumCollection;

namespace FutsalGoal.Controllers
{
    public class SidebarViewComponent : BaseViewComponent
    {
        public SidebarViewComponent(IHttpContextAccessor contextAccessor) : base(contextAccessor) { }
        public override IViewComponentResult Invoke()
        {
            UserLoginSpModel userModel = null;
            var user = User as ClaimsPrincipal;
            if (user.Identity.IsAuthenticated)
            {
                var securityToken = new JwtSecurityToken(user.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.AccessToken.ToString())).Value);
                userModel = new UserLoginSpModel()
                {
                    DisplayName = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.DisplayName.ToString())).Value,
                    EmailId = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.EmailId.ToString())).Value,
                    UserRole = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.RoleName.ToString())).Value
                };
            }

            if (userModel == null)
            {
                return View(userModel);
            }
            else if (userModel.UserRole.Equals(HelperClass.EnumCollection.ConfigChoiceRole.User.ToString()))
            {
                return View("_userLeftMenu", userModel);
            }
            else if (userModel.UserRole.Equals(HelperClass.EnumCollection.ConfigChoiceRole.Admin.ToString()))
            {
                return View("_adminLeftMenu", userModel);
            }
            else if (userModel.UserRole.Equals(HelperClass.EnumCollection.ConfigChoiceRole.SuperAdministrator.ToString()))
            {
                return View("_superAdminLeftMenu", userModel);
            }
            return View(userModel);
        }
    }
}