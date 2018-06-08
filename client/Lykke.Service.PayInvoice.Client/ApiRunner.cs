using System;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Api.Contract.Responses;
using Refit;

namespace Lykke.Service.PayInvoice.Client
{
    internal class ApiRunner
    {
        public async Task RunAsync(Func<Task> method)
        {
            try
            {
                await method();
            }
            catch (ApiException ex)
            {
                ThrowException(ex);
            }
        }

        public async Task<T> RunAsync<T>(Func<Task<T>> method)
        {
            try
            {
                return await method();
            }
            catch (ApiException ex)
            {
                ThrowException(ex);
                return default(T);
            }
        }

        private void ThrowException(ApiException ex)
        {
            if (ex.HasContent)
            {
                throw new ErrorResponseException(GetErrorResponse(ex), ex);
            }
            else
            {
                throw new ErrorResponseException(null, ex);
            }
        }

        private static ErrorResponse GetErrorResponse(ApiException ex)
        {
            ErrorResponse errorResponse;

            try
            {
                errorResponse = ex.GetContentAs<ErrorResponse>();
            }
            catch (Exception)
            {
                errorResponse = null;
            }

            return errorResponse;
        }
    }
}
