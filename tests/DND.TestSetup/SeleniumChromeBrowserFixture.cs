using DND.Common.Testing.Selenium;
using System;
using System.Configuration;

namespace DND.TestSetup
{
    public class SeleniumChromeBrowserFixture : SeleniumChromeBrowserFixtureBase, IDisposable
    {
        public SeleniumChromeBrowserFixture()
            :base(ConfigurationManager.AppSettings["SeleniumUrl"], bool.Parse(ConfigurationManager.AppSettings["HideBrowser"]))
        {

        }
    }
}
