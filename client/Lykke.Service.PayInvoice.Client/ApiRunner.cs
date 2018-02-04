using System;
using System.Threading.Tasks;
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
            catch (ApiException exception)
            {
                throw new ErrorResponseException(GetErrorResponse(exception), exception);
            }
        }

        public async Task<T> RunAsync<T>(Func<Task<T>> method)
        {
            try
            {
                return await method();
            }
            catch (ApiException exception)
            {
                throw new ErrorResponseException(GetErrorResponse(exception), exception);
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

            return errorResponse ?? ErrorResponse.Create("AssetDisclaimers API is not specify the error response");
        }
    }
}
