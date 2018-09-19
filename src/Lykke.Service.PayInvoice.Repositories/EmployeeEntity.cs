using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class EmployeeEntity : TableEntity
    {
        public EmployeeEntity()
        {
        }

        public EmployeeEntity(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }
        
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MerchantId { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsInternalSupervisor { get; set; }
        public bool IsDeleted { get; set; }
    }
}
