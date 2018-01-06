using AutoMapper;
using Lykke.Pay.Service.Invoces.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Pay.Service.Invoices.Services.Tests
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
