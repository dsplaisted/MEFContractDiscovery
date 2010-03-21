using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;

namespace ContractDiscovery
{
    public interface IContractInformation
    {
        string ContractIdentifier { get; }

        string ContractName { get; }
        Type ContractType { get; }
        IEnumerable<IContractMetadataItem> MetadataInformation { get; }

        bool ForExporting { get; }
        bool ForImporting { get; }

        ImportCardinality ExpectedCardinality { get; }

        IEnumerable<Type> CustomExportAttributes { get; }
        IEnumerable<Type> MetadataViewTypes { get; }
        
        IEnumerable<string> RelatedContractIdentifiers { get; }

        string Description { get; }
        IEnumerable<string> Keywords { get; }
        IEnumerable<string> Templates { get; }
    }

    public interface IContractMetadataItem
    {
        string Key { get; }
        Type ValueType { get; }
        bool Optional { get; }
    }

    public interface IContractInformationProvider
    {
        IContractInformation GetContractInformation(string contractIdentifier);

        IEnumerable<string> GetAvailableContractIdentifiers();
    }

}
