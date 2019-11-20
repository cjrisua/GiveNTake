using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GiveNTake.Model;
//using MySql.Data.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GiveNTake
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            services.AddControllersWithViews();
            services.AddDbContextPool<GiveNTakeContext>(
                    options =>
                    options.UseMySql(Configuration["ConnectionStrings:DefaultConnection"],
                    mySqlOptions =>
                    {
                        mySqlOptions.ServerVersion(new Version(8, 0, 17), ServerType.MySql); // replace with your Server Version and Type
                    }
            ));
            //Enabling the identity infrastructure
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<GiveNTakeContext>()
                .AddDefaultTokenProviders();

            //JASON Web Token Config
            services.AddAuthentication(
                    option =>
                    {
                        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }).
                AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JWTConfiguration:Issuer"],
                        ValidAudience = Configuration["JWTConfiguration:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTConfiguration:SigningKey"]))
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Enabling the identity infrastructure
            app.UseAuthentication();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints
                .MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
