using AutoFixture;
using Moq;
using TrainHelper.WebApi.Controllers;
using TrainHelper.WebApi.Dto.Token;
using TrainHelper.WebApi.Services.Interfaces;

namespace TrainHelper.Tests;

public class AuthControllerTests
{
    private readonly IFixture _fixture;

    public AuthControllerTests() => _fixture = new Fixture();

    [Fact]
    public async Task RefreshToken_Result_Valid()
    {
        // Arrange
        var service = new Mock<IAuthService>();
        service.Setup(s => s.GetTokenByRefreshToken(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Create<TokenResultDto>());
        var controller = GetMockAuthController(service);

        // Act
        var result = await controller.RefreshToken(_fixture.Create<RefreshTokenRequestDto>());

        // Assert
        service.VerifyAll();
        Assert.IsType<TokenResultDto>(result);
    }

    [Fact]
    public async Task Token_Result_Valid()
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
        Assert.IsType<TokenResultDto>(result);
    }

    private AuthController GetMockAuthController(Mock<IAuthService>? service = null)
    {
        service ??= new Mock<IAuthService>();
        return new AuthController(service.Object);
    }
}