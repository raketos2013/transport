namespace FileManager.Core.Entities;

public class LockInfoEntity
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string EntityId { get; set; }
    public DateTime Created { get; set; }
}
