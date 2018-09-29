using AutoMapper;
using DND.Common.ActionResults;
using DND.Common.Testing;
using DND.Domain.FlightSearch.Search.Dtos;
using DND.Domain.ViewModels;
using DND.Interfaces.FlightSearch.ApplicationServices;
using DND.Web.FlightSearch.Mvc.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace DND.UnitTests.Controllers.Api
{
    //1. Don't test validation in your controller tests. Either you trust MVC's validation or write your own. 
    //2. If you do want to test validation is doing what you expect, test it in your model tests.
    //3. What you really want to test here is that your controller does what you expect it to do when validation fails.
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

            _controller = new FlightSearchController(mockService.Object, mockMapper.Object, null, null, null);
            _controller.MockCurrentUser("1", "d.ikin@test.com", IdentityConstants.ApplicationScheme);
        }

        [Fact]
        public async void ShouldReturnOkObjectResponseForSearchRequest()
        {
            var model = new FlightSearchClientRequestForm();
            var result = await _controller.Search(model);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async void ReturnErrorForInvalidSearchRequest()
        {
            var model = new FlightSearchClientRequestForm();

            _controller.ViewData.ModelState.AddModelError("Key", "ErrorMessage");

            var result = await _controller.Search(model);
            result.Should().BeOfType<UnprocessableEntityAngularObjectResult>();
        }
    }
}
