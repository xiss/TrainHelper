namespace TrainHelper.DAL.Entities;

public class Train
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string TrainIndexCombined { get; set; } = null!;
    public virtual List<Car> Cars { get; set; } = new List<Car>();
}