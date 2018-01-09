using AutoMapper;
using Lykke.Pay.Service.Invoces.Models.Invoice;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lykke.Pay.Service.Invoces.Utils;

namespace Lykke.Pay.Service.Invoices.Tests
{
    [TestClass]
    public class InvoiceControllerTests
    {
        [TestMethod]
        public void AutoMapper_OK()
        {
            var context = new UpdateDraftInvoiceModel
            {
                Currency = "a",
                Amount = 10,
                ClientEmail = "a@a.com",
                ClientName = "c",
                DueDate = "d",
                InvoiceId = "v",
                InvoiceNumber = "n",
                MerchantId = "n"
            }.ToContext();

            Assert.IsTrue(true);
        }
    }
}
