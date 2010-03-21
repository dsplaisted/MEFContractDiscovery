using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;

namespace ContractDiscovery
{
    public class CatalogContractInformationProvider : IContractInformationProvider
    {
        private ComposablePartCatalog _catalog;
        private List<CatalogContractInformation> _discoveredContracts;

        public CatalogContractInformationProvider(ComposablePartCatalog catalog)
        {
            _catalog = catalog;
            var changeableCatalog = _catalog as INotifyComposablePartCatalogChanged;
            if (changeableCatalog != null)
            {
                changeableCatalog.Changed +=new EventHandler<ComposablePartCatalogChangeEventArgs>(catalog_Changed);
            }
        }

        private List<CatalogContractInformation> DiscoveredContracts
        {
            get
            {
                if (_discoveredContracts == null)
                {
                    _discoveredContracts = DiscoverContracts();
                }
                return _discoveredContracts;
            }
        }

        public IContractInformation GetContractInformation(string contractIdentifier)
        {
            var results = DiscoveredContracts.Where(c => c.ContractInformation.ContractIdentifier == contractIdentifier).Select(c => c.ContractInformation).ToArray();

            if (results.Length == 0)
            {
                //  No matching contracts
                return null;
            }
            else if (results.Length == 1)
            {
                return results[0];
            }
            else
            {
                throw new InvalidOperationException("Multiple contracts matching identifier: " + contractIdentifier);
            }
        }

        public IEnumerable<string> GetAvailableContractIdentifiers()
        {
            return DiscoveredContracts.Select(c => c.ContractInformation.ContractIdentifier);
        }

        void catalog_Changed(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            _discoveredContracts = null;
        }

        List<CatalogContractInformation> DiscoverContracts()
        {
            List<CatalogContractInformation> ret = new List<CatalogContractInformation>();

            foreach (var part in _catalog.Parts)
            {
                foreach (ImportDefinition import in part.ImportDefinitions)
                {
                    var contractInformation = new CatalogContractInformation();
                    contractInformation.PartDefinition = part;
                    contractInformation.ImportDefinition = import;

                    contractInformation.ContractInformation.ContractName = import.ContractName;
                    contractInformation.ContractInformation.ForExporting = true;
                    contractInformation.ContractInformation.ExpectedCardinality = import.Cardinality;

                    var contractBasedImport = import as ContractBasedImportDefinition;
                    if (contractBasedImport != null)
                    {
                        foreach (var kvp in contractBasedImport.RequiredMetadata)
                        {
                            ContractMedatadataItem metadataItem = new ContractMedatadataItem();
                            metadataItem.Key = kvp.Key;
                            metadataItem.ValueType = kvp.Value;
                            metadataItem.Optional = false;
                        }
                    }

                    MemberInfo importingMember = TryGetImportingMemberType(import);
                    if (importingMember != null)
                    {
                        Type targetType = GetImportingMemberType(importingMember);

                        if (targetType != null)
                        {
                            Type contractType;
                            Type metadataViewType;
                            bool isImportMany = import.Cardinality == ImportCardinality.ZeroOrMore;
                            GetContractTypesFromType(targetType,  isImportMany, out contractType, out metadataViewType);

                            if (contractType != null)
                            {
                                //  TODO: contract type should come off of import attribute, if it is specified there
                                contractInformation.ContractInformation.ContractType = contractType;
                            }
                            if (metadataViewType != null)
                            {
                                contractInformation.ContractInformation.MetadataViewTypes = new[] { metadataViewType };
                                //  TODO: get optional metadata value information from strongly typed view interface
                            }
                        }

                        var contractAttribute = importingMember.GetCustomAttributes(typeof(ContractInformationAttribute), false).OfType<ContractInformationAttribute>().FirstOrDefault();
                        if (contractAttribute != null)
                        {
                            contractInformation.ContractInformation.ContractIdentifier = contractAttribute.ContractIdentifier;
                            contractInformation.ContractInformation.CustomExportAttributes = new[] { contractAttribute.CustomExportAttributeType };
                        }


                    }

                    ret.Add(contractInformation);
                }
            }


            return ret;
        }

        private static MemberInfo TryGetImportingMemberType(ImportDefinition import)
        {
            LazyMemberInfo lazyMemberInfo;
            try
            {
                //  TODO: add support for constructor arguments (GetImportingParameter())
                lazyMemberInfo = ReflectionModelServices.GetImportingMember(import);
            }
            catch (ArgumentException)
            {
                return null;
            }
            var accessor = lazyMemberInfo.GetAccessors()[0];
            if (accessor == null)
            {
                //  For properties, accessor[0] is the get method, and accessor[1] is the set method.  So if the get method is null, use the set method
                accessor = lazyMemberInfo.GetAccessors()[1];
            }
            if (lazyMemberInfo.MemberType == MemberTypes.Field)
            {
                //return ((FieldInfo)accessor).FieldType;
                return accessor;
            }
            else if (lazyMemberInfo.MemberType == MemberTypes.Property)
            {
                //return ((MethodInfo)accessor).ReturnType;
                //  The accessor is for the get/set method, so we have to find a property that uses that method
                foreach (var property in accessor.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (property.GetGetMethod(true) == accessor || property.GetSetMethod(true) == accessor)
                    {
                        return property;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        private static Type GetImportingMemberType(MemberInfo importingMember)
        {

            Type targetType = null;
            if (importingMember.MemberType == MemberTypes.Field)
            {
                targetType = ((FieldInfo)importingMember).FieldType;
            }
            else if (importingMember.MemberType == MemberTypes.Property)
            {
                targetType = ((PropertyInfo)importingMember).PropertyType;
            }

            return targetType;
        }

        private static void GetContractTypesFromType(Type type, bool isImportMany, out Type contractType, out Type metadataViewType)
        {
            metadataViewType = null;
            Type targetType = type;

            if (isImportMany)
            {
                Type elementType = GetEnumerableElementType(targetType);
                if (elementType != null)
                {
                    targetType = elementType;
                }
            }

            if (targetType.IsGenericType)
            {
                Type genericType = targetType.GetGenericTypeDefinition();
                Type[] arguments = targetType.GetGenericArguments();
                if (genericType == typeof(Lazy<>))
                {
                    contractType = arguments[0];
                }
                else if (genericType == typeof(Lazy<,>))
                {
                    contractType = arguments[0];
                    metadataViewType = arguments[1];
                }
                //  TODO: Support for PartCreator
                else
                {
                    contractType = targetType;
                }
            }
            else
            {
                contractType = targetType;
            }
        }

        private static Type GetEnumerableElementType(Type type)
        {
            if (type == typeof(string) || !typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }

            Type closedType;
            if (TryGetGenericInterfaceType(type, typeof(IEnumerable<>), out closedType))
            {
                return closedType.GetGenericArguments()[0];
            }

            return null;
        }

        private static bool TryGetGenericInterfaceType(Type instanceType, Type targetOpenInterfaceType, out Type targetClosedInterfaceType)
        {
            // The interface must be open
            //Assumes.IsTrue(targetOpenInterfaceType.IsInterface);
            //Assumes.IsTrue(targetOpenInterfaceType.IsGenericTypeDefinition);
            //Assumes.IsTrue(!instanceType.IsGenericTypeDefinition);

            // if instanceType is an interface, we must first check it directly
            if (instanceType.IsInterface &&
                instanceType.IsGenericType &&
                instanceType.GetGenericTypeDefinition() == targetOpenInterfaceType)
            {
                targetClosedInterfaceType = instanceType;
                return true;
            }

            try
            {
                // Purposefully not using FullName here because it results in a significantly
                //  more expensive implementation of GetInterface, this does mean that we're
                //  takign the chance that there aren't too many types which implement multiple
                //  interfaces by the same name...
                Type targetInterface = instanceType.GetInterface(targetOpenInterfaceType.Name, false);
                if (targetInterface != null &&
                    targetInterface.GetGenericTypeDefinition() == targetOpenInterfaceType)
                {
                    targetClosedInterfaceType = targetInterface;
                    return true;
                }
            }
            catch (AmbiguousMatchException)
            {
                // If there are multiple with the same name we should not pick any
            }

            targetClosedInterfaceType = null;
            return false;
        }

    }

    class CatalogContractInformation
    {
        public ContractInformation ContractInformation { get; set; }
        public ComposablePartDefinition PartDefinition { get; set; }
        public ImportDefinition ImportDefinition { get; set; }

        public CatalogContractInformation()
        {
            ContractInformation = new ContractInformation();
        }
    }

    public class ContractInformation : IContractInformation
    {
        public string ContractIdentifier{ get; set; }

        public string ContractName { get; set; }
        public Type ContractType { get; set; }
        public IEnumerable<IContractMetadataItem> MetadataInformation { get; set; }

        public bool ForExporting { get; set; }
        public bool ForImporting { get; set; }

        public ImportCardinality ExpectedCardinality { get; set; }
        
        public IEnumerable<Type> CustomExportAttributes { get; set; }
        public IEnumerable<Type> MetadataViewTypes { get; set; }

        public IEnumerable<string> RelatedContractIdentifiers { get; set; }

        public string Description { get; set; }
        public IEnumerable<string> Keywords { get; set; }
        public IEnumerable<string> Templates { get; set; }

        public ContractInformation()
        {
            ContractType = typeof(object);
            MetadataInformation = Enumerable.Empty<IContractMetadataItem>();
            ExpectedCardinality = ImportCardinality.ZeroOrMore;
            CustomExportAttributes = Enumerable.Empty<Type>();
            MetadataViewTypes = Enumerable.Empty<Type>();
            RelatedContractIdentifiers = Enumerable.Empty<string>();

            Description = string.Empty;
            Keywords = Enumerable.Empty<string>();
            Templates = Enumerable.Empty<string>();
            
        }
    }


    public class ContractMedatadataItem : IContractMetadataItem
    {
        public string Key { get; set; }
        public Type ValueType { get; set; }
        public bool Optional { get; set; }
    }


}
