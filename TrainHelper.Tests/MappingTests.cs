using AutoFixture;
using AutoMapper;
using TrainHelper.DAL.Entities;
using TrainHelper.WebApi.Dto;

namespace TrainHelper.WebApi.Tests;

public class MappingTests
{
    private readonly IFixture _fixture;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        _mapper = mapperConfiguration.CreateMapper();
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void CarToCarDto_Correct()
    {
        // Arrange
        var expected = _fixture.Create<Car>();

        // Act
        var result = _mapper.Map<CarDto>(expected);

        // Assert
        Assert.Equal(expected.PositionInTrain, result.PositionInTrain);
        Assert.Equal(expected.CarNumber, result.CarNumber);
        Assert.Equal(expected.Invoice.InvoiceNum, result.InvoiceNumber);
        Assert.Equal(expected.WayPoints.Max(w => w.OperationDate), result.WhenLastOperation);
        Assert.Equal(expected.Freight.FreightEtsngName, result.FreightEtsngName);
        Assert.Equal((double)expected.FreightTotalWeightKg / 1000, result.FreightTotalWeightTn);
        Assert.Equal(expected.WayPoints.OrderByDescending(w => w.OperationDate).First().Operation.OperationName, result.LastOperationName);
    }

    [Fact]
    public void ItemToCarTest_Correct()
    {
        // Arrange
        var expected = _fixture
            .Build<UploadDataTrainDto.Item>()
            .With(c => c.WhenLastOperationForXml, _fixture.Create<DateTime>().ToString)
            .Create();

        // Act
        var result = _mapper.Map<Car>(expected);

        // Assert
        Assert.Equal(expected.CarNumber, result.CarNumber);
        Assert.Equal(expected.FreightEtsngName, result.Freight.FreightEtsngName);
        Assert.Equal((int)expected.FreightTotalWeightKg, result.FreightTotalWeightKg);
        Assert.Equal(expected.PositionInTrain, result.PositionInTrain);
        Assert.Equal(expected.InvoiceNum, result.Invoice.InvoiceNum);
    }

    [Fact]
    public void Mapper_AllDestinationsFilledOrIgnored() => _mapper.ConfigurationProvider.AssertConfigurationIsValid();
}