using AutoMapper;
using DND.Domain.FlightSearch.Search.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.ViewModels;
using DND.Web.MVCImplementation.FlightSearch.Api;
using Moq;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace DND.UnitTests.Controllers.Api
{
    public class FlightSearchControllerShould
    {
        private FlightSearchController _controller;

        public FlightSearchControllerShould()
        {
            var expected = new FlightSearchRequestDto();
            var mockService = new Mock<IFlightSearchApplicationService>();
            mockService.Setup(s => s.SearchAsync(It.IsAny<FlightSearchRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FlightSearchResponseDto(new List<ItineraryDto>(), 0, 10, 1));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<FlightSearchClientRequestForm, FlightSearchRequestDto>(It.IsAny<FlightSearchClientRequestForm>()))
                .Returns(expected);

            //var mockLogger = new Mock<ILogFactory>();

            _controller = new FlightSearchController(mockService.Object, mockMapper.Object, null, null, null);
            //_controller.MockHttpContext("1", "d.ikin@test.com");
        }

        [Fact]
        public void ShouldReturnBetterJsonResultResponseForSearchRequest()
        {
            var model = new FlightSearchClientRequestForm();
            //var task = _controller.Search(model);
            //task.Wait();
            // result = task.Result;
            // result.Should().BeOfType<BetterJsonResult<FlightSearchResponseDto>>();
        }

        [Fact]
        public void ReturnErrorForInvalidSearchRequest()
        {
            var model = new FlightSearchClientRequestForm();
            // _controller.BindModelToController(model);

            //var task = _controller.Search(new FlightSearchClientRequestForm());
            //  task.Wait();
            //var result = (BetterJsonResult)task.Result;
        }
    }
}
