using Solution.Base.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Extensions;

namespace Solution.Base.Helpers
{
    public static class WebHelper
    {
        public static class ServerDetails
        {
            private const string ConfigFile = "web.config";

            public static string DomainURL
            {
                get
                {
                    return HttpContext.Current.Request.Url().AbsoluteUri.Replace(HttpContext.Current.Request.Url().PathAndQuery, "") + "/";
                }
            }

            public static string DomainHost
            {
                get
                {
                    string text = new Uri(WebHelper.ServerDetails.DomainURL).Host;
                    if (text.EndsWith("/"))
                    {
                        text = text.Substring(0, checked(text.Count<char>() - 1));
                    }
                    return text;
                }
            }

            public static string DomainPhysicalRoot
            {
                get
                {
                    if (HttpContext.Current != null)
                    {
                        return HttpContext.MapPath("~/");
                    }
                    string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uriBuilder = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uriBuilder.Path);
                    return System.IO.Path.GetDirectoryName(path) + "\\";
                }
            }

            public static string CurrentPageDirectory
            {
                get
                {
                    return HttpContext.MapPath(".");
                }
            }

            public static string BinFolder()
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }

            public static bool DLLExists(string dll)
            {
                return System.IO.File.Exists(WebHelper.ServerDetails.BinFolder() + dll);
            }

            public static string DLLPath(string dll)
            {
                return WebHelper.ServerDetails.BinFolder() + dll;
            }

            public static string GetPhysicalPath(string relativePath)
            {
                relativePath = relativePath.Replace("~/", "\\").Replace("/", "\\");
                if (relativePath.First().ToString() == "\\")
                {
                    relativePath = relativePath.Substring(1);
                }
                return WebHelper.ServerDetails.DomainPhysicalRoot + relativePath;
            }

            public static IEnumerable<FileInfoExtended> GetFiles(string relativePath, string extensions = "")
            {
                string physicalPath = WebHelper.ServerDetails.GetPhysicalPath(relativePath);
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(physicalPath);
                List<FileInfoExtended> list = new System.Collections.Generic.List<FileInfoExtended>();
                try
                {
                    foreach (FileInfo current in dir.GetFilesByExtensionsStringCSV(extensions))
                    {
                        if (current.Name != "web.config")
                        {
                            list.Add(new FileInfoExtended(current));
                        }
                    }
                }
                finally
                {
                }
                return list;
            }

            public static IEnumerable<FileInfoExtended> GetImageFiles(string relativePath)
            {
                string physicalPath = WebHelper.ServerDetails.GetPhysicalPath(relativePath);
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(physicalPath);
                System.Collections.Generic.List<FileInfoExtended> list = new System.Collections.Generic.List<FileInfoExtended>();
                try
                {
                    foreach (FileInfo current in dir.GetFilesByExtensions(new string[]
                    {
                        ".jpg",
                        ".png",
                        ".gif"
                    }))
                    {
                        list.Add(new FileInfoExtended(current));
                    }
                }
                finally
                {
                }
                return list;
            }

            public static bool FolderExists(string path)
            {
                return System.IO.Directory.Exists(WebHelper.ServerDetails.GetFolderName(path));
            }

            public static string GetFolderName(string path)
            {
                return System.IO.Path.GetDirectoryName(path);
            }

            public static string GetFileName(string path)
            {
                return System.IO.Path.GetFileName(path);
            }

            public static string GetVirtualRelativeURL(string physicalPath)
            {
                return physicalPath.Substring(WebHelper.ServerDetails.DomainPhysicalRoot.Length).Replace("\\", "/").Insert(0, "~/");
            }

            private static string GetRelativeURL(string physicalPath)
            {
                return WebHelper.ServerDetails.GetVirtualRelativeURL(physicalPath).Replace("~", "");
            }

            public static string GetAbsoluteURL(string pathOrURL)
            {
                string result;
                if (pathOrURL.Contains(":\\"))
                {
                    string relativeURL = WebHelper.ServerDetails.GetRelativeURL(pathOrURL);
                    result = WebHelper.ServerDetails.DomainURL + relativeURL;
                }
                else
                {
                    pathOrURL = pathOrURL.Replace("~", "").Replace("\\", "/");
                    if (pathOrURL.First<char>().ToString() == "/")
                    {
                        pathOrURL = pathOrURL.Substring(1);
                    }
                    result = WebHelper.ServerDetails.DomainURL + pathOrURL;
                }
                return result;
            }
        }

        public class FileInfoExtended
        {
            private System.IO.FileInfo _FileInfo;

            [System.Runtime.CompilerServices.CompilerGenerated]
            private string _Description;

            public System.IO.FileInfo FileInfo
            {
                get
                {
                    return this._FileInfo;
                }
                set
                {
                    this._FileInfo = value;
                }
            }

            public string Description
            {
                get
                {
                    return this._Description;
                }
                set
                {
                    this._Description = value;
                }
            }

            public string FullName
            {
                get
                {
                    return this.FileInfo.FullName;
                }
            }

            public object Name
            {
                get
                {
                    return this.FileInfo.Name;
                }
            }

            public object Extension
            {
                get
                {
                    return this.FileInfo.Extension;
                }
            }

            public object Directory
            {
                get
                {
                    return this.FileInfo.Directory;
                }
            }

            public object DirectoryName
            {
                get
                {
                    return this.FileInfo.DirectoryName;
                }
            }

            public FileInfoExtended(System.IO.FileInfo fileInfo)
            {
                this.FileInfo = fileInfo;
            }
        }
    }
}
