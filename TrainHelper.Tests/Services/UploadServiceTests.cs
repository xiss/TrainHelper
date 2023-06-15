using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TrainHelper.DAL.Entities;
using TrainHelper.DAL.Providers;
using AutoFixture.AutoMoq;
using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using TrainHelper.WebApi.Services;
using TrainHelper.WebApi.Services.Interfaces;

namespace TrainHelper.WebApi.Tests.Services
{
    public class UploadServiceTests
    {
        private readonly IFixture _fixture;
        private readonly IMapper _mapper;

        public UploadServiceTests()
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
        public async Task UploadData_EmptyFile_Unsuccessful()
        {
            // Arrange
            using var service = GetReportUploadService();
            var stream = new MemoryStream();
            var file = new FormFile(stream, 0, stream.Length, _fixture.Create<string>(), _fixture.Create<string>());

            // Act
            var result = await service.UploadData(file);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.True(result.Errors.Count > 0);
        }

        [Fact]
        public async Task UploadData_CorrectFile_Successful()
        {
            // Arrange
            using var service = GetReportUploadService();
            var stream = new MemoryStream();
            var file = new FormFile(stream, 0, stream.Length, _fixture.Create<string>(), _fixture.Create<string>());

            // Act
            var result = await service.UploadData(file);

            // Assert
            Assert.True(false);
        }

        private IUploadService GetReportUploadService(ITrainDataProvider? trainDataProvider = null)
        {
            trainDataProvider ??= _fixture.Create<ITrainDataProvider>();
            return new UploadService(trainDataProvider, _mapper);
        }
    }
}