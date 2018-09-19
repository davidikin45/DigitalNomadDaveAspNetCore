using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.PageObjects;

namespace DND.Common.Testing.Selenium
{
    public class SeleniumPage
    {
        public IWebDriver Driver { get; }

        private string _pagePath = "";

        public SeleniumPage(IWebDriver driver, string pagePath)
        {
            _pagePath = pagePath;
            Driver = driver;
            PageFactory.InitElements(driver, this);
        }

        public string Path { get
            {
                return new Uri(Driver.Url).GetLeftPart(UriPartial.Path).Replace(new Uri(Driver.Url).GetLeftPart(UriPartial.Authority)+"/","");
            }
        }
    
        public string Html
        {
            get
            {
                return Driver.FindElement(By.XPath("//*")).GetAttribute("outerHTML");
            }
        }

        public void NavigateTo()
        {
            var root = new Uri(Driver.Url).GetLeftPart(UriPartial.Authority);

            var url = $"{root}/{_pagePath}";

            Driver.Navigate().GoToUrl(url);
        }

        [FindsBy(How = How.CssSelector, Using = ".validation-summary-errors ul > li")]
        private IWebElement FirstError { get; set; }

        public string FirstErrorMessage => FirstError.Text;


        public void EnterFormValue(string id, string value)
        {
            Driver.FindElement(By.Id(id)).SendKeys(value);
        }

        public SeleniumPage ClickLink(string text, string cssClass)
        {
            Driver.FindElement(By.XPath($@"//a[contains(text(), ""{text}"") and @class=""{cssClass}""]")).Click();

            return new SeleniumPage(Driver, Path);
        }

        public SeleniumPage Submit(string id = "Submit")
        {
            Driver.FindElement(By.Id(id)).Click();

            return new SeleniumPage(Driver, _pagePath);
        }
    }
}
