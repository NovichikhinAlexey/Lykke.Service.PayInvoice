using System;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayInvoice.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Service.PayInvoice.Validation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new ErrorResponse().AddErrors(context.ModelState));
            }
        }
    }
}
