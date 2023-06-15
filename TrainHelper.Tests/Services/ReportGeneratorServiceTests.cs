using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using OfficeOpenXml;
using TrainHelper.DAL.Entities;
using TrainHelper.DAL.Providers;
using TrainHelper.WebApi.Config;
using TrainHelper.WebApi.Services;
using TrainHelper.WebApi.Services.Interfaces;

namespace TrainHelper.WebApi.Tests.Services
{
    public class ReportGeneratorServiceTests
    {
        private readonly IFixture _fixture;
        private readonly IMapper _mapper;

        public ReportGeneratorServiceTests()
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
        public async Task GetNlDetailsReport_NullTrain_Null()
        {
            // Arrange
            var provider = new Mock<ITrainDataProvider>();
            provider.Setup(s => s.GetTrainDetail(It.IsAny<int>()))
                .ReturnsAsync((Train?)null);
            using var service = GetReportGeneratorService(provider.Object);

            // Act
            var result = await service.GetNlDetailsReport(_fixture.Create<int>());

            // Assert
            provider.VerifyAll();
            Assert.Null(result);
        }

        [Fact]
        public async Task GetNlDetailsReport_ValidTrain_CorrectData()
        {
            // Arrange
            var train = _fixture.Create<Train>();
            train.TrainIndexCombined = "111-123-456";
            var provider = new Mock<ITrainDataProvider>();
            provider.Setup(s => s.GetTrainDetail(It.IsAny<int>()))
                .ReturnsAsync(train);
            using var service = GetReportGeneratorService(provider.Object);

            // Act
            var result = await service.GetNlDetailsReport(_fixture.Create<int>());

            // Assert
            provider.VerifyAll();
            Assert.Equal(train.Number, result?.TrainNumber);
            Assert.Equal("123", result?.TailNumber);
            Assert.Equal(train.Cars.First().WayPoints.First().Station.StationName, result?.LastStation);
            Assert.Equal(train.Cars.First().WayPoints.First().OperationDate, result?.WhenLastOperation);
            Assert.Equal(
                train.Cars.OrderBy(c => c.PositionInTrain).Select(c => c.PositionInTrain).ToList(),
                result?.Cars.Select(c => c.PositionInTrain).ToList());
            Assert.Equal(train.Cars.Select(c => c.Freight.FreightEtsngName).Distinct().Count(), result?.Subtotals.Count);
            Assert.Equal(
                train.Cars.GroupBy(c => c.Freight).Select(g => g.Sum(g => (double)g.FreightTotalWeightKg / 1000)).ToList(),
                result?.Subtotals.Select(s => s.FreightTotalWeightTn).ToList());
        }

        [Fact]
        public async Task GetNlDetailsReportXlsx_NullTrain_EmptyResult()
        {
            // Arrange
            var provider = new Mock<ITrainDataProvider>();
            provider.Setup(s => s.GetTrainDetail(It.IsAny<int>()))
                .ReturnsAsync((Train?)null);
            using var service = GetReportGeneratorService(provider.Object);

            // Act
            var result = await service.GetNlDetailsReportXlsx(_fixture.Create<int>());

            // Assert
            provider.VerifyAll();
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetNlDetailsReportXlsx_ValidReport_CorrectExcel()
        {
            // Arrange
            var provider = new Mock<ITrainDataProvider>();
            var appConfig = _fixture.Create<IOptions<AppConfig>>();
            appConfig.Value.NlReportTemplate = "ReportTemplates\\NL_Template.xlsx";
            provider.Setup(s => s.GetTrainDetail(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Create<Train>());

            using var service = GetReportGeneratorService(provider.Object, appConfig);
            var expected = await service.GetNlDetailsReport(_fixture.Create<int>());

            // Act
            var result = await service.GetNlDetailsReportXlsx(_fixture.Create<int>());

            // Assert
            using var memStream = new MemoryStream(result);
            var sheet = new ExcelPackage(memStream).Workbook.Worksheets[0];
            provider.VerifyAll();
            Assert.Equal(expected?.TrainNumber, sheet.Cells["C3"].GetValue<int>());
            Assert.Equal(expected?.TailNumber, sheet.Cells["C4"].GetValue<string>());
            Assert.Equal(expected?.LastStation, sheet.Cells["E3"].GetValue<string>());
            Assert.Equal(expected?.WhenLastOperation.Date.ToShortDateString(), sheet.Cells["E4"].GetValue<string>());
            var row = 7;
            // table data
            Assert.All(expected.Cars, c =>
            {
                Assert.Equal(c.PositionInTrain, sheet.Cells[$"A{row}"].GetValue<int>());
                Assert.Equal(c.CarNumber, sheet.Cells[$"B{row}"].GetValue<int>());
                Assert.Equal(c.InvoiceNumber, sheet.Cells[$"C{row}"].GetValue<string>());
                Assert.Equal(c.WhenLastOperation.Date.ToShortDateString(), sheet.Cells[$"D{row}"].GetValue<string>());
                Assert.Equal(c.FreightEtsngName, sheet.Cells[$"E{row}"].GetValue<string>());
                Assert.Equal(c.FreightTotalWeightTn, sheet.Cells[$"F{row}"].GetValue<double>());
                Assert.Equal(c.LastOperationName, sheet.Cells[$"G{row}"].GetValue<string>());
                row++;
            });
            // subtotals data
            Assert.All(expected.Subtotals, s =>
            {
                Assert.Equal(s.CarsCount, sheet.Cells[$"B{row}"].GetValue<int>());
                Assert.Equal(s.FreightEtsngName, sheet.Cells[$"E{row}"].GetValue<string>());
                Assert.Equal(s.FreightTotalWeightTn, sheet.Cells[$"F{row}"].GetValue<double>());
                row++;
            });
            // totals
            Assert.Equal("Всего: " + expected?.Cars.Count, sheet.Cells[$"A{row}"].GetValue<string>());
            Assert.Equal(expected?.Subtotals.Count, sheet.Cells[$"E{row}"].GetValue<int>());
            Assert.Equal(expected?.Cars.Sum(c => c.FreightTotalWeightTn), sheet.Cells[$"F{row}"].GetValue<int>());
        }

        private IReportGeneratorService GetReportGeneratorService(
            ITrainDataProvider? trainDataProvider = null,
            IOptions<AppConfig>? apcConfig = null)
        {
            trainDataProvider ??= _fixture.Create<ITrainDataProvider>();
            apcConfig ??= _fixture.Create<IOptions<AppConfig>>();
            return new ReportGeneratorService(trainDataProvider, _mapper, apcConfig);
        }
    }
}