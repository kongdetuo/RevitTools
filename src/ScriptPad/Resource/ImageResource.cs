using Microsoft.CodeAnalysis.Tags;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ScriptPad
{
    public class ImageResource
    {
        private static ResourceDictionary ResourceDictionary = new Lazy<ResourceDictionary>(() =>
        {
            var dic = new ResourceDictionary();
            dic.Source = new Uri("/ScriptPad;component/Resource/ImageResource.xaml", UriKind.Relative);
            return dic;
        }, true).Value;

        private static ImageSource GetImage(string resourceKey)
        {
            return (ImageSource)ResourceDictionary[resourceKey];      
        }

        public static ImageSource GetImage(ImmutableArray<string> tags)
        {
            var tag = tags.FirstOrDefault();
            if (tag == null) { return null; }

            switch (tag)
            {
                case WellKnownTags.Class:
                    return GetImage("ClassImageSource");

                case WellKnownTags.Constant:
                    return GetImage("ConstantImageSource");

                case WellKnownTags.Delegate:
                    return GetImage("DelegateImageSource");

                case WellKnownTags.Enum:
                    return GetImage("EnumImageSource");

                case WellKnownTags.EnumMember:
                    return GetImage("EnumItemImageSource");

                case WellKnownTags.Event:
                    return GetImage("EventImageSource");

                case WellKnownTags.ExtensionMethod:
                    return GetImage("ExtensionMethodImageSource");

                case WellKnownTags.Field:
                    return GetImage("FieldImageSource");

                case WellKnownTags.Interface:
                    return GetImage("InterfaceImageSource");

                case WellKnownTags.Keyword:
                    return GetImage("KeywordImageSource");

                case WellKnownTags.Method:
                    return GetImage("MethodImageSource");

                case WellKnownTags.Module:
                    return GetImage("ModuleImageSource");

                case WellKnownTags.Namespace:
                    return GetImage("NamespaceImageSource");

                case WellKnownTags.Property:
                    return GetImage("PropertyImageSource");

                case WellKnownTags.Structure:
                    return GetImage("StructureImageSource");
            }
            return null;
        }

    }
}