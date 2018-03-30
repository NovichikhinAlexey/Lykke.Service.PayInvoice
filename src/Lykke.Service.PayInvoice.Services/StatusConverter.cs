using System;
using Lykke.Service.PayInternal.Client.Models.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Services
{
    public static class StatusConverter
    {
        public static InvoiceStatus Convert(
            PayInternal.Contract.PaymentRequest.PaymentRequestStatus status,
            PayInternal.Contract.PaymentRequest.PaymentRequestErrorType error)
        {
            return Convert((PaymentRequestStatus) status, (PaymentRequestErrorType) error);
        }
        
        public static InvoiceStatus Convert(PaymentRequestStatus status, PaymentRequestErrorType error)
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
                        case PaymentRequestErrorType.PaymentExpired:
                            return InvoiceStatus.LatePaid;
                        case PaymentRequestErrorType.PaymentAmountBelow:
                            return InvoiceStatus.Underpaid;
                        case PaymentRequestErrorType.PaymentAmountAbove:
                            return InvoiceStatus.Overpaid;
                        case PaymentRequestErrorType.RefundNotConfirmed:
                            return InvoiceStatus.NotConfirmed;
                        //case PaymentRequestErrorType.InvalidAddress:
                        //    return InvoiceStatus.InvalidAddress;
                        //case PaymentRequestErrorType.InternalError:
                        //    return InvoiceStatus.InternalError;
                        default:
                            throw new Exception($"Unknown payment request error '{error}'");
                    }
                default:
                    throw new Exception($"Unknown payment request status '{status}'");
            }
        }
    }
}
