namespace Lykke.Service.PayInvoice.Core.Utils
{
    public interface ICallbackUrlFormatter
    {
        string GetProgressUrl(string invoiceId);

        string GetSuccessUrl(string invoiceId);
        
        string GetErrorUrl(string invoiceId);
    }
}
