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
            var response = await _fixture.TestServer.Client.GetAsync("/api/flight-search/currencies");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.True(true);
        }

        [Fact]
        public async Task SuccessfullyGetLocales()
        {
            var response = await _fixture.TestServer.Client.GetAsync("/api/flight-search/locales");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.True(true);
        }


        [Fact]
        public async Task SuccessfullyGetMarkets()
        {
            var response = await _fixture.TestServer.Client.GetAsync("/api/flight-search/markets/by-locale/en-GB");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.True(true);
        }

    }
}
