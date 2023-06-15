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
    public void CreateUserDtoToUser_Correct()
    {
        // Arrange
        var expected = _fixture.Create<CreateUserDto>();

        // Act
        var result = _mapper.Map<User>(expected);

        // Assert
        Assert.Equal(expected.Email, result.Email);
        Assert.Equal(expected.Name, result.Name);
        Assert.Equal(expected.Patronymic, result.Patronymic);
        Assert.Equal(expected.Surname, result.Surname);
    }

    [Fact]
    public void InvoiceToInvoiceDto_Correct()
    {
        // Arrange
        var expected = _fixture.Create<Invoice>();

        // Act
        var result = _mapper.Map<InvoiceDto>(expected);

        // Assert
        Assert.Equal(expected.InvoiceNum, result.InvoiceNum);
    }

    [Fact]
    public void ItemToCarOperation_Correct()
    {
        // Arrange
        var expected = GetItem();

        // Act
        var result = _mapper.Map<CarOperation>(expected);

        // Assert
        Assert.Equal(expected.LastOperationName, result.OperationName);
    }

    [Fact]
    public void ItemToCarTest_Correct()
    {
        // Arrange
        var expected = GetItem();

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
    public void ItemToFreight_Correct()
    {
        // Arrange
        var expected = GetItem();

        // Act
        var result = _mapper.Map<Freight>(expected);

        // Assert
        Assert.Equal(expected.FreightEtsngName, result.FreightEtsngName);
    }

    [Fact]
    public void ItemToInvoice_Correct()
    {
        // Arrange
        var expected = GetItem();

        // Act
        var result = _mapper.Map<Invoice>(expected);

        // Assert
        Assert.Equal(expected.InvoiceNum, result.InvoiceNum);
    }

    [Fact]
    public void ItemToTrain_Correct()
    {
        // Arrange
        var expected = GetItem();

        // Act
        var result = _mapper.Map<Train>(expected);

        // Assert
        Assert.Equal(expected.TrainNumber, result.Number);
        Assert.Empty(result.Cars);
    }

    [Fact]
    public void ItemToWayPoint_Correct()
    {
        // Arrange
        var expected = GetItem();

        // Act
        var result = _mapper.Map<WayPoint>(expected);

        // Assert
        Assert.Equal(expected.WhenLastOperation.UtcDateTime, result.OperationDate);
        Assert.Equal(expected.LastOperationName, result.Operation.OperationName);
    }

    [Fact]
    public void Mapper_AllDestinationsFilledOrIgnored() => _mapper.ConfigurationProvider.AssertConfigurationIsValid();

    [Fact]
    public void StationToStationDto_Correct()
    {
        // Arrange
        var expected = _fixture.Create<Station>();

        // Act
        var result = _mapper.Map<StationDto>(expected);

        // Assert
        Assert.Equal(expected.StationName, result.StationName);
    }

    [Fact]
    public void TrainToTrainDto_Correct()
    {
        // Arrange
        var expected = _fixture.Create<Train>();

        // Act
        var result = _mapper.Map<TrainDto>(expected);

        // Assert
        Assert.Equal(expected.Number, result.Number);
        Assert.Equal(expected.TrainIndexCombined, result.TrainIndexCombined);
    }

    [Fact]
    public void UserToUserDto_Correct()
    {
        // Arrange
        var expected = _fixture.Create<User>();

        // Act
        var result = _mapper.Map<UserDto>(expected);

        // Assert
        Assert.Equal(expected.Email, result.Email);
        Assert.Equal(expected.Name, result.Name);
        Assert.Equal(expected.Patronymic, result.Patronymic);
        Assert.Equal(expected.Surname, result.Surname);
        Assert.Equal(expected.Id, result.Id);
    }

    private UploadDataTrainDto.Item GetItem() => _fixture
        .Build<UploadDataTrainDto.Item>()
        .With(c => c.WhenLastOperationForXml, _fixture.Create<DateTime>().ToString)
        .Create();
}