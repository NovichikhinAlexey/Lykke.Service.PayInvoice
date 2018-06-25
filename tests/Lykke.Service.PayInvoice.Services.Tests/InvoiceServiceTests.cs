using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.PayInvoice.Services.Tests
{
    [TestClass]
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        private readonly Mock<IFileInfoRepository> _fileInfoRepositoryMock = new Mock<IFileInfoRepository>();
        private readonly Mock<IFileRepository> _fileRepositoryMock = new Mock<IFileRepository>();
        private readonly Mock<IHistoryRepository> _historyRepositoryMock = new Mock<IHistoryRepository>();
        private readonly Mock<IPaymentRequestHistoryRepository> _paymentRequestHistoryRepository = new Mock<IPaymentRequestHistoryRepository>();
        private readonly Mock<IEmployeeRepository> _employeeRepository = new Mock<IEmployeeRepository>();
        private readonly Mock<IInvoiceDisputeRepository> _invoiceDisputeRepository = new Mock<IInvoiceDisputeRepository>();
        private readonly Mock<IPayInternalClient> _payInternalClientMock = new Mock<IPayInternalClient>();
        private readonly Mock<ILog> _logMock = new Mock<ILog>();

        private InvoiceService _service;

        [TestInitialize]
        public void TestInitialized()
        {
            _service = new InvoiceService(
                _invoiceRepositoryMock.Object,
                _fileInfoRepositoryMock.Object,
                _fileRepositoryMock.Object,
                _historyRepositoryMock.Object,
                _paymentRequestHistoryRepository.Object,
                _employeeRepository.Object,
                _invoiceDisputeRepository.Object,
                _payInternalClientMock.Object,
                _logMock.Object);
        }

        [TestMethod]
        public async Task UpdateDraftAsync_Throw_Exception_If_Invoice_Status_Is_Not_Draft()
        {
            // arrange
            var invoice = new Invoice {Status = InvoiceStatus.Unpaid};

            _invoiceRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(invoice));

            // act

            var task = _service.UpdateDraftAsync(invoice);

            // assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await task);
        }

        [TestMethod]
        public async Task UpdateDraftAsync_Properties_Status_CreatedDate_Not_Updated()
        {
            // arrange
            var sourceInvoice = new Invoice
            {
                Status = InvoiceStatus.Draft,
                CreatedDate = DateTime.Now.AddDays(-1)
            };

            var invoice = new Invoice
            {
                Status = InvoiceStatus.Paid,
                CreatedDate = DateTime.Now.AddDays(1)
            };

            Invoice updatedInvocie = null;

            _invoiceRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(sourceInvoice));

            _invoiceRepositoryMock.Setup(o => o.UpdateAsync(It.IsAny<Invoice>()))
                .Returns(Task.CompletedTask)
                .Callback((Invoice o) => { updatedInvocie = o; });

            // act

            await _service.UpdateDraftAsync(invoice);

            // assert
            Assert.IsTrue(sourceInvoice.Status == updatedInvocie.Status &&
                          sourceInvoice.CreatedDate == updatedInvocie.CreatedDate);
        }
    }
}
