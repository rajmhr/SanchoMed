
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FutsalGoal.Model.SpModels;
using FutsalGoal.Components;
using System.Security.Claims;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using static HelperClass.EnumCollection;

namespace FutsalGoal.Controllers
{
    public class NavDisplayViewComponent : BaseViewComponent
    {
        public NavDisplayViewComponent(IHttpContextAccessor contextAccessor) : base(contextAccessor) { }
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
            return View(userModel);
        }
    }
}