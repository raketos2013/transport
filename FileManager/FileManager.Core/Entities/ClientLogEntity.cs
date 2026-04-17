using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileManager.Core.Entities;

public class ClientLogEntity
{
    [Key]
    public long Id { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Values { get; set; }

    public DateTime Created { get; set; }
}
