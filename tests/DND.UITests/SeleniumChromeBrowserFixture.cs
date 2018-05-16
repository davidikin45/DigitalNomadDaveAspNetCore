using DND.Common.Testing.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.UITests
{
    public class SeleniumChromeBrowserFixture : BaseSeleniumChromeBrowserFixture, IDisposable
    {
        public SeleniumChromeBrowserFixture()
            :base(ConfigurationManager.AppSettings["SeleniumUrl"], bool.Parse(ConfigurationManager.AppSettings["HideBrowser"]))
        {

        }
    }
}
