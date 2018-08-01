using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when requested employee cannot be found.
    /// </summary>
    [Serializable]
    public class EmployeeNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNotFoundException"/> class.
        /// </summary>
        public EmployeeNotFoundException() : base("Employee not found")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNotFoundException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected EmployeeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNotFoundException"/> class with a specified invoice id.
        /// </summary>
        /// <param name="employeeId">The employee id.</param>
        public EmployeeNotFoundException(string employeeId)
            : base("Employee not found")
        {
            EmployeeId = employeeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<c>Nothing</c> in Visual Basic) if no inner exception is specified.</param>
        public EmployeeNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets or sets a employee id.
        /// </summary>
        public string EmployeeId { get; }
    }
}
