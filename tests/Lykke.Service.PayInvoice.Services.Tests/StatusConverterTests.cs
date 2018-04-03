using System;
using System.Linq;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.PayInvoice.Services.Tests
{
    [TestClass]
    public class StatusConverterTests
    {
        [TestMethod]
        public void Convert_PaymentRequest_Contract_Status_To_PaymentRequest_Model_Status_OK()
        {
            // arrange
            var error = PaymentRequestProcessingError.PaymentExpired;

            var values = Enum.GetValues(typeof(PaymentRequestStatus))
                .Cast<PaymentRequestStatus>()
                .Where(o => o != PaymentRequestStatus.None)
                .ToList();

            // act

            foreach (var value in values)
            {
                StatusConverter.Convert(value, error);
            }

            // assert
            Assert.IsTrue(true);
        }
    }
}
