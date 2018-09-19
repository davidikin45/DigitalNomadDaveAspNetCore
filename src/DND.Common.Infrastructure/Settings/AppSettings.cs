namespace DND.Common.Infrastructure.Settings
{
    public class AppSettings
    {
        public string DefaultCulture { get; set; }
        public string AssemblyPrefix { get; set; }
        public string MVCImplementationFolder { get; set; }
        public string ActiveViewTheme { get; set; }
        public string SignalRUrlPrefix { get; set; }
        public string CookieConsentName { get; set; }
        public string CookieAuthName { get; set; }
        public string CookieApplicationAuthName { get; set; }
        public string CookieExternalAuthName { get; set; }
        public string CookieTempDataName { get; set; }
        public int ResponseCacheSizeMB { get; set; }
        public string AngularApp { get; set; }
        public string Timezone { get; set; }
        public string TimezoneAbbr { get; set; }
        public string SiteTitle { get; set; }
        public string TitleSeparator { get; set; }
        public string SiteDescription { get; set; }
        public string SiteLogoLarge { get; set; }
        public string SiteLogoSmall { get; set; }
        public string SiteShareImage { get; set; }
        public string SiteAboutMeImage { get; set; }
        public string SiteFooterImage { get; set; }
        public string ImageWatermarkShareEnabled { get; set; }
        public string ImageWatermarkEnabled { get; set; }
        public string ImageWatermark { get; set; }
        public string ImageWatermarkMinWidth { get; set; }
        public string ImageWatermarkMinHeight { get; set; }
        public string SiteKeyWords { get; set; }
        public string SiteAuthor { get; set; }
        public string SiteUrl { get; set; }
        public string Domain { get; set; }
        public string BodyFont { get; set; }
        public string NavBarFont { get; set; }
        public string NavBarFontStyleCSS { get; set; }
        public string FacebookAppId { get; set; }
        public string DisqusShortName { get; set; }
        public string AddThisId { get; set; }
        public string GoogleMapsApiKey { get; set; }
        public string InstagramUserId { get; set; }
        public string InstagramAccessToken { get; set; }
        public string GoogleAnalyticsTrackingId { get; set; }
        public string GoogleAdSenseId { get; set; }
        public string RSSFeed { get; set; }
        public string GitHubLink { get; set; }
        public string InstagramLink { get; set; }
        public string FacebookLink { get; set; }
        public string LinkedInLink { get; set; }
        public string YouTubeLink { get; set; }
        public string PublicUploadFolders { get; set; }
        public string FFMpeg { get; set; }
        public string FFMpeg_ExeLocation { get; set; }
        public string FFMpeg_WorkingPath { get; set; }
        public FolderSettings Folders { get; set; }
    }

    public class FolderSettings
    {
        public string Uploads { get; set; }
        public string Gallery { get; set; }
        public string Videos { get; set; }
        public string BucketList { get; set; }
        public string SocialMedia { get; set; }
        public string Carousel { get; set; }
        public string Advertisements { get; set; }
        public string Parellax { get; set; }
        public string Projects { get; set; }
        public string Testimonials { get; set; }
    }
}
