using Gp_Api.Helpers;
using Gp_Api.Hubs;
using Gp_Api.IServices;
using Gp_Api.Services;
using Gp_Api.Tools.Converters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PMS_api.Services;
using SEPV_Api.Models.PMS;
using System;
using System.Text;

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
            services.AddTransient<ICodeLookupService, CodeLookupService>();
            services.AddTransient<ICodeLookupSourceService, CodeLookupSourceService>();
            services.AddTransient<ILoginInfoService, LoginInfoService>();
            services.AddTransient<ILoginRolesService, LoginRolesService>();
            services.AddTransient<ILoginMenusService, LoginMenusService>();
            services.AddTransient<ICustomerPlmService, CustomerPlmService>();
            services.AddTransient<IProjectPlmService, ProjectPlmService>();
            services.AddTransient<ICustomer2CheckService, Customer2CheckService>();
            services.AddTransient<IProject2CheckService, Project2CheckService>();
            services.AddTransient<IWeeklyReportPlmService, WeeklyReportPlmService>();
            services.AddTransient<ICheckinLogsService, CheckinLogsService>();
            services.AddTransient<IDepartmentsService, DepartmentsService>();
            services.AddTransient<ILeaveApplicationsService, LeaveApplicationsService>();
            services.AddTransient<IProjectInternalService, ProjectInternalService>();
            services.AddTransient<IProjectSvcService, ProjectSvcService>();
            services.AddTransient<ITaskMasterService, TaskMasterService>();
            services.AddTransient<IFileProcessorService, FileProcessorService>();
            services.AddTransient<IExpenseClaimsService, ExpenseClaimsService>();
            services.AddTransient<IProjectFirmService, ProjectFirmService>();
            // services.AddScoped

            services.AddDbContext<PMSContext>(option => option.UseSqlServer(Configuration.GetConnectionString(nameof(PMSContext))));

            services.AddSingleton<JwtHelpers>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   // �����ҥ��ѮɡA�^�����Y�|�]�t WWW-Authenticate ���Y�A�o�̷|��ܥ��Ѫ��Բӿ��~��]
                   options.IncludeErrorDetails = true; // �w�]�Ȭ� true�A���ɷ|�S�O����

                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       // �z�L�o���ŧi�A�N�i�H�q "sub" ���Ȩó]�w�� User.Identity.Name
                       NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                       // �z�L�o���ŧi�A�N�i�H�q "roles" ���ȡA�åi�� [Authorize] �P�_����
                       RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                       // �@��ڭ̳��|���� Issuer
                       ValidateIssuer = true,
                       ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),

                       // �q�`���ӻݭn���� Audience
                       ValidateAudience = false,
                       //ValidAudience = "JwtAuthDemo", // �����ҴN���ݭn��g

                       // �@��ڭ̳��|���� Token �����Ĵ���
                       ValidateLifetime = true,

                       // �p�G Token ���]�t key �~�ݭn���ҡA�@�볣�u��ñ���Ӥw
                       ValidateIssuerSigningKey = false,

                       // "1234567890123456" ���ӱq IConfiguration ���o
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
