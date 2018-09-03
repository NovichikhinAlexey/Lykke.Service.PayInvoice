using System.Collections.Generic;
using Autofac;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.RabbitMq;
using Lykke.Service.PayInvoice.Contract;
using Lykke.Service.PayInvoice.Contract.Commands;
using Lykke.Service.PayInvoice.Contract.Events;
using Lykke.Service.PayInvoice.Settings.ServiceSettings;
using Lykke.Service.PayInvoice.Workflow.CommandHandlers;
using Lykke.SettingsReader;

namespace Lykke.Service.PayInvoice.Modules
{
    public class CqrsModule : Module
    {
        private readonly IReloadingManager<CqrsSettings> _settings;

        private const string CommandsRoute = "commands";
        private const string EventsRoute = "events";

        public CqrsModule(IReloadingManager<CqrsSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterChaosKitty(builder);

            builder.Register(context => new AutofacDependencyResolver(context))
                .As<IDependencyResolver>()
                .SingleInstance();

            var rabbitSettings = new RabbitMQ.Client.ConnectionFactory
                {Uri = _settings.CurrentValue.RabbitMqConnectionString};

            builder.Register(ctx =>
            {
                var logFactory = ctx.Resolve<ILogFactory>();
                return new MessagingEngine(
                    logFactory,
                    new TransportResolver(new Dictionary<string, TransportInfo>
                    {
                        {
                            "RabbitMq",
                            new TransportInfo(
                                rabbitSettings.Endpoint.ToString(),
                                rabbitSettings.UserName,
                                rabbitSettings.Password,
                                "None", "RabbitMq")
                        }
                    }),
                    new RabbitMqTransportFactory(logFactory));
            });

            builder.RegisterType<RegisterEmployeeCommandHandler>();

            builder.RegisterType<UpdateEmployeeCommandHandler>();

            builder.Register(ctx => new CqrsEngine(
                    ctx.Resolve<ILogFactory>(),
                    ctx.Resolve<IDependencyResolver>(),
                    ctx.Resolve<MessagingEngine>(),
                    new DefaultEndpointProvider(),
                    true,
                    Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                        "RabbitMq",
                        Messaging.Serialization.SerializationFormat.ProtoBuf,
                        environment: "lykke")),

                    Register.BoundedContext("lykkepay-employee-registration")
                        .ListeningCommands(typeof(RegisterEmployeeCommand))
                        .On(CommandsRoute)
                        .WithCommandsHandler<RegisterEmployeeCommandHandler>()
                        .PublishingEvents(typeof(EmployeeRegisteredEvent), typeof(EmployeeRegistrationFailedEvent))
                        .With(EventsRoute)

                        .ListeningCommands(typeof(UpdateEmployeeCommand))
                        .On(CommandsRoute)
                        .WithCommandsHandler<UpdateEmployeeCommandHandler>()
                        .PublishingEvents(typeof(EmployeeUpdatedEvent), typeof(EmployeeUpdateFailedEvent))
                        .With(EventsRoute)
                ))
                .As<ICqrsEngine>()
                .SingleInstance()
                .AutoActivate();
        }

        private void RegisterChaosKitty(ContainerBuilder builder)
        {
            if (_settings.CurrentValue.ChaosKitty != null)
            {
                builder.RegisterType<ChaosKitty>()
                    .WithParameter(TypedParameter.From(_settings.CurrentValue.ChaosKitty.StateOfChaos))
                    .As<IChaosKitty>()
                    .SingleInstance();
            }
            else
            {
                builder.RegisterType<SilentChaosKitty>()
                    .As<IChaosKitty>()
                    .SingleInstance();
            }
        }
    }
}
