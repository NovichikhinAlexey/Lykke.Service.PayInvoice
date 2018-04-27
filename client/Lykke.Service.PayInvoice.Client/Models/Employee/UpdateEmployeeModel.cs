using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Client.Models.Employee
{
    public class UpdateEmployeeModel
    {
        /// <summary>
        /// Get or set Employee Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets an email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a last name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Get or set merchant id
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// Get or set blocked flag
        /// </summary>
        public bool IsBlocked { get; set; }
    }
}
