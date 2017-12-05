using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace PlaceholderYacht
{
    public class Startup
    {
        //IConfiguration conf;
        //public Startup(IConfiguration conf)
        //{
        //    this.conf = conf;
        //}

        public void ConfigureServices(IServiceCollection services)
        {
            //string connstring = conf.GetConnectionString("connstring");
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
