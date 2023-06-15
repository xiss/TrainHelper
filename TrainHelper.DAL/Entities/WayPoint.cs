namespace TrainHelper.DAL.Entities;

public class WayPoint
{
    public int Id { get; set; }
    public Station Station { get; set; } = null!;
    public DateTimeOffset OperationDate { get; set; }
    public Car Car { get; set; } = null!;
    public CarOperation Operation { get; set; } = null!;
}
