﻿using System;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    public class CreateInvoiceModel
    {
        public string Number { get; set; }
        public double Amount { get; set; }
        public string SettlementAssetId { get; set; }
        public string PaymentAssetId { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string EmployeeId { get; set; }
        public DateTime DueDate { get; set; }
    }
}
