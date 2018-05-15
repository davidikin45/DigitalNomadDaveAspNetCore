using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DND.Common.ModelMetadataCustom.Interfaces;
using DND.Common.ModelMetadataCustom.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web
{

    //https://andrewlock.net/accessing-services-when-configuring-mvcoptions-in-asp-net-core/
    public class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IMetadataFilter[] _metadataFilters;

        public ConfigureMvcOptions(IMetadataFilter[] metadataFilters)
        {
            _metadataFilters = metadataFilters;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Add(new AttributeMetadataProvider());
            options.ModelMetadataDetailsProviders.Add(new ConventionsMetadataProvider(_metadataFilters));
        }
    }
}
