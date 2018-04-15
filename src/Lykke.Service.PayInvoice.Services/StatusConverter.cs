using System;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Services
{
    public static class StatusConverter
    {
        public static InvoiceStatus Convert(PaymentRequestStatus status, PaymentRequestProcessingError error)
        {
            switch (status)
            {
                case PaymentRequestStatus.New:
                    return InvoiceStatus.Unpaid;

                case PaymentRequestStatus.Cancelled:
                    return InvoiceStatus.Removed;

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
                        case PaymentRequestProcessingError.PaymentExpired:
                            return InvoiceStatus.LatePaid;
                        case PaymentRequestProcessingError.PaymentAmountBelow:
                            return InvoiceStatus.Underpaid;
                        case PaymentRequestProcessingError.PaymentAmountAbove:
                            return InvoiceStatus.Overpaid;
                        case PaymentRequestProcessingError.RefundNotConfirmed:
                            return InvoiceStatus.NotConfirmed;
                        //case PaymentRequestErrorType.InvalidAddress:
                        //    return InvoiceStatus.InvalidAddress;
                        case PaymentRequestProcessingError.UnknownPayment:
                        case PaymentRequestProcessingError.UnknownRefund:
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
