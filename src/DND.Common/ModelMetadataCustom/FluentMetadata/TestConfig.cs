using DND.Common.Implementation.Models;

namespace DND.Common.ModelMetadataCustom.FluentMetadata
{
    public class TestConfig : ModelMetadataConfiguration<JpegMetadata>
    {
        public TestConfig()
        {
            Configure(a => a.FileInfo).Required().DisplayName("");
            Configure(a => a.DateTaken).Required().DisplayName("");
            Configure(a => a.Comments).DisplayName("");
            Configure<string>("TenantName").Required();
        }
    }
}
