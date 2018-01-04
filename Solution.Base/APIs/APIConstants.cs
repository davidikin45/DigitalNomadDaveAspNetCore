using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.APIs
{
    public static class APIConstants
    {
        public static class Key
        {
            public const string GoogleServerKey = "AIzaSyBq751QPTogCry92Z42Qb50K4vt9lk_yAY";
        }

        public static class URL
        {
            public const string GoogleMap = "https://maps.googleapis.com/maps/api/geocode/json?{0}&sensor=false";

            public const string GoogleURLShortener = "https://www.googleapis.com/urlshortener/v1/url";

            public const string GoogleFontInformation = "https://www.google.com/fonts/specimen/";

            public const string GoogleFont = "https://www.googleapis.com/webfonts/v1/webfonts?&key={0}";

            public const string GoogleFontCSS = "http://fonts.googleapis.com/css?";

            public const string GoogleMapEmbedPlace = "https://www.google.com/maps/embed/v1/place?key={0}";

            public const string GoogleMapEmbedView = "https://www.google.com/maps/embed/v1/view?key={0}";

            public const string Instagram = "https://api.instagram.com/v1/users/{0}/media/recent/?client_id={1}";

            public const string InstagramUserSearch = "https://api.instagram.com/v1/users/search?q={0}&client_id={1}";

            public const string IPLocation = "http://freegeoip.net/json/{0}";
        }
    }
}
