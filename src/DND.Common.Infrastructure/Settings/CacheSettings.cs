namespace DND.Common.Infrastructure.Settings
{
    public class CacheSettings
    {
        public int UploadFilesDays { get; set; }
        public int VersionedStaticFilesDays { get; set; }
        public int NonVersionedStaticFilesDays { get; set; }
    }
}
