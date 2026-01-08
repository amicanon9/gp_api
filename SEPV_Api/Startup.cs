using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SEPV_Api.Models.GreenPower;

using Gp_Api.Helpers;
using Gp_Api.IServices;
using Gp_Api.Services;
using System.Text;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.Authorization;
using System;
using Gp_Api.Tools.Converters;
using Gp_Api.Hubs;
using static Gp_Api.Services.res;

namespace SEPV_Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson();
            //  allow CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://www.sepv.com.tw", "http://localhost:4200", "https://10.1.3.16")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            services.AddSignalR();

            // Dependency Injection
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ISetOfBooksService, SetOfBooksService>();
            services.AddTransient<IServiceNoInfoService, ServiceNoInfoService>();
            services.AddTransient<IServiceNoDetailInfoService, ServiceNoDetailInfoService>();
            services.AddTransient<IServiceNoDetailDataService, ServiceNoDetailDataService>();
            services.AddTransient<IPsbasicInfoService, PsbasicInfoService>();
            services.AddTransient<IPssurplusInfoService, PssurplusInfoService>();
            services.AddTransient<IPssurplusDataService, PssurplusDataService>();
            services.AddTransient<IPpbasicInfoService, PpbasicInfoService>();
            services.AddTransient<IPspowerNoInfoService, PspowerNoInfoService>();
            services.AddTransient<IPpmeterNoInfoService, PpmeterNoInfoService>();
            services.AddTransient<IPppowerNoInfoService, PppowerNoInfoService>();
            services.AddTransient<IPsmeterNoInfoService, PsmeterNoInfoService>();
            services.AddTransient<ICodeLookupService, CodeLookupService>();
            services.AddTransient<ICodeLookupSourceService, CodeLookupSourceService>();
            services.AddTransient<ILoginInfoService, LoginInfoService>();
            services.AddTransient<ILoginRolesService, LoginRolesService>();
            services.AddTransient<ILoginMenusService, LoginMenusService>();
            services.AddTransient<IBankInfoService, BankInfoService>();
            services.AddTransient<IImportService, ImportService>();
            services.AddTransient<IBankBranchInfoService, BankBranchInfoService>();
            services.AddTransient<IPsbankDataService, PsbankDataService>();
            services.AddTransient<IProjectPlmService, ProjectPlmService>();
            // services.AddScoped

            services.AddDbContext<GreenPowerContext>(option => option.UseSqlServer(Configuration.GetConnectionString(nameof(GreenPowerContext))));

            services.AddSingleton<JwtHelpers>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                   options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                       NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                       // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                       RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                       // 一般我們都會驗證 Issuer
                       ValidateIssuer = true,
                       ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),

                       // 通常不太需要驗證 Audience
                       ValidateAudience = false,
                       //ValidAudience = "JwtAuthDemo", // 不驗證就不需要填寫

                       // 一般我們都會驗證 Token 的有效期間
                       ValidateLifetime = true,

                       // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                       ValidateIssuerSigningKey = false,

                       // "1234567890123456" 應該從 IConfiguration 取得
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSettings:SignKey")))
                   };
               });
        }

        private int CustomersImportService()
        {
            throw new NotImplementedException();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/api/chatHub");
                endpoints.MapControllers();
            });
        }
    }
}
