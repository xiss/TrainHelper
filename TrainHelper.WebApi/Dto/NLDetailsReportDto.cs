namespace TrainHelper.WebApi.Dto;

public class NlDetailsReportDto
{
    public int TrainNumber { get; set; }
    public string TailNumber { get; set; } = null!;
    public string LastStation { get; set; } = null!;
    public DateTimeOffset WhenLastOperation { get; set; }
    public List<NlDetailsReportSubtotal> Subtotals { get; set; } = new();
    public List<CarDto> Cars { get; set; } = new();
    public record NlDetailsReportSubtotal(int CarsCount, string FreightEtsngName, double FreightTotalWeightTn);
}