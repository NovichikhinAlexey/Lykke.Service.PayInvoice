using System;
using System.Runtime.Serialization;

namespace Lykke.Pay.Service.Invoces.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when requested invoice cannot be found.
    /// </summary>
    [Serializable]
    public class InvoiceNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceNotFoundException"/> class.
        /// </summary>
        public InvoiceNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceNotFoundException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected InvoiceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceNotFoundException"/> class with a specified invoice id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        public InvoiceNotFoundException(string invoiceId)
            : base("Invoice not found")
        {
            InvoiceId = invoiceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<c>Nothing</c> in Visual Basic) if no inner exception is specified.</param>
        public InvoiceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets or sets a invoice id.
        /// </summary>
        public string InvoiceId { get; }
    }
}
