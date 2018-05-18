﻿using Microsoft.Extensions.PlatformAbstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.IO;

namespace DND.Common.Testing.Selenium
{
    public abstract class BaseSeleniumChromeBrowserFixture : IDisposable
    {
        public IWebDriver Driver { get; private set; }
        private string _url;
        private bool _hideBrowser;

        public BaseSeleniumChromeBrowserFixture(string url, bool hideBrowser)
        {
            _url = url;
            _hideBrowser = hideBrowser;

            LaunchBrowser();
        }

        private void LaunchBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            //Hide browser
            if (_hideBrowser)
            {
                options.AddArguments("--headless");
            }

            Driver = new ChromeDriver(options);
            Driver.Navigate().GoToUrl(_url);
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
