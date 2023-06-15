namespace TrainHelper.WebApi.Dto;

public record TrainDto
{
    public int Number { get; set; }
    public string TrainIndexCombined { get; set; } = null!;
    public virtual StationDto FromStation { get; set; } = null!;
    public virtual StationDto ToStation { get; set; } = null!;
    public virtual StationDto LastStation { get; set; } = null!;
    public DateTimeOffset WhenLastOperation { get; set; }
    public string LastOperationName { get; set; } = null!;
}