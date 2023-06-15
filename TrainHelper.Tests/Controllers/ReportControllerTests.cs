using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System.Security.Claims;
using TrainHelper.WebApi.Constants;
using TrainHelper.WebApi.Controllers;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Services.Interfaces;

namespace TrainHelper.Tests;

public class ReportControllerTests
{
    private const string TestUrl = "testUrl";
    private readonly IFixture _fixture;
    private readonly int _testUserId;

    public ReportControllerTests()
    {
        _testUserId = 1;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetNlDetail_NullReport_NotFoundResult()
    {
        // Arrange
        var service = new Mock<IReportGeneratorService>();
        service.Setup(s => s.GetNlDetailsReport(It.IsAny<int>()))
            .ReturnsAsync((NlDetailsReportDto?)null);
        var controller = GetMockReportController(service);

        // Act
        var result = await controller.GetNlDetail(_fixture.Create<int>());

        // Assert
        service.VerifyAll();
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetNlDetail_Result_Valid()
    {
        // Arrange
        var controller = GetMockReportController();

        // Act
        var result = await controller.GetNlDetail(_fixture.Create<int>());

        // Assert
        Assert.IsAssignableFrom<ActionResult>(result);
    }

    [Fact]
    public async Task GetNlDetailXlsx_EmptyReport_NotFoundResult()
    {
        // Arrange
        var service = new Mock<IReportGeneratorService>();
        service.Setup(s => s.GetNlDetailsReportXlsx(It.IsAny<int>()))
            .ReturnsAsync(Array.Empty<byte>());
        var controller = GetMockReportController(service);

        // Act
        var result = await controller.GetNlDetailXlsx(_fixture.Create<int>());

        // Assert
        service.VerifyAll();
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetNlDetailXlsx_Result_Valid()
    {
        // Arrange
        var controller = GetMockReportController();

        // Act
        var result = await controller.GetNlDetailXlsx(_fixture.Create<int>());

        // Assert
        Assert.IsAssignableFrom<ActionResult>(result);
    }

    private ReportController GetMockReportController(Mock<IReportGeneratorService>? service = null)
    {
        service ??= new Mock<IReportGeneratorService>();

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(s => s.Action(It.IsAny<UrlActionContext>())).Returns(TestUrl);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new( ClaimNames.UserId, _testUserId.ToString())
        }));

        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var controller = new ReportController(service.Object)
        {
            ControllerContext = controllerContext,
            Url = mockUrlHelper.Object
        };
        return controller;
    }
}