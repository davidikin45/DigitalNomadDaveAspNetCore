using AutoMapper;
using Solution.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Helpers
{
    public static class YouTubeHelper
    {
        public static string YouTubeEmbedUrl(this string id)
        {
            return String.Format("https://www.youtube.com/embed/{0}", id);
        }

        public static string YouTubeMaxResThumbailUrl(this string id)
        {
            return String.Format("https://img.youtube.com/vi/{0}/maxresdefault.jpg", id);
        }
    }
}
