using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Moq;
using TrainHelper.DAL.Entities;
using TrainHelper.DAL.Providers;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Services;

namespace TrainHelper.WebApi.Tests.Services;

public class UploadServiceTests
{
    private const string XmlDataSample = "<Root>\r\n  " +
                                         "<row>\r\n    " +
                                         "<TrainNumber>2236</TrainNumber>\r\n    " +
                                         "<TrainIndexCombined>86560-725-98470</TrainIndexCombined>\r\n    " +
                                         "<FromStationName>САРБАЛА</FromStationName>\r\n    " +
                                         "<ToStationName>НАХОДКА (ЭКСП.)</ToStationName>\r\n    " +
                                         "<LastStationName>ЧЕРНОРЕЧЕНСКАЯ</LastStationName>\r\n    " +
                                         "<WhenLastOperation>30.06.2019 14:07:00</WhenLastOperation>\r\n    " +
                                         "<LastOperationName>ОТПРАВЛЕНИЕ  ВАГОНА В СОСТАВЕ ПОЕЗДА СО СТАНЦИИ</LastOperationName>\r\n    " +
                                         "<InvoiceNum>ЭЛ598121</InvoiceNum>\r\n    " +
                                         "<PositionInTrain>33</PositionInTrain>\r\n    " +
                                         "<CarNumber>63731863</CarNumber>\r\n    " +
                                         "<FreightEtsngName>УГОЛЬ КАМЕННЫЙ МАРКИ Т-ТОЩИЙ</FreightEtsngName>\r\n    " +
                                         "<FreightTotalWeightKg>74700</FreightTotalWeightKg>\r\n " +
                                         "</row>\r\n" +
                                         "</Root>";

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
    public async Task UploadData_CorrectFile_Successful()
    {
        // Arrange
        Train? resultTrain = null;
        var dataProvider = new Mock<ITrainDataProvider>();
        dataProvider.Setup(p => p.
                UpsertTrain(It.IsAny<Train>()))
            .Callback<Train>(t => resultTrain = t)
            .ReturnsAsync(true);
        var service = GetReportUploadService(dataProvider.Object);

        // Act
        var result = await service.UploadData(new UploadFileDto("", await GetStreamWithSample(XmlDataSample)));

        // Assert
        dataProvider.VerifyAll();
        Assert.Empty(result.Errors);
        Assert.True(result.IsSuccessful);
        Assert.Equal(2236, resultTrain?.Number);
        Assert.Equal("86560-725-98470", resultTrain?.TrainIndexCombined);
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.CarNumber == 63731863));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.FromStation.StationName == "САРБАЛА"));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.ToStation.StationName == "НАХОДКА (ЭКСП.)"));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.WayPoints
            .FirstOrDefault(w => w.Station.StationName == "ЧЕРНОРЕЧЕНСКАЯ") != null));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.WayPoints
            .FirstOrDefault(w => w.OperationDate == DateTimeOffset.Parse("30.06.2019 14:07:00")) != null));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.WayPoints
            .FirstOrDefault(w => w.Operation.OperationName == "ОТПРАВЛЕНИЕ  ВАГОНА В СОСТАВЕ ПОЕЗДА СО СТАНЦИИ") != null));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.Invoice.InvoiceNum == "ЭЛ598121"));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.PositionInTrain == 33));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.Freight.FreightEtsngName == "УГОЛЬ КАМЕННЫЙ МАРКИ Т-ТОЩИЙ"));
        Assert.NotNull(resultTrain?.Cars.FirstOrDefault(c => c.FreightTotalWeightKg == 74700));
    }

    [Fact]
    public async Task UploadData_EmptyFile_Unsuccessful()
    {
        // Arrange
        var service = GetReportUploadService();
        var file = await GetStreamWithSample();

        // Act
        var result = await service.UploadData(new UploadFileDto("", file));

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.True(result.Errors.Count > 0);
    }

    [Fact]
    public async Task UploadData_ErrorAccordingAddingTrain_UnsuccessfulWithError()
    {
        // Arrange
        var dataProvider = new Mock<ITrainDataProvider>();
        dataProvider.Setup(p => p.UpsertTrain(It.IsAny<Train>()))
            .ReturnsAsync(false);
        var service = GetReportUploadService(dataProvider.Object);

        // Act
        var result = await service.UploadData(new UploadFileDto("", await GetStreamWithSample(XmlDataSample)));

        // Assert
        dataProvider.VerifyAll();
        Assert.False(result.IsSuccessful);
        Assert.True(result.Errors.Count > 0);
    }

    private async Task<Stream> GetStreamWithSample(string dataSample = "")
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(dataSample);
        await writer.FlushAsync();
        stream.Position = 0;
        return stream;
    }

    private IUploadService GetReportUploadService(ITrainDataProvider? trainDataProvider = null)
    {
        trainDataProvider ??= _fixture.Create<ITrainDataProvider>();
        return new UploadService(trainDataProvider, _mapper);
    }
}