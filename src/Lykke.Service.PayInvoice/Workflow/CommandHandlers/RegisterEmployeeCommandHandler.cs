using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Service.PayInvoice.Contract.Commands;
using Lykke.Service.PayInvoice.Contract.Events;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Extensions;
using Lykke.Service.PayInvoice.Core.Services;

namespace Lykke.Service.PayInvoice.Workflow.CommandHandlers
{
    [UsedImplicitly]
    public class RegisterEmployeeCommandHandler
    {
        private readonly IEmployeeService _employeeService;
        private readonly IChaosKitty _chaosKitty;
        private readonly ILog _log;

        public RegisterEmployeeCommandHandler(
            [NotNull] IEmployeeService employeeService,
            [NotNull] ILogFactory logFactory,
            [NotNull] IChaosKitty chaosKitty)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _chaosKitty = chaosKitty ?? throw new ArgumentNullException(nameof(chaosKitty));
            _log = logFactory.CreateLog(this);
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterEmployeeCommand command, IEventPublisher publisher)
        {
            Employee employee;

            try
            {
                employee = await _employeeService.AddAsync(Mapper.Map<Employee>(command));
            }
            catch (Exception e)
            {
                if (e is MerchantNotFoundException merchantEx)
                    _log.WarningWithDetails(merchantEx.Message, new {merchantEx.MerchantId});

                if (e is EmployeeExistException employeeEx)
                    _log.WarningWithDetails(employeeEx.Message, new { command.Email });

                publisher.PublishEvent(new EmployeeRegistrationFailedEvent
                {
                    Email = command.Email,
                    Error = e.Message
                });

                _chaosKitty.Meow("Issue with RabbitMq publishing EmployeeRegistrationFailedEvent");

                return CommandHandlingResult.Ok();
            }

            publisher.PublishEvent(new EmployeeRegisteredEvent
            {
                Id = employee.Id,
                Email = employee.Email,
                MerchantId = employee.MerchantId,
                Password = command.Password
            });

            _chaosKitty.Meow("Issue with RabbitMq publishing EmployeeRegisteredEvent");

            return CommandHandlingResult.Ok();
        }
    }
}
