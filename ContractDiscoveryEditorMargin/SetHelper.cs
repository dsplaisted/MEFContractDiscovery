using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.Language.Intellisense;


namespace ContractDiscoveryEditorMargin
{
    internal static class SetHelper
    {
        // All items with ids below this number have modifiers applied; above they directly map to StandardGlyphGroup
        private const int maxItemsWithModifiers = (int)StandardGlyphGroup.GlyphGroupError;

        // The ids have an unusual mapping to StandardGlyphGroup.
        // For all items with an id below 191, they are calculated by taking the StandardGlyphGroup enum and adding the appropriate
        // StandardGlyphItem value (GlyphItemFriend, GlyphItemInternal, GlyphItemPrivate, etc...)
        // This was found through discovery and is not documented at this time
        internal static String GetNameFromIconAutomationText(String id)
        {
            int num = Convert.ToInt32(id);
            int mod = num % (int)StandardGlyphItem.TotalGlyphItems;
            if (num < maxItemsWithModifiers) num -= mod;

            String name;
            switch ((StandardGlyphGroup)num)
            {
                case StandardGlyphGroup.GlyphGroupClass: name = "Classes"; break;
                case StandardGlyphGroup.GlyphGroupMethod: name = "Methods"; break;
                case StandardGlyphGroup.GlyphGroupProperty: name = "Properties"; break;
                case StandardGlyphGroup.GlyphGroupInterface: name = "Interfaces"; break;
                case StandardGlyphGroup.GlyphGroupEnum: name = "Enums"; break;
                case StandardGlyphGroup.GlyphGroupVariable: name = "Variables"; break;
                case StandardGlyphGroup.GlyphKeyword: name = "Keywords"; break;
                case StandardGlyphGroup.GlyphGroupNamespace: name = "Namespaces"; break;
                case StandardGlyphGroup.GlyphGroupStruct: name = "Types"; break;
                case StandardGlyphGroup.GlyphCSharpExpansion: name = "C# Expansion"; break;
                case StandardGlyphGroup.GlyphGroupDelegate: name = "Delegates"; break;
                case StandardGlyphGroup.GlyphExtensionMethod: name = "Extensions"; break;
                case StandardGlyphGroup.GlyphGroupEvent: name = "Events"; break;
                case StandardGlyphGroup.GlyphGroupField: name = "Field"; break;
                case StandardGlyphGroup.GlyphGroupEnumMember: name = "Enum Values"; break;
                case StandardGlyphGroup.GlyphCSharpFile: name = "C# File"; break;

                // Ones I haven't encountered yet; Left here as placeholders 
                case StandardGlyphGroup.GlyphGroupUnknown:
                case StandardGlyphGroup.GlyphXmlDescendantCheck:
                case StandardGlyphGroup.GlyphXmlDescendantQuestion:
                case StandardGlyphGroup.GlyphXmlChildCheck:
                case StandardGlyphGroup.GlyphXmlChildQuestion:
                case StandardGlyphGroup.GlyphXmlAttributeCheck:
                case StandardGlyphGroup.GlyphXmlAttributeQuestion:
                case StandardGlyphGroup.GlyphXmlNamespace:
                case StandardGlyphGroup.GlyphXmlDescendant:
                case StandardGlyphGroup.GlyphXmlChild:
                case StandardGlyphGroup.GlyphXmlAttribute:
                case StandardGlyphGroup.GlyphExtensionMethodShortcut:
                case StandardGlyphGroup.GlyphExtensionMethodPrivate:
                case StandardGlyphGroup.GlyphExtensionMethodProtected:
                case StandardGlyphGroup.GlyphExtensionMethodFriend:
                case StandardGlyphGroup.GlyphExtensionMethodInternal:
                case StandardGlyphGroup.GlyphMaybeCall:
                case StandardGlyphGroup.GlyphMaybeCaller:
                case StandardGlyphGroup.GlyphMaybeReference:
                case StandardGlyphGroup.GlyphWarning:
                case StandardGlyphGroup.GlyphCallGraph:
                case StandardGlyphGroup.GlyphCallersGraph:
                case StandardGlyphGroup.GlyphForwardType:
                case StandardGlyphGroup.GlyphJSharpDocument:
                case StandardGlyphGroup.GlyphJSharpProject:
                case StandardGlyphGroup.GlyphXmlItem:
                case StandardGlyphGroup.GlyphRecursion:
                case StandardGlyphGroup.GlyphReference:
                case StandardGlyphGroup.GlyphInformation:
                case StandardGlyphGroup.GlyphArrow:
                case StandardGlyphGroup.GlyphClosedFolder:
                case StandardGlyphGroup.GlyphOpenFolder:
                case StandardGlyphGroup.GlyphDialogId:
                case StandardGlyphGroup.GlyphCppProject:
                case StandardGlyphGroup.GlyphCoolProject:
                case StandardGlyphGroup.GlyphVBProject:
                case StandardGlyphGroup.GlyphLibrary:
                case StandardGlyphGroup.GlyphAssembly:
                case StandardGlyphGroup.GlyphBscFile:
                case StandardGlyphGroup.GlyphGroupError:
                case StandardGlyphGroup.GlyphGroupJSharpInterface:
                case StandardGlyphGroup.GlyphGroupJSharpNamespace:
                case StandardGlyphGroup.GlyphGroupJSharpClass:
                case StandardGlyphGroup.GlyphGroupJSharpField:
                case StandardGlyphGroup.GlyphGroupJSharpMethod:
                case StandardGlyphGroup.GlyphGroupIntrinsic:
                case StandardGlyphGroup.GlyphGroupValueType:
                case StandardGlyphGroup.GlyphGroupUnion:
                case StandardGlyphGroup.GlyphGroupType:
                case StandardGlyphGroup.GlyphGroupTypedef:
                case StandardGlyphGroup.GlyphGroupTemplate:
                case StandardGlyphGroup.GlyphGroupOperator:
                case StandardGlyphGroup.GlyphGroupModule:
                case StandardGlyphGroup.GlyphGroupOverload:
                case StandardGlyphGroup.GlyphGroupMapItem:
                case StandardGlyphGroup.GlyphGroupMap:
                case StandardGlyphGroup.GlyphGroupMacro:
                case StandardGlyphGroup.GlyphGroupException:
                case StandardGlyphGroup.GlyphGroupConstant:
                    name = String.Format("{0} ({1})", ((StandardGlyphGroup)num).ToString(),
                        num);
                    break;
                default:
                    name = num.ToString();
                    break;
            }

            return name;
        }
    }
}
