using DND.TestSetup;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DND.Common.HttpClientREST;
using DND.Web.MVCImplementation.Account.Models;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain;
using System.Transactions;

namespace DND.IntegrationTestsXUnit
{
    public class CategoryApiControllerShould : IAssemblyFixture<DbSetupAndTestServerXUnitFixture>
    {
        private readonly DbSetupAndTestServerXUnitFixture _fixture;

        public CategoryApiControllerShould(DbSetupAndTestServerXUnitFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async Task AuthenticateAndCreateCategory()
        {
            var tokenResponse = await _fixture.TestServer.Client.Post("/api/apitoken", new LoginViewModel() { Username = DNDSeedData.AdminUsername, Password = DNDSeedData.AdminPassword });

            tokenResponse.EnsureSuccessStatusCode();

            var tokenResponseObject = tokenResponse.ContentAsDynamic();

            string bearerToken = tokenResponseObject.token;

            var category = new CategoryDto()
            {
                Name = "Category 1",
                Description = "Category 1",
                UrlSlug = "category-1"
            };

            //Create
            var response = await _fixture.TestServer.Client.Post("/api/categories", category, bearerToken);

            response.EnsureSuccessStatusCode();

            //Get Again for concurrency token purposes
            var categoryGetResponse = await _fixture.TestServer.Client.Get("/api/categories/1", bearerToken);

            category = categoryGetResponse.ContentAsType<CategoryDto>();

            //Make updates
            category.Name = "Category 2";
            response = await _fixture.TestServer.Client.Put("/api/categories/1", category, bearerToken);

            response.EnsureSuccessStatusCode();
        }
    }
}
