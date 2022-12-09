using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FutsalGoal.Model.SpModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using static HelperClass.EnumCollection;

namespace FutsalGoal.Controllers
{
    public class BaseController : Controller
    {
        /*  public UserLoginSpModel UserSession
          {
              get
              {
                  if (HttpContext == null)
                      return null;
                  var strSession = HttpContext.Session.GetString("UserSession");
                  if (strSession == null)
                      return null;
                  return Newtonsoft.Json.JsonConvert.DeserializeObject<UserLoginSpModel>(strSession);
              }
              set
              {
                  if (value == null)
                      HttpContext.Session.Clear();
                  HttpContext.Session.SetString("UserSession", Newtonsoft.Json.JsonConvert.SerializeObject(value));
              }
          }*/

        public UserLoginSpModel UserSession
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return null;

                var accessToken = User.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.AccessToken.ToString())).Value;
                var securityToken = new JwtSecurityToken(accessToken);

                var userModel = new UserLoginSpModel()
                {
                    DisplayName = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.DisplayName.ToString())).Value,
                    EmailId = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.EmailId.ToString())).Value,
                    Token = accessToken,
                    UserRole = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.RoleName.ToString())).Value,
                    UserDetailId = Convert.ToInt32(securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.UserDetailId.ToString())).Value)
                };
                return userModel;
            }
        }
    }
}