using AutoMapper;
using DND.Controllers;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;

namespace DND.UnitTests.Controllers.Api
{
    [TestClass]
    public class FlightSearchControllerTests
    {
        private FlightSearchController _controller;

        [TestInitialize()]
        public void TestInitialize()
        {
            var expected = new FlightSearchRequestDTO();
            var mockService = new Mock<IFlightSearchApplicationService>();
            mockService.Setup(s => s.SearchAsync(It.IsAny<FlightSearchRequestDTO>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FlightSearchResponseDTO(new List<ItineraryDTO>(), 0, 10, 1));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<FlightSearchClientRequestForm, FlightSearchRequestDTO>(It.IsAny<FlightSearchClientRequestForm>()))
                .Returns(expected);

            //var mockLogger = new Mock<ILogFactory>();

            _controller = new FlightSearchController(mockService.Object, mockMapper.Object,null,null);
            //_controller.MockHttpContext("1", "d.ikin@test.com");
        }

        [TestMethod]
        public void Search_Request_ShouldReturnBetterJsonResultResponse()
        {
            var model = new FlightSearchClientRequestForm();
            //var task = _controller.Search(model);
            //task.Wait();
            // result = task.Result;
            // result.Should().BeOfType<BetterJsonResult<FlightSearchResponseDTO>>();
        }

        [TestMethod]
        public void Search_InvalidRequest_ShouldReturnError()
        {
            var model = new FlightSearchClientRequestForm();
            // _controller.BindModelToController(model);

            //var task = _controller.Search(new FlightSearchClientRequestForm());
            //  task.Wait();
            //var result = (BetterJsonResult)task.Result;
        }
    }
}
