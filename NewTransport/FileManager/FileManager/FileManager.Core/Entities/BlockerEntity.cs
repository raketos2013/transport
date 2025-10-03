namespace FileManager.Core.Entities;

public class BlockerEntity
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Table {  get; set; }
    public string TableId { get; set; }
    public string User {  get; set; }
}
