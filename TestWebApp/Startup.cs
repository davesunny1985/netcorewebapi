using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestWebApp.Models;
using TestWebApp.Services;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using TestWebApp.Mappings;
using TestWebApp.Core;
using TestWebApp.Infrastructure;
using TestWebApp.Infrastructure.Interfaces;
using TestWebApp.Infrastructure.Implmentations;
using Microsoft.AspNetCore.Identity;
using TestWebApp.Infrastructure.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Options;

namespace TestWebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<TestWebContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),b=>b.MigrationsAssembly("TestWebApp")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<TestWebContext>()
                  //.AddUserStore<ApplicationUserStore>()
            //.AddRoleStore<ApplicationRoleStore>()
            //.AddUserManager<UserManager<ApplicationUser>>()
            //.AddRoleManager<ApplicationRoleManager>()
            .AddDefaultTokenProviders();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            //services.AddScoped<UserManager<ApplicationUser>>();

            // Automapper Configuration
            AutoMapperConfiguration.Configure();

            // Enable Cors
            services.AddCors();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<SeedIdentity>();
        }

        private static readonly string secretKey = "mysupersecret_secretkey!123";
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)//, SeedIdentity seeder)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseExceptionHandler(
              builder =>
              {
                  builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
              });
            // Add JWT generation endpoint:
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var options = new TokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };

            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options), app.ApplicationServices);
            app.UseMvc();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //   // routes.MapRoute("DefaultApi", "api/{controller}/{id?}");
            //});

            TestDbInitializer.Initialize(app.ApplicationServices);

            //var tasks = seeder.SeedUserRoles().ConfigureAwait(false);
            //seeder.SeedUserRoles("Sunny", "Dave", "davesunny1985", "davesunny1985@gmail.com", "6dkd!kitchen", "admin");
            //seeder.SeedUserRoles("Sunny2", "Dave2", "davesunny19852", "davesunny19852@gmail.com", "6dkd!kitchen2", "admin2");
            //seeder.SeedUserRoles("Sunny3", "Dave3", "davesunny19853", "davesunny19853@gmail.com", "6dkd!kitchen3", "admin");
        }
    }
}
