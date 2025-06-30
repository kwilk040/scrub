using API.Controllers;
using Application.Services.Interfaces;
using Core.DTO.ListenBrainz;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Tests;

public class ListenbrainzControllerTest
{
    private readonly Mock<IListenBrainzService> _listenBrainzService;
    private readonly ListenbrainzController _listenBrainzController;

    public ListenbrainzControllerTest()
    {
        _listenBrainzService = new Mock<IListenBrainzService>();
        _listenBrainzController = new ListenbrainzController(_listenBrainzService.Object);
    }

    private static HttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = "Bearer valid-token";
        return httpContext;
    }

    [Fact]
    public async Task ValidateToken_ShouldReturn_OkResult_IfTokenIsValid()
    {
        var httpContext = CreateHttpContext();
        _listenBrainzController.ControllerContext.HttpContext = httpContext;

        _listenBrainzService.Setup(s => s.ValidateToken(It.IsAny<string>()))
            .ReturnsAsync(It.IsAny<ValidateTokenResponse>());

        var result = await _listenBrainzController.ValidateToken();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task ValidateToken_ShouldReturn_BadRequestResult_IfTokenIsNotPresent()
    {
        var httpContext = new DefaultHttpContext();
        _listenBrainzController.ControllerContext.HttpContext = httpContext;

        var result = await _listenBrainzController.ValidateToken();

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var message = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal("You need to provide an Authorization token.", message);
    }

    [Fact]
    public async Task SubmitListens_ShouldReturn_OkResult_IfTokenIsValid()
    {
        var httpContext = CreateHttpContext();
        _listenBrainzController.ControllerContext.HttpContext = httpContext;
        _listenBrainzService.Setup(s => s.SubmitListens(It.IsAny<SubmitListensRequest>(), It.IsAny<string>()))
            .ReturnsAsync(It.IsAny<SubmitListensResponse>());
        var request = new SubmitListensRequest(ListenType.Single, []);

        var result = await _listenBrainzController.SubmitListens(request);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task SubmitListens_ShouldReturn_BadRequestResult_IfTokenIsNotPresent()
    {
        var httpContext = new DefaultHttpContext();
        _listenBrainzController.ControllerContext.HttpContext = httpContext;
        var request = new SubmitListensRequest(ListenType.Single, []);

        var result = await _listenBrainzController.SubmitListens(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var message = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal("You need to provide an Authorization token.", message);
    }
}