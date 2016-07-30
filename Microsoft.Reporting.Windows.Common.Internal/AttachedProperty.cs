using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class AttachedProperty
    {
        private static Collection<DependencyProperty> s_attachedProperties = new Collection<DependencyProperty>();
        private static ReadOnlyCollection<DependencyProperty> s_publicProperties = new ReadOnlyCollection<DependencyProperty>(AttachedProperty.s_attachedProperties);

        public static IEnumerable<DependencyProperty> RegisteredProperties
        {
            get
            {
                return AttachedProperty.s_publicProperties;
            }
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata)
        {
            DependencyProperty dependencyProperty = DependencyProperty.RegisterAttached(name, propertyType, ownerType, defaultMetadata);
            AttachedProperty.s_attachedProperties.Add(dependencyProperty);
            return dependencyProperty;
        }
    }
}
