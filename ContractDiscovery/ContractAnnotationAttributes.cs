using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContractDiscovery
{
    public class ContractInformationAttribute : Attribute
    {
        public string ContractIdentifier { get; set; }
        public Type CustomExportAttributeType { get; set; }
    }
}
