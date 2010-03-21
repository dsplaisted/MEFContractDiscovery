using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;
using ContractDiscovery;

namespace ContractDiscoveryTests
{
    public static class Constants
    {
        public const string ContractIdentifier = "ContractDiscovery.PluginContract";
    }

    public interface IPlugin
    {

    }

    public interface IPluginMetadata
    {
        string Name { get; }
        [DefaultValue(0)]
        int Priority { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false)]
    public class ExportPluginAttribute : ExportAttribute, IPluginMetadata
    {
        public ExportPluginAttribute(string name)
            : base(typeof(IPlugin))
        {
            
        }

        public string Name { get; private set; }
        public int Priority { get; set; }
    }

    [Export]
    public class Part
    {
        [ContractInformation(ContractIdentifier = Constants.ContractIdentifier, CustomExportAttributeType = typeof(ExportPluginAttribute))]
        [ImportMany]
        public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins { get; set; }
    }
}
