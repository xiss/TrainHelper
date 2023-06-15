namespace TrainHelper.WebApi.Dto;

public record CarDto
{
    public int PositionInTrain { get; init; }
    public int CarNumber { get; init; }
    public string InvoiceNumber { get; init; } = null!;
    public DateTimeOffset WhenLastOperation { get; init; }
    public string FreightEtsngName { get; init; } = null!;
    public double FreightTotalWeightTn { get; init; }
    public string LastOperationName { get; init; } = null!;
}
