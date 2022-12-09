using FutsalGoal.Model.Models;
using FutsalGoal.Filters;
using FutsalGoal.Utility;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FutsalGoal
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddCors(o => o.AddPolicy("AllowMyOrigin", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddMvc().AddRazorRuntimeCompilation();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<ConfigApiValues>(Configuration.GetSection("ConfigValues"));
            services.AddMvc(option =>
            {
                //option.Filters.Add(typeof(ActionFilter));
                option.Filters.Add(typeof(ActionFilter));
            }).AddViewOptions(a => a.HtmlHelperOptions.ClientValidationEnabled = true);

            var keysFolder = Path.Combine(_environment.ContentRootPath, "temp-keys");

            services.AddDataProtection()
                .SetApplicationName("medical_sancho_1010")
                .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

            var configuration = Configuration.GetSection("AuthValues");
            var authValues = configuration.Get<AuthValues>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie(x =>
                {
                    x.SlidingExpiration = true;
                    x.LoginPath = "/User/Login";
                    x.AccessDeniedPath = "/User/Login";
                })
              .AddJwtBearer("Bearer", options =>
              {
                  options.RequireHttpsMetadata = false;
                  // name of the API resource
                  options.Audience = authValues.Audience;
                  options.Authority = authValues.Authority;
                  options.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidIssuer = authValues.ValidIssuer,
                      ValidateAudience = false,
                      ValidateIssuer = false,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authValues.SecretKey)),
                      ValidateLifetime = true,
                      ClockSkew = TimeSpan.Zero,
                  };
                  options.Events = new JwtBearerEvents
                  {
                      OnAuthenticationFailed = context =>
                      {
                          if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                          {
                              context.Response.Headers.Add("Token-Expired", "true");
                          }
                          return Task.CompletedTask;
                      }
                  };
              });

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            services.Configure<ConfigApiValues>(Configuration.GetSection("ConfigValues"));
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {

            app.UseCors("AllowMyOrigin");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<ValidateAntiForgeryTokenMiddleware>();

            app.Use(next => context =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append(
                    "XSRF-TOKEN",
                    tokens.RequestToken,
                    new CookieOptions() { HttpOnly = false }
                );

                return next(context);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=User}/{action=Index}");
            });
        }
    }
}
