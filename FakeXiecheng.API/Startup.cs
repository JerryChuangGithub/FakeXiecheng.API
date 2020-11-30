using System;
using AutoMapper;
using FakeXiecheng.API.Database;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace FakeXiecheng.API
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
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                // dotnet core 1.0
                // setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            })
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                // after that dotnet core 2.0
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "Validation",
                            Title = "資料驗證常敗",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "請看詳細說明",
                            Instance = context.HttpContext.Request.Path
                        };
                        problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}