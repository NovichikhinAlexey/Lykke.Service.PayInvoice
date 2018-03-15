using System;
using Lykke.Service.PayInternal.Client.Models.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Services
{
    public static class StatusConverter
    {
        public static InvoiceStatus Convert(PayInternal.Contract.PaymentRequest.PaymentRequestStatus status, string error)
        {
            return Convert(Enum.Parse<PaymentRequestStatus>(status.ToString()), error);
        }
        
        public static InvoiceStatus Convert(PaymentRequestStatus status, string error)
        {
            switch (status)
            {
                case PaymentRequestStatus.New:
                    return InvoiceStatus.Unpaid;

                //case PaymentRequestStatus.Canceled:
                //    return InvoiceStatus.Removed;

                case PaymentRequestStatus.InProcess:
                    return InvoiceStatus.InProgress;

                case PaymentRequestStatus.Confirmed:
                    return InvoiceStatus.Paid;

                case PaymentRequestStatus.RefundInProgress:
                    return InvoiceStatus.RefundInProgress;

                case PaymentRequestStatus.Refunded:
                    return InvoiceStatus.Refunded;

                //case PaymentRequestStatus.SettlementInProgress:
                //    return InvoiceStatus.SettlementInProgress;

                //case PaymentRequestStatus.Settled:
                //    return InvoiceStatus.Settled;

                case PaymentRequestStatus.Error:
                    switch (error)
                    {
                        case "EXPIRED":
                            return InvoiceStatus.LatePaid;
                        case "AMOUNT BELOW":
                            return InvoiceStatus.Underpaid;
                        case "AMOUNT ABOVE":
                            return InvoiceStatus.Overpaid;
                        case "NOT CONFIRMED":
                            return InvoiceStatus.NotConfirmed;
                        case "INVALID ADDRESS":
                            return InvoiceStatus.InvalidAddress;
                        case "INTERNAL ERROR":
                            return InvoiceStatus.InternalError;
                        default:
                            throw new Exception($"Unknown payment request error '{error}'");
                    }
                default:
                    throw new Exception($"Unknown payment request status '{status}'");
            }
        }
    }
}
