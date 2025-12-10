using System.Text.Json.Serialization;

namespace FileManager.Core.ViewModels;

public class SapUserRequest
{
    [JsonPropertyName("i_pernr")]
    public string Pernr {  get; set; }
}
