using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using ContractDiscovery;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;

namespace ContractDiscoveryTests
{
 

    [TestClass]
    public class ContractDiscoveryTests
    {
        [TestMethod]
        public void TestPropertyImport()
        {
            var catalog = new TypeCatalog(typeof(Part));
            var discovery = new CatalogContractInformationProvider(catalog);

            IContractInformation contractInformation = discovery.GetContractInformation(Constants.ContractIdentifier);
            Assert.IsNotNull(contractInformation, "Contract information should be found");

            Assert.AreEqual(AttributedModelServices.GetContractName(typeof(IPlugin)), contractInformation.ContractName, "Contract Name");
            Assert.AreEqual(typeof(IPlugin), contractInformation.ContractType, "Contract Type");

            Assert.AreEqual(1, contractInformation.CustomExportAttributes.Count(), "Custom export attributes count");
            Assert.AreEqual(typeof(ExportPluginAttribute), contractInformation.CustomExportAttributes.Single(), "Custom export attribute");
        }
    }
}
