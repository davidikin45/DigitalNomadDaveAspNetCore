using System;
using System.Threading;
using DND.Common.Alerts;
using DND.Common.Testing.Selenium;
using DND.Web.MVCImplementation.Contact.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace DND.UITests
{
    public class SeleniumTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly SeleniumPage _homePage;
        private readonly SeleniumPage _contactPage;

        public SeleniumTests()
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl("http://localhost:54428");

            _homePage = new SeleniumPage(_driver, "");
            _contactPage = new SeleniumPage(_driver, "contact");
        }

        [Fact]
        public void ShouldLoadHomePage_SmokeTest()
        {
            _homePage.NavigateTo();
            Assert.Equal("Digital Nomad Dave", _homePage.Driver.Title);
        }

        [Fact]
        public void ShouldValidateContactDetails()
        {
            _contactPage.NavigateTo();

            // Don't enter a name
            _contactPage.EnterValue(nameof(ContactViewModel.Email), "test@gmail.com");
            _contactPage.EnterValue(nameof(ContactViewModel.Email), "");
            _contactPage.EnterValue(nameof(ContactViewModel.Email), "Enquiry");
            _contactPage.EnterValue(nameof(ContactViewModel.Email), "This is a test message");

           var responsePage = _contactPage.Submit();

            Assert.Equal("Contact Me | Digital Nomad Dave", _contactPage.Driver.Title);

            IWebElement firstErrorMessage =
                _driver.FindElement(By.Id("Name-error"));

            Assert.Equal("The Name field is required.", firstErrorMessage.Text);
        }

        [Fact]
        public void ShouldSubmitContactDetails()
        {
            _contactPage.NavigateTo();

            _contactPage.EnterValue(nameof(ContactViewModel.Name), "James Smith");
            _contactPage.EnterValue(nameof(ContactViewModel.Email), "test@gmail.com");
            _contactPage.EnterValue(nameof(ContactViewModel.Email), "");
            _contactPage.EnterValue(nameof(ContactViewModel.Email), "Enquiry");
            _contactPage.EnterValue(nameof(ContactViewModel.Email), "This is a test message");

            var responsePage = _contactPage.Submit();

            Assert.Equal("Contact Me | Digital Nomad Dave", responsePage.Driver.Title);

           // Assert.Equal(Messages.MessageSentSuccessfully, firstErrorMessage.Text);
        }

        private static void DelayForDemoVideo()
        {
            Thread.Sleep(1000);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
