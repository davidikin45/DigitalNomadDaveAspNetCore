using DND.Common.Implementation.Models;

namespace DND.Common.ModelMetadataCustom.FluentMetadata
{
    public class DynamicConfig : ModelMetadataConfiguration<dynamic>
    {
        public DynamicConfig()
        { 
            //Configure<string>("TenantName").Required();
        }
    }
}
