using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Log;
using Lykke.AzureStorage.Tables.Entity.Metamodel;
using Lykke.AzureStorage.Tables.Entity.Metamodel.Providers;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Settings;
using Lykke.Service.PayInvoice.Swagger;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Lykke.MonitoringServiceApiCaller;
using Lykke.Common.Log;
using Lykke.Service.PayInvoice.Modules;

namespace Lykke.Service.PayInvoice
{
    public class Startup
    {
        private string _monitoringServiceUrl;

        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }
        public ILog Log { get; private set; }
        private IHealthNotifier HealthNotifier { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver =
                            new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    });

                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new Info
                        {
                            Version = "v1",
                            Title = "Lykke.Service.PayInvoice API"
                        });

                    options.DescribeAllEnumsAsStrings();
                    options.EnableXmsEnumExtension();
                    options.EnableXmlDocumentation();

                    options.OperationFilter<FileUploadOperation>();
                });

                EntityMetamodel.Configure(new AnnotationsBasedMetamodelProvider());

                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfiles(typeof(AutoMapperProfile));
                    cfg.AddProfiles(typeof(Services.AutoMapperProfile));
                    cfg.AddProfiles(typeof(Repositories.AutoMapperProfile));
                });

                Mapper.AssertConfigurationIsValid();

                var builder = new ContainerBuilder();
                var appSettings = Configuration.LoadSettings<AppSettings>(options => {});
                if (appSettings.CurrentValue.MonitoringServiceClient != null)
                    _monitoringServiceUrl = appSettings.CurrentValue.MonitoringServiceClient.MonitoringServiceUrl;

                services.AddLykkeLogging(
                    appSettings.ConnectionString(x => x.PayInvoiceService.Db.LogsConnectionString),
                    "PayInvoiceLog",
                    appSettings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                    appSettings.CurrentValue.SlackNotifications.AzureQueue.QueueName);

                builder.RegisterModule(
                    new Repositories.AutofacModule(appSettings.Nested(o => o.PayInvoiceService.Db.DataConnectionString)));

                builder.RegisterModule(new Services.AutofacModule(
                    appSettings.CurrentValue.PayInvoiceService.CacheExpirationPeriods,
                    appSettings.CurrentValue.PayInvoiceService.DistributedCacheSettings,
                    appSettings.CurrentValue.PayInvoiceService.RetryPolicy));
                
                builder.RegisterModule(new AutofacModule(appSettings));

                builder.RegisterModule(new CqrsModule(appSettings.Nested(x => x.PayInvoiceService.Cqrs)));

                builder.Populate(services);
                ApplicationContainer = builder.Build();

                Log = ApplicationContainer.Resolve<ILogFactory>().CreateLog(this);

                HealthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                Log?.Critical(ex);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseLykkeMiddleware(ex => new
                {
                    Message = "Technical problem"
                });

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

                appLifetime.ApplicationStarted.Register(() => StartApplication().Wait());
                appLifetime.ApplicationStopping.Register(StopApplication);
                appLifetime.ApplicationStopped.Register(CleanUp);
            }
            catch (Exception ex)
            {
                Log?.Critical(ex);
                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                // NOTE: Service not yet recieve and process requests here

                ApplicationContainer.Resolve<IStartupManager>().Start();

                HealthNotifier?.Notify("Started");

                await AutoRegistrationInMonitoring.RegisterInMonitoringServiceAsync(Configuration, _monitoringServiceUrl, HealthNotifier);
            }
            catch (Exception ex)
            {
                Log?.Critical(ex);
                throw;
            }
        }

        private void StopApplication()
        {
            try
            {
                // NOTE: Service still can recieve and process requests here, so take care about it if you add logic here.

                ApplicationContainer.Resolve<IShutdownManager>().Stop();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Critical(ex);
                }

                throw;
            }
        }

        private void CleanUp()
        {
            try
            {
                // NOTE: Service can't recieve and process requests here, so you can destroy all resources

                HealthNotifier?.Notify("Terminating");

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Critical(ex);
                    (Log as IDisposable)?.Dispose();
                }

                throw;
            }
        }
    }
}
