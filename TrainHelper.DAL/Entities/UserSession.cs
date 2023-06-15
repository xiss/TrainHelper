namespace TrainHelper.DAL.Entities;

public class UserSession
{
    public int Id { get; set; }
    public Guid RefreshTokenId { get; set; }
    public virtual User User { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public bool IsActive { get; set; } = true;
}