namespace Lykke.Service.PayInvoice.Client.Models.Employee
{
    /// <summary>
    /// Merchant employee creation details.
    /// </summary>
    public class CreateEmployeeModel
    {
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
    }
}