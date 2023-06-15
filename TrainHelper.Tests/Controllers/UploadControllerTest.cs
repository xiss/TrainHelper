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

namespace TrainHelper.WebApi.Tests.Controllers;

public class UploadControllerTest
{
    private const string TestUrl = "testUrl";
    private readonly IFixture _fixture;
    private readonly int _testUserId;

    public UploadControllerTest()
    {
        _testUserId = 1;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task UploadData_Result_Valid()
    {
        // Arrange
        var controller = GetMockUploadController();
        var stream = new MemoryStream();
        var file = new FormFile(stream, 0, stream.Length, _fixture.Create<string>(), _fixture.Create<string>());

        // Act
        var result = await controller.UploadData(file);

        // Assert
        Assert.IsType<ActionResult<UploadDataResultDto>>(result);
    }

    private UploadController GetMockUploadController(Mock<IUploadService>? service = null)
    {
        service ??= new Mock<IUploadService>();

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(s => s.Action(It.IsAny<UrlActionContext>())).Returns(TestUrl);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimNames.UserId, _testUserId.ToString())
        }));

        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var controller = new UploadController(service.Object)
        {
            ControllerContext = controllerContext,
            Url = mockUrlHelper.Object
        };
        return controller;
    }
}