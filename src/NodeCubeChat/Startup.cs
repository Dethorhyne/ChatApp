using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodeCubeChat.Data;
using NodeCubeChat.Models;
using NodeCubeChat.Services;
using Microsoft.AspNetCore.Mvc;

namespace NodeCubeChat
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
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

#if DEBUG
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ChatContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("ChatDatabase")));
#else
            // Azure services
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AzureIdentity")));

            services.AddDbContext<ChatContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("AzureChatDB")));

#endif

            services.AddMvc();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            //app.UseFacebookAuthentication(new FacebookOptions()
            //{
            //    AppId = "1755620094679482",
            //    AppSecret = "548dcd9bc71af6a65c5defa507686997"
            //});

            //Baca error 500 zbog DataProtection-a, nema puno informacija o pogrešci no uputa za podešavanje praćena u potpunosti.
            //app.UseTwitterAuthentication(new TwitterOptions()
            //{
            //    ConsumerKey = "AlBHfK2YYPYkmTIZVgT9fQnrG",
            //    ConsumerSecret = "d9sxBecc7frIF2uyHhW71mCdA3pzxfpKk2Xm8SOMtrd44k9a4W"
            //});

            //Vraća access denied bez nekog dobrog razloga, podešeno kao na testnom projektu ali ovdje ne želi proć
            //app.UseGoogleAuthentication(new GoogleOptions()
            //{
            //    ClientId = "917128493679-minfopf66a6jgu1furskl0n1r497hpo5.apps.googleusercontent.com",
            //    ClientSecret = "XQLjP5Z86OSIhhmhzf75XYlv"

            //});


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{Id?}");
            });
        }
    }
}
