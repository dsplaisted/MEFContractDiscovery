using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ContractDiscoveryEditorClassifier
{
    internal static class ContractDiscoveryEditorClassifierClassificationDefinition
    {
        /// <summary>
        /// Defines the "ContractDiscoveryEditorClassifier" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ContractDiscoveryEditorClassifier")]
        internal static ClassificationTypeDefinition ContractDiscoveryEditorClassifierType = null;
    }
}
