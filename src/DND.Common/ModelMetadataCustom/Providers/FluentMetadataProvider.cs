﻿using DND.Common.ModelMetadataCustom.FluentMetadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.ModelMetadataCustom.Providers
{
    public class FluentMetadataProvider : IDisplayMetadataProvider, IValidationMetadataProvider
    {
        private readonly IMetadataConfiguratorProviderSingleton _provider;

        public FluentMetadataProvider(IMetadataConfiguratorProviderSingleton provider)
        {
            _provider = provider;
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.MetadataKind == ModelMetadataKind.Property)
            {
                foreach (var configurator in _provider.GetMetadataConfigurators(context.Key))
                {
                    configurator.Configure(context.DisplayMetadata);
                }
            }
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context.Key.MetadataKind == ModelMetadataKind.Property)
            {
                foreach (var configurator in _provider.GetMetadataConfigurators(context.Key))
                {
                    configurator.Configure(context.ValidationMetadata);
                }
            }
        }
    }
}