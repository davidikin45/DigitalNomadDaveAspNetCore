using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Solution.Base.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Infrastructure
{
    public static class Server
    {

        public static string GetWwwFolderPhysicalPathById(string folderId)
        {
            return FileHelper.GetWwwFolderPhysicalPathById(folderId);
        }

        public static string MapWwwPath(string path)
        {
            var result = path ?? string.Empty;
            if (IsWwwPathMapped(path) == false)
            {
                var wwwroot = WwwRoot();
                if (result.StartsWith("~", StringComparison.Ordinal))
                {
                    result = result.Substring(1);
                }
                if (result.StartsWith("/", StringComparison.Ordinal))
                {
                    result = result.Substring(1);
                }
                result = Path.Combine(wwwroot, result.Replace('/', '\\'));
            }

            return result;
        }

        public static string UnmapWwwPath(string path)
        {
            var result = path ?? string.Empty;
            if (IsWwwPathMapped(path))
            {
                var wwwroot = WwwRoot();
                result = result.Remove(0, wwwroot.Length);
                result = result.Replace('\\', '/');

                var prefix = (result.StartsWith("/", StringComparison.Ordinal) ? "~" : "~/");
                result = prefix + result;
            }

            return result;
        }

        public static bool IsWwwPathMapped(string path)
        {
            var result = path ?? string.Empty;
            return result.StartsWith(WwwRoot(),
                StringComparison.Ordinal);
        }

        public static string MapContentPath(string path)
        {
            var result = path ?? string.Empty;
            if (IsContentPathMapped(path) == false)
            {
                var contentRoot = ContentRoot();
                if (result.StartsWith("~", StringComparison.Ordinal))
                {
                    result = result.Substring(1);
                }
                if (result.StartsWith("/", StringComparison.Ordinal))
                {
                    result = result.Substring(1);
                }
                result = Path.Combine(contentRoot, result.Replace('/', '\\'));
            }

            return result;
        }

        public static string UnmapContentPath(string path)
        {
            var result = path ?? string.Empty;
            if (IsContentPathMapped(path))
            {
                var contentRoot = ContentRoot();
                result = result.Remove(0, contentRoot.Length);
                result = result.Replace('\\', '/');

                var prefix = (result.StartsWith("/", StringComparison.Ordinal) ? "~" : "~/");
                result = prefix + result;
            }

            return result;
        }

        public static bool IsContentPathMapped(string path)
        {
            var result = path ?? string.Empty;
            return result.StartsWith(ContentRoot(),
                StringComparison.Ordinal);
        }

        public static string ContentRoot()
        {
            return StaticProperties.HostingEnvironment.ContentRootPath;
        }

        public static string WwwRoot()
        {         
            return StaticProperties.HostingEnvironment.WebRootPath + @"\";
        }



    }
}
