using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Picosa.App.Infrastructure
{
    public class ViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var viewModelType = item.GetType();

            var viewType = GetViewTypeForViewModelType(viewModelType);

            DataTemplate dataTemplate = null;

            if (viewType != null)
                dataTemplate = CreateTemplate(viewType);

            return dataTemplate;
        }

        public static Type GetViewTypeForViewModelType(Type viewModelType)
        {
            return GetViewTypeForViewModelType(viewModelType, viewModelType.Assembly);
        }

        public static Type GetViewTypeForViewModelType(Type viewModelType, Assembly assembly)
        {
            var viewModelAssembly = viewModelType.Assembly;

            while (viewModelType != null && viewModelType.Assembly == viewModelAssembly)
            {
                if (string.IsNullOrEmpty(viewModelType.FullName))
                    return null;

                var viewModelTypeName = viewModelType.FullName.Split('`')[0];

                var viewTypeName = viewModelTypeName.Replace("ViewModel", "View");

                var viewType = assembly.GetType(viewTypeName);

                if (viewType != null && !viewType.IsAbstract && typeof(FrameworkElement).IsAssignableFrom(viewType))
                    return viewType;

                viewModelType = viewModelType.BaseType;
            }

            return null;
        }

        public static DataTemplate CreateTemplate(Type viewType)
        {
            if (viewType.Namespace == null)
                throw new ArgumentException($"{nameof(viewType)} must have a clr namespace.", nameof(viewType));

            if (!typeof(FrameworkElement).IsAssignableFrom(viewType))
                throw new ArgumentException($"{nameof(viewType)} must be of type FrameworkElement", nameof(viewType));

            var xaml = $@"<DataTemplate><v:{viewType.Name} /></DataTemplate>";

            var context = new ParserContext { XamlTypeMapper = new XamlTypeMapper(new string[0]) };
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("v", "v");
            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            return (DataTemplate)XamlReader.Parse(xaml, context);
        }
    }
}