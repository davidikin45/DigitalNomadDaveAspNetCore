using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace DND.Common.Testing.Selenium
{
    public abstract class BaseSeleniumChromeBrowserFixture : IDisposable
    {
        public readonly IWebDriver Driver;

        public BaseSeleniumChromeBrowserFixture(string url, bool hideBrowser)
        {
            ChromeOptions options = new ChromeOptions();
            //Hide browser
            if (hideBrowser)
            {
                options.AddArguments("--headless");
            }

            Driver = new ChromeDriver(options);
            Driver.Navigate().GoToUrl(url);
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
