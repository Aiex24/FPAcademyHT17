using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PlaceholderYacht.Models.Entities;
using PlaceholderYacht.Models;
using System.Globalization;

namespace PlaceholderYacht
{
    public class Startup
    {
        IConfiguration conf;
        public Startup(IConfiguration conf)
        {
            this.conf = conf;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connstring = conf.GetConnectionString("connstring");
            services.AddMvc();
            services.AddDbContext<WindCatchersContext>(o =>
                o.UseSqlServer(connstring));
            services.AddDbContext<IdentityDbContext>(o =>
                o.UseSqlServer(connstring));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IBoatRepository, BoatDbRepository>();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            var cultureInfo = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            app.UseMvcWithDefaultRoute();
        }
    }
}
