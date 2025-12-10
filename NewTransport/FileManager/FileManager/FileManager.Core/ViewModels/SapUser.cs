using System.Text.Json.Serialization;

namespace FileManager.Core.ViewModels;

public class SapUser
{
    [JsonPropertyName("pernr")]
    public string PersNumber { get; set; }
    public string Orgeh {  get; set; }
    public string Fio {  get; set; }
}
