﻿using DND.Common.Alerts;
using DND.Common.Infrastructure;
using DND.TestSetup;
using DND.Web.Mvc.Contact.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DND.IntegrationTestsXUnit
{
    public class DNDWebApplicationShould : IAssemblyFixture<DbSetupAndTestServerXUnitFixture>
    {
        private readonly DbSetupAndTestServerXUnitFixture _fixture;
     
        public DNDWebApplicationShould(DbSetupAndTestServerXUnitFixture fixture)
        {
            this._fixture = fixture;
        }

        [Theory]
        [InlineData("")]
        [InlineData("/blog")]
        [InlineData("/gallery")]
        [InlineData("/videos")]
        [InlineData("/bucket-list")]
        [InlineData("/travel-map")]
        [InlineData("/about")]
        [InlineData("/work-with-me")]
        [InlineData("/contact")]
        public async Task RenderPageSuccessfully(string path)
        {
            var response = await _fixture.TestServer.Client.GetAsync(path);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.True(true);
        }

        [Fact]
        public async Task GetAllPublicRoutesAndRenderPagesSuccessfully()
        {
            var response = await _fixture.TestServer.Client.GetAsync("/all-routes");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            dynamic routes = JsonConvert.DeserializeObject(responseString);

            var testRoutes = new List<string>();
            foreach (var action in routes.actions)
            {
                string template = action.Template;
                IEnumerable<string> httpMethods = action.HttpMethods.ToObject<List<string>>();
                bool authorized = action.Authorized;

                if (!authorized && template != null && (httpMethods == null || httpMethods.ToList().Contains(HttpMethod.Get.Method)))
                {
                    testRoutes.Add("/"+template);
                }           
            }

            foreach (var route in testRoutes)
            {
                if(!route.Contains("{") && !route.Contains("api"))
                {
                    response = await _fixture.TestServer.Client.GetAsync(route);
                    Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
                }
            }

            Assert.True(true);
        }

        [Fact]
        public async Task AcceptContactFormPost()
        {
            // Get initial response that contains anti forgery tokens
            HttpResponseMessage initialResponse = await _fixture.TestServer.Client.GetAsync("/contact");
            var antiForgeryValues = await _fixture.TestServer.ExtractAntiForgeryValues(initialResponse);

            // Create POST request, adding anti forgery cookie and form field
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/contact");

            postRequest.Headers.Add("Cookie", $"{TestServerFixture.AntiForgeryCookieName}={antiForgeryValues.cookieValue}");

            var formData = new Dictionary<string, string>
            {
                {TestServerFixture.AntiForgeryFieldName, antiForgeryValues.fieldValue},
                {nameof(ContactViewModel.Name),"James Smith"},
                {nameof(ContactViewModel.Email),"test@gmail.com"},
                {nameof(ContactViewModel.Website),""},
                {nameof(ContactViewModel.Subject),"Enquiry"},
                {nameof(ContactViewModel.Message),"This is a test message"}
            };

            postRequest.Content = new FormUrlEncodedContent(formData);

            HttpResponseMessage postResponse = await _fixture.TestServer.Client.SendAsync(postRequest);

            postResponse.EnsureSuccessStatusCode();

            var responseString = await postResponse.Content.ReadAsStringAsync();

            Assert.Contains(Messages.MessageSentSuccessfully, responseString);
        }
    }

}
