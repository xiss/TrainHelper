using Xunit;
using TrainHelper.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.AutoMoq;
using AutoFixture;
using AutoMapper;
using Microsoft.Extensions.Options;
using TrainHelper.DAL.Providers;
using TrainHelper.WebApi.Config;
using Moq;
using TrainHelper.DAL.Entities;

namespace TrainHelper.WebApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly IFixture _fixture;
        private readonly IMapper _mapper;

        public AuthServiceTests()
        {
            _fixture = new Fixture();

            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = mapperConfiguration.CreateMapper();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _fixture.Customize(new CompositeCustomization(new AutoMoqCustomization()));
        }

        [Fact]
        public async Task CreateUserTest()
        {
            // Arrange
            var provider = new Mock<ITrainDataProvider>();
            provider.Setup(s => s.GetTrainDetail(It.IsAny<int>()))
                .ReturnsAsync((Train?)null);
            //using var service = GetReportGeneratorService(provider.Object);

            // Act
            // var result = await service.GetNlDetailsReport(_fixture.Create<int>());

            // Assert
            provider.VerifyAll();
            //Assert.Null(result);
        }



        [Fact()]
        public async Task GetTokenTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public async Task GetTokenByRefreshTokenTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public async Task GetUserByCredentialTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        private AuthService GetAuthService(
            IUserDataProvider? userDataProvider = null,
            IOptions<AppConfig>? apcConfig = null)
        {
            userDataProvider ??= _fixture.Create<IUserDataProvider>();
            apcConfig ??= _fixture.Create<IOptions<AppConfig>>();
            return new AuthService(apcConfig, _mapper, userDataProvider);
        }
    }
}