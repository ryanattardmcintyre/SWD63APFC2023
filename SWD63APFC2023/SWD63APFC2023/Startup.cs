using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SWD63APFC2023.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63APFC2023
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
             
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
           @"C:\Users\attar\source\repos\SWD63APFC2023\SWD63APFC2023\SWD63APFC2023\swd63a2023-377009-5caf98b90066.json");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string projectId = Configuration["project"];
            string serviceName = Configuration["serviceName"];
            
            services.AddGoogleErrorReportingForAspNetCore(new  ErrorReportingServiceOptions
            {
                // Replace ProjectId with your Google Cloud Project ID.
                ProjectId = projectId,
                // Replace Service with a name or identifier for the service.
                ServiceName = serviceName,
                // Replace Version with a version for the service.
                Version = "1"
            });




            services.AddControllersWithViews();

            //oauth_secretkey

            // Create the client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName(projectId, "oauth_secretkey", "1");

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String secretKey = result.Payload.Data.ToStringUtf8();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId = "175243740817-op16co8e6t7mu9421aa10l50s5j5cmto.apps.googleusercontent.com";
                    options.ClientSecret = secretKey;
                });

            
        
            services.AddScoped(provider => new FirestoreBooksRepository(projectId));
            services.AddScoped(provider => new FirestoreReservationsRepository(projectId));
            services.AddScoped<CacheMenusRepository>(provider => new CacheMenusRepository(
                "127.0.0.1:6379"
                ));

            services.AddScoped<PubsubEmailsRepository>(provider => new PubsubEmailsRepository(projectId));

            //string connectionStringRedisLabs = "redis-14410.c1.us-east1-2.gce.cloud.redislabs.com:14410,password=";

            //string projectId = builder.Configuration["project"];
            //builder.Services.AddScoped<....>

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

 


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
