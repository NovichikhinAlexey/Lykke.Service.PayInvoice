using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Pay.Service.Invoces.Core;
using Lykke.Pay.Service.Invoces.DependencyInjection;
using Lykke.Pay.Service.Invoces.Models;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;

namespace Lykke.Pay.Service.Invoces
{
    [UsedImplicitly]
    public class Startup
    {
        private IContainer ApplicationContainer { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration("v1", "Invoices service");
            });

            var settings = HttpSettingsLoader.Load<ApplicationSettings>();
            var log = CreateLog(services, settings);
            var builder = new ContainerBuilder();

            builder.RegisterModule(new ApiModule(settings, log));
            builder.Populate(services);

            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        private static ILog CreateLog(IServiceCollection services, ApplicationSettings settings)
        {
            var appSettings = settings.InvoicesService;

            LykkeLogToAzureStorage logToAzureStorage = null;
            var logToConsole = new LogToConsole();
            var logAggregate = new LogAggregate();

            logAggregate.AddLogger(logToConsole);

            if (!string.IsNullOrEmpty(appSettings.Logs.DbConnectionString) &&
                !(appSettings.Logs.DbConnectionString.StartsWith("${") && appSettings.Logs.DbConnectionString.EndsWith("}")))
            {
                logToAzureStorage = new LykkeLogToAzureStorage(Constants.ComponentName,new LykkeLogToAzureStoragePersistenceManager(
                    AzureTableStorage<LogEntity>.Create(new StringSettingsManager(appSettings.Logs.DbConnectionString), "AssetsServiceLogs", null)));

                logAggregate.AddLogger(logToAzureStorage);
            }

            var log = logAggregate.CreateLogger();

           
            return log;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //app.UseLykkeMiddleware(Constants.ComponentName, ex => ErrorResponse.Create("Technical problem"));

            app.UseMvc();

            app.UseSwagger(c =>

            {

                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);

            });

            app.UseSwaggerUI(x =>

            {

                x.RoutePrefix = "swagger/ui";

                x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

            });

            app.UseStaticFiles();
        }
    }
}