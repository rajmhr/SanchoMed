using FutsalGoal.Model.SpModels;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using static HelperClass.EnumCollection;

namespace SKLib.Utility
{
    public class HttpHelper
    {
        public static UserLoginSpModel GetClaimsFromAccessToken(HttpContext context)
        {
            var token = context.User.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.AccessToken.ToString())).Value;
            if (string.IsNullOrEmpty(token))
                return null;

            var securityToken = new JwtSecurityToken(token);
            int.TryParse(securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.RoleId.ToString())).Value, out int roleId);
            var userModel = new UserLoginSpModel()
            {
                DisplayName = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.DisplayName.ToString())).Value,
                EmailId = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.EmailId.ToString())).Value,
                Token = token,
                UserRole = securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.RoleName.ToString())).Value,
                RoleId = roleId,
                UserDetailId = Convert.ToInt32(securityToken.Claims.FirstOrDefault(x => x.Type.Equals(MyClaimTypes.UserDetailId.ToString())).Value)
            };
            return userModel;
        }
    }
}
