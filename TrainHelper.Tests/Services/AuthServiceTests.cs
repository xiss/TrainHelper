using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TrainHelper.DAL.Entities;
using TrainHelper.DAL.Providers;
using TrainHelper.WebApi.Config;
using TrainHelper.WebApi.Constants;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Services;

namespace TrainHelper.WebApi.Tests.Services;

public class AuthServiceTests
{
    private readonly IOptions<AuthSettings> _authSettings;
    private readonly IFixture _fixture;
    private readonly IMapper _mapper;
    private readonly TimeSpan _precisionDateTime = new (0, 0, 0, 2);

    private const string Password = "TestPassword123";
    private const string PasswordHash = "d519397a4e89a7a66d28a266ed00a679bdee93fddec9ebba7d01ff27c39c1a99";

    public AuthServiceTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        _mapper = mapperConfiguration.CreateMapper();

        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _fixture.Customize(new CompositeCustomization(new AutoMoqCustomization()));

        _authSettings = _fixture.Create<IOptions<AuthSettings>>();
        _authSettings.Value.Audience = _fixture.Create<string>();
        _authSettings.Value.Issuer = _fixture.Create<string>();
        _authSettings.Value.Key = _fixture.Create<string>();
        _authSettings.Value.LifeTime = _fixture.Create<int>();

        IdentityModelEventSource.ShowPII = true;
    }

    [Fact]
    public async Task CreateUser_ValidCreateUser_ValidUserToAdd()
    {
        // Arrange
        var provider = new Mock<IUserDataProvider>();
        var createUserDto = _fixture.Create<CreateUserDto>();
        User? userCallback = null;
        provider.Setup(s => s.AddUser(It.IsAny<User>()))
            .ReturnsAsync(_fixture.Create<User>())
            .Callback<User>(u => userCallback = u);
        var service = GetAuthService(provider.Object);

        // Act
        await service.CreateUser(createUserDto);

        // Assert
        provider.VerifyAll();
        Assert.Equal(createUserDto.Email, userCallback?.Email);
        Assert.Equal(createUserDto.Name, userCallback?.Name);
        Assert.Equal(createUserDto.Patronymic, userCallback?.Patronymic);
        Assert.Equal(createUserDto.Surname, userCallback?.Surname);
        Assert.NotEqual(createUserDto.Password, userCallback?.PasswordHash);
    }

    [Fact]
    public async Task GetToken_UserNotFound_Error()
    {
        // Arrange
        var service = GetAuthService();

        // Act
        var result = await service.GetToken(_fixture.Create<string>(), _fixture.Create<string>());

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Error);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task GetToken_ValidRequest_CorrectTokens()
    {
        // Arrange
        var user = GetUserWithPasswordHash();
        var provider = new Mock<IUserDataProvider>();
        var session = _fixture.Create<UserSession>();
        session.User = user;
        var now = DateTime.UtcNow;

        provider.Setup(s => s.GetUserByLogin(It.Is<string>(l => l == user.Email)))
            .ReturnsAsync(user);
        provider.Setup(s => s.AddUserSession(It.IsAny<UserSession>()))
            .ReturnsAsync(session);
        var service = GetAuthService(provider.Object);

        // Act
        var result = await service.GetToken(user.Email, Password);

        // Assert
        provider.VerifyAll();
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token?.SecurityToken);
        var refreshToken = new JwtSecurityTokenHandler().ReadJwtToken(result.Token?.RefreshToken);
        //token
        Assert.Equal(_authSettings.Value.Issuer, token.Issuer); //Issuer
        Assert.Equal(now.AddMinutes(_authSettings.Value.LifeTime), token.ValidTo, _precisionDateTime); //ValidTo
        Assert.Equal(now, token.ValidFrom, _precisionDateTime); //ValidFrom
        Assert.Equal(_authSettings.Value.Audience, token.Audiences.First());//Audience
        Assert.Equal(user.Name, token.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultNameClaimType)?.Value); //Claim name
        Assert.Equal(user.Email, token.Claims.FirstOrDefault(c => c.Type == ClaimNames.Login)?.Value); //Claim email
        Assert.Equal(user.Id.ToString(), token.Claims.FirstOrDefault(c => c.Type == ClaimNames.UserId)?.Value);//Claim userId
        Assert.Equal(session.Id.ToString(), token.Claims.FirstOrDefault(c => c.Type == ClaimNames.SessionId)?.Value);//Claim sessionId
        //refreshToken
        Assert.Equal(session.RefreshTokenId.ToString(), refreshToken.Claims.FirstOrDefault(c => c.Type == ClaimNames.RefreshTokenId)?.Value);//Claim RefreshTokenId
        Assert.Equal(now, refreshToken.ValidFrom, _precisionDateTime); // ValidFrom
        Assert.Equal(now.AddHours(_authSettings.Value.LifeTime), refreshToken.ValidTo, _precisionDateTime);//ValidTo
    }

    [Fact]
    public async Task GetToken_ValidRequest_TokenResultAndAddedSession()
    {
        // Arrange
        var user = GetUserWithPasswordHash();
        var provider = new Mock<IUserDataProvider>();
        var sessionCallback = new UserSession();
        provider.Setup(s => s.GetUserByLogin(It.Is<string>(l => l == user.Email)))
            .ReturnsAsync(user);
        provider.Setup(s => s.AddUserSession(It.IsAny<UserSession>()))
            .ReturnsAsync(_fixture.Create<UserSession>())
            .Callback<UserSession>(s => sessionCallback = s);
        var service = GetAuthService(provider.Object);

        // Act
        var result = await service.GetToken(user.Email, Password);

        // Assert
        provider.VerifyAll();
        Assert.Null(result.Error);
        Assert.NotNull(result.Token);
        Assert.Equal(user, sessionCallback.User);
        Assert.Equal(DateTimeOffset.Now.UtcDateTime, sessionCallback.Created.UtcDateTime, _precisionDateTime);
    }

    [Fact]
    public async Task GetTokenByRefreshToken_InvalidToken_Error()
    {
        // Arrange
        var service = GetAuthService();

        // Act
        var result = await service.GetTokenByRefreshToken(_fixture.Create<string>());

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Error);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task GetUserByCredential_CorrectPassword_User()
    {
        // Arrange
        var user = GetUserWithPasswordHash();
        var provider = new Mock<IUserDataProvider>();
        provider.Setup(s => s.GetUserByLogin(It.IsAny<string>()))
            .ReturnsAsync(user);
        var service = GetAuthService(provider.Object);

        // Act
        var result = await service.GetUserByCredential(_fixture.Create<string>(), Password);

        // Assert
        provider.VerifyAll();
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUserByCredential_IncorrectPassword_Null()
    {
        // Arrange
        var provider = new Mock<IUserDataProvider>();
        provider.Setup(s => s.GetUserByLogin(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Create<User>());
        var service = GetAuthService(provider.Object);

        // Act
        var result = await service.GetUserByCredential(_fixture.Create<string>(), _fixture.Create<string>());

        // Assert
        provider.VerifyAll();
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByCredential_UserNotFound_Null()
    {
        // Arrange
        var provider = new Mock<IUserDataProvider>();
        provider.Setup(s => s.GetUserByLogin(It.IsAny<string>()))
            .ReturnsAsync((User?)null);
        var service = GetAuthService(provider.Object);

        // Act
        var result = await service.GetUserByCredential(_fixture.Create<string>(), _fixture.Create<string>());

        // Assert
        provider.VerifyAll();
        Assert.Null(result);
    }

    private AuthService GetAuthService(IUserDataProvider? userDataProvider = null)
    {
        userDataProvider ??= _fixture.Create<IUserDataProvider>();
        return new AuthService(_authSettings, _mapper, userDataProvider);
    }

    private User GetUserWithPasswordHash() => _fixture.Build<User>().With(u => u.PasswordHash, PasswordHash).Create();
}