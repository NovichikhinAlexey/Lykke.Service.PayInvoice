﻿using System;
using Lykke.Service.PayInvoice.Core.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class InvoiceDetailsModel
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public InvoiceStatus Status { get; set; }
        public string SettlementAssetId { get; set; }
        public string PaymentAssetId { get; set; }
        public string PaymentRequestId { get; set; }
        public string MerchantId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OrderId { get; set; }
        public string WalletAddress { get; set; }
        public double PaymentAmount { get; set; }
        public DateTime OrderDueDate { get; set; }
        public DateTime OrderCreatedDate { get; set; }
    }
}
