namespace Lykke.Service.PayInvoice.Core.Domain
{
    public interface IEmployee
    {
        string Id { get; set; }

        string Email { get; set; }
        
        string FirstName { get; set; }
        
        string LastName { get; set; }
        
        string MerchantId { get; set; }
    }
}