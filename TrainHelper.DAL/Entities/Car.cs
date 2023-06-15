namespace TrainHelper.DAL.Entities;

public class Car
{
    public int Id { get; set; }
    public int CarNumber { get; set; }
    public virtual Train Train { get; set; } = null!;
    public Freight Freight { get; set; } = null!;
    public int FreightTotalWeightKg { get; set; }
    public int PositionInTrain { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public virtual Station FromStation { get; set; } = null!;
    public virtual Station ToStation { get; set; } = null!;
    public List<WayPoint> WayPoints { get; set; } = new();
}
