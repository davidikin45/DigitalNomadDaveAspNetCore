using DND.Common.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Diagnostics;
using System.IO;

namespace DND.Common.Testing.DotNetRun
{
    public abstract class BaseDotNetRunFixture : IDisposable
    {
        private Process _process;
        private string _environment;
        private string _webAppRelativePath;

        private string _url;
        private bool _hideBrowser;

        public BaseDotNetRunFixture(string webAppRelativePath, string environment, string url, bool hideBrowser)
        {
            _webAppRelativePath = webAppRelativePath;
            _environment = environment;
            _hideBrowser = hideBrowser;
            _url = url;
        }

        public void Launch()
        {
            if (!string.IsNullOrEmpty(_webAppRelativePath) && !string.IsNullOrEmpty(_environment) && _url.Contains("localhost"))
            {
                StartServer();
            }
        }

        private void StartServer()
        {
            _process = new Process
            {
                StartInfo =
                {
                    FileName = "dotnet",
                    Arguments = $@"run --project {GetContentRootPath()}\ environment={_environment} --no-build --urls ""{_url}"""
                }
            };

            if (_hideBrowser)
            { 
                _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.UseShellExecute = false;
            }

            _process.Start();
        }

        private string GetProjectFile()
        {
            return Path.GetFileName(_webAppRelativePath) + ".csproj";
        }

        private string GetContentRootPath()
        {
            var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
            var contentPath = Path.GetFullPath(Path.Combine(testProjectPath, _webAppRelativePath));
            return contentPath;
        }

        public bool ProcessRunning
        {
            get
            {
                return !_process.HasExited;
            }
        }

        public void Dispose()
        {
            if(_process != null)
            {
                _process.KillProcessAndChildren();
            }
           
        }
    }
}
