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
    public class UpdateEmployeeCommandHandler
    {
        private readonly IEmployeeService _employeeService;
        private readonly IChaosKitty _chaosKitty;
        private readonly ILog _log;

        public UpdateEmployeeCommandHandler(
            [NotNull] IEmployeeService employeeService,
            [NotNull] ILogFactory logFactory, 
            [NotNull] IChaosKitty chaosKitty)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _chaosKitty = chaosKitty ?? throw new ArgumentNullException(nameof(chaosKitty));
            _log = logFactory.CreateLog(this);
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(UpdateEmployeeCommand command, IEventPublisher publisher)
        {
            var employee = Mapper.Map<Employee>(command);

            try
            {
                await _employeeService.UpdateAsync(employee);
            }
            catch (Exception e)
            {
                if (e is EmployeeNotFoundException notFoundEx)
                    _log.WarningWithDetails(notFoundEx.Message, new { notFoundEx.EmployeeId });

                if (e is EmployeeExistException existsEx)
                    _log.WarningWithDetails(existsEx.Message, new { command.Email });

                publisher.PublishEvent(new EmployeeUpdateFailedEvent
                {
                    Email = command.Email,
                    Error = e.Message
                });

                _chaosKitty.Meow("Issue with RabbitMq publishing EmployeeUpdateFailedEvent");

                return CommandHandlingResult.Ok();
            }

            publisher.PublishEvent(new EmployeeUpdatedEvent
            {
                Id = employee.Id,
                Email = employee.Email,
                MerchantId = employee.MerchantId,
                Password = command.Password
            });

            _chaosKitty.Meow("Issue with RabbitMq publishing EmployeeUpdatedEvent");

            return CommandHandlingResult.Ok();
        }
    }
}
