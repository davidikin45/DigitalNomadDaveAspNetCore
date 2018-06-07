using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using DND.Common.Alerts;
using DND.Common.Testing.Selenium;
using DND.TestSetup;
using DND.Web.MVCImplementation.Contact.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace DND.UITests
{
    public class SeleniumTests : IAssemblyFixture<DbSetupAndDotNetRunXUnitFixture>, IDisposable
    {
        private readonly SeleniumChromeBrowserFixture _fixture;
        private readonly SeleniumPage _contactPage;

        public SeleniumTests(DbSetupAndDotNetRunXUnitFixture dbSetupAndDotNetRun)
        {
            this._fixture = new SeleniumChromeBrowserFixture();
            _contactPage = new SeleniumPage(_fixture.Driver, "contact");
        }

        [Fact]
        public void ShouldLoad_SmokeTest()
        {
            var pages = new List<SeleniumPage>();

            pages.Add(new SeleniumPage(_fixture.Driver, ""));
            pages.Add(new SeleniumPage(_fixture.Driver, "blog"));
            pages.Add(new SeleniumPage(_fixture.Driver, "gallery"));
            pages.Add(new SeleniumPage(_fixture.Driver, "videos"));
            pages.Add(new SeleniumPage(_fixture.Driver, "bucket-list"));
            pages.Add(new SeleniumPage(_fixture.Driver, "travel-map"));
            pages.Add(new SeleniumPage(_fixture.Driver, "about"));
            pages.Add(new SeleniumPage(_fixture.Driver, "work-with-me"));
            pages.Add(new SeleniumPage(_fixture.Driver, "contact"));

            foreach (var page in pages)
            {
                page.NavigateTo();
            }

            Assert.True(true);
        }

        [Fact]
        public void ShouldValidateContactDetails()
        {
            _contactPage.NavigateTo();

            // Don't enter a name
            _contactPage.EnterFormValue(nameof(ContactViewModel.Email), "test@gmail.com");
            _contactPage.EnterFormValue(nameof(ContactViewModel.Website), "");
            _contactPage.EnterFormValue(nameof(ContactViewModel.Subject), "Enquiry");
            _contactPage.EnterFormValue(nameof(ContactViewModel.Message), "This is a test message");

            var responsePage = _contactPage.Submit("ContactSubmit");

            Assert.Equal("Contact Me | Digital Nomad Dave", _contactPage.Driver.Title);

            IWebElement firstErrorMessage =
                _contactPage.Driver.FindElement(By.Id(nameof(ContactViewModel.Name) + "-error"));

            Assert.Equal("The Name field is required.", firstErrorMessage.Text);
        }

        [Fact]
        public void ShouldSubmitContactDetails()
        {
            _contactPage.NavigateTo();

            _contactPage.EnterFormValue(nameof(ContactViewModel.Name), "James Smith");
            _contactPage.EnterFormValue(nameof(ContactViewModel.Email), "test@gmail.com");
            _contactPage.EnterFormValue(nameof(ContactViewModel.Website), "");
            _contactPage.EnterFormValue(nameof(ContactViewModel.Subject), "Enquiry");
            _contactPage.EnterFormValue(nameof(ContactViewModel.Message), "This is a test message");

            var responsePage = _contactPage.Submit("ContactSubmit");

            Assert.Contains(Messages.MessageSentSuccessfully, responsePage.Html);
        }

        private static void DelayForDemoVideo()
        {
            Thread.Sleep(1000);
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }
    }
}
