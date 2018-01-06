using System;

namespace Lykke.Pay.Service.Invoces.Client
{
    public class ErrorResponseException : Exception
    {
        public ErrorResponseException()
        {
        }

        public ErrorResponseException(string message)
            : base(message)
        {
        }

        public ErrorResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
