using AutoMapper;
using Lykke.Service.PayInvoice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.PayInvoice.Tests
{
    [TestClass]
    public class TestAssembly
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();
        }
    }
}
