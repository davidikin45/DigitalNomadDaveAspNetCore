using DND.TestSetup;
using System.Threading.Tasks;
using Xunit;

namespace DND.IntegrationTestsXUnit
{
    public class FlightSearchApiShould : IAssemblyFixture<DbSetupAndTestServerXUnitFixture>
    {    
        private readonly DbSetupAndTestServerXUnitFixture _fixture;

        public FlightSearchApiShould(DbSetupAndTestServerXUnitFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async Task SuccessfullyGetCurrencies()
        {
            var response = await _fixture.TestServer.Client.GetAsync("/api/currencies");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.True(true);
        }

        //[Fact]
        //public async Task ErrorOnInvalidCurrency()
        //{
        //    var response = await _fixture.Client.GetAsync("/api/currencies/abc");

        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        [Fact]
        public async Task SuccessfullyGetLocales()
        {
            var response = await _fixture.TestServer.Client.GetAsync("/api/locales");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.True(true);
        }

        //[Fact]
        //public async Task ErrorOnInvalidLocale()
        //{
        //    var response = await _fixture.Client.GetAsync("/api/currencies/abc");

        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        [Fact]
        public async Task SuccessfullyGetMarkets()
        {
            var response = await _fixture.TestServer.Client.GetAsync("/api/markets/by-locale/en-GB");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.True(true);
        }

        //[Fact]
        //public async Task ErrorOnInvalidMarket()
        //{
        //    var response = await _fixture.Client.GetAsync("/api/markets/abc");

        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

    }
}
