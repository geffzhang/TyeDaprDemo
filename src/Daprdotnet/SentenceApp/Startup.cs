using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapr.Client;
using SentenceApp.Services;
using Newtonsoft.Json.Converters;

namespace SentenceApp
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
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddApplicationServices();

            /*

            Note: HttpClient setup is *only* required if you're using Tye to wire up your services.
            It's not required when using Dapr extensions as they now follow the Dapr service invocation uri convention which the Dapr client uses.

            services.AddHttpClient() is merely here because we have native Tye examples in each of the following service clients.
           

            services.AddHttpClient<UppercaseServiceClient>(client =>
                {
                    client.BaseAddress = Configuration.GetServiceUri("UppercaseService");
                });

            services.AddHttpClient<LowercaseServiceClient>(client =>
                {
                    client.BaseAddress = Configuration.GetServiceUri("LowercaseService");
                });

            services.AddHttpClient<TitlecaseServiceClient>(client =>
                {
                    client.BaseAddress = Configuration.GetServiceUri("TitlecaseService");
                });
            */
            services.AddControllers()
                .AddDapr()
                .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            //services.AddDaprClient(builder =>
            //    builder.UseJsonSerializationOptions(
            //        new JsonSerializerOptions()
            //        {
            //            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //            PropertyNameCaseInsensitive = true,
            //        }));
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(
                _ => new UppercaseServiceClient(DaprClient.CreateInvokeHttpClient("UppercaseService")));

            services.AddSingleton(
                _ => new LowercaseServiceClient(DaprClient.CreateInvokeHttpClient("LowercaseService")));

            services.AddSingleton(
                _ => new TitlecaseServiceClient(DaprClient.CreateInvokeHttpClient("TitlecaseService")));

            return services;
        }
    }
}
