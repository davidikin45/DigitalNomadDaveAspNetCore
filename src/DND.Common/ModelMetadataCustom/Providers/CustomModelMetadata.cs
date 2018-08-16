using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DND.Common.ModelMetadataCustom.Providers
{
    public class CustomModelMetadata : DefaultModelMetadata
    {
        public new ModelMetadataIdentity Identity { get { return base.Identity; } }

        private readonly ICustomModelMetadataProviderSingleton _customProvider;
        public CustomModelMetadata(ICustomModelMetadataProviderSingleton provider, ICompositeMetadataDetailsProvider detailsProvider, DefaultMetadataDetails details)
            : base(provider, detailsProvider, details)
        {
            _customProvider = provider;
        }

        public CustomModelMetadata(ICustomModelMetadataProviderSingleton provider, ICompositeMetadataDetailsProvider detailsProvider, DefaultMetadataDetails details, DefaultModelBindingMessageProvider modelBindingMessageProvider)
             : base(provider, detailsProvider, details, modelBindingMessageProvider)
        {
            _customProvider = provider;
        }

        //Lazy Loaded
        public ModelPropertyCollection PropertiesRuntime(object model)
        {

            if (!(model is ICustomTypeDescriptor))
            {
                return Properties;
            }
            else
            {
                var propertiesField = typeof(CustomModelMetadata).BaseType.GetField("_properties", BindingFlags.Instance | BindingFlags.NonPublic);

                if (propertiesField.GetValue(this) == null)
                {
                    var properties = _customProvider.GetMetadataForProperties(ModelType, model as ICustomTypeDescriptor);
                    properties = properties.OrderBy(p => p.Order);
                    propertiesField.SetValue(this, new ModelPropertyCollection(properties));
                }

                return (ModelPropertyCollection)propertiesField.GetValue(this);
            }
        }

        //1.
        public override int GetHashCode()
        {
            if (MetadataKind != ModelMetadataKind.Property || !ContainerType.GetInterfaces().Contains(typeof(ICustomTypeDescriptor)))
            {
                return base.GetHashCode();
            }
            else
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + base.GetHashCode();
                    hash = hash * 31 + new Random().Next();
                    return hash;
                };
            }
        }

        //2.
        public override bool Equals(object obj)
        {
            if (MetadataKind != ModelMetadataKind.Property || !ContainerType.GetInterfaces().Contains(typeof(ICustomTypeDescriptor)))
            {
                return base.Equals(obj);
            }
            else
            {
               return false;
            }
        }
    }
}
