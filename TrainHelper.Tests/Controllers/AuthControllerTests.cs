using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrainHelper.WebApi.Controllers;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Dto.Token;
using TrainHelper.WebApi.Services;

namespace TrainHelper.WebApi.Tests.Controllers;

public class AuthControllerTests
{
    private readonly IFixture _fixture;

    public AuthControllerTests() => _fixture = new Fixture();

    [Fact]
    public async Task RefreshToken_Result_Valid()
    {
        // Arrange
        var service = new Mock<IAuthService>();
        var tokenResult = new TokenResultDto(_fixture.Create<TokenDto?>());
        service.Setup(s => s.GetTokenByRefreshToken(It.IsAny<string>()))
            .ReturnsAsync(tokenResult);
        var controller = GetMockAuthController(service);

        // Act
        var result = await controller.RefreshToken(_fixture.Create<RefreshTokenRequestDto>());

        // Assert
        service.VerifyAll();
        Assert.IsType<JsonResult>(result);
    }

    [Fact]
    public async Task RefreshToken_WithErrors_Unauthorized()
    {
        // Arrange
        var service = new Mock<IAuthService>();
        service.Setup(s => s.GetTokenByRefreshToken(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Create<TokenResultDto>());
        var controller = GetMockAuthController(service);

        // Act
        var result = await controller.RefreshToken(_fixture.Create<RefreshTokenRequestDto>());

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task RegisterUser_CreateUser_Called()
    {
        // Arrange
        var service = new Mock<IAuthService>();
        service.Setup(s => s.CreateUser(It.IsAny<CreateUserDto>()));
        var controller = GetMockAuthController(service);

        // Act
        await controller.RegisterUser(_fixture.Create<CreateUserDto>());

        // Assert
        service.VerifyAll();
    }

    [Fact]
    public async Task Token_Result_Valid()
    {
        // Arrange
        var service = new Mock<IAuthService>();
        var tokenResult = new TokenResultDto(_fixture.Create<TokenDto?>());
        service.Setup(s => s.GetToken(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(tokenResult);
        var controller = GetMockAuthController(service);

        // Act
        var result = await controller.Token(_fixture.Create<TokenRequestDto>());

        // Assert
        service.VerifyAll();
        Assert.IsType<JsonResult>(result);
    }

    [Fact]
    public async Task Token_WithErrors_Unauthorized()
    {
        // Arrange
        var service = new Mock<IAuthService>();
        service.Setup(s => s.GetToken(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_fixture.Create<TokenResultDto>());
        var controller = GetMockAuthController(service);

        // Act
        var result = await controller.Token(_fixture.Create<TokenRequestDto>());

        // Assert
        service.VerifyAll();
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    private AuthController GetMockAuthController(Mock<IAuthService>? service = null)
    {
        service ??= new Mock<IAuthService>();
        return new AuthController(service.Object);
    }
}