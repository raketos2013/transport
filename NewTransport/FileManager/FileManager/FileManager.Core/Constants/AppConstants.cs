using System.Text.Json.Serialization;
using System.Text.Json;

namespace FileManager.Core.Constants;

public class AppConstants
{
    public const string BUFFER_FILE_MASK = "{BUFFER}";
    public static readonly JsonSerializerOptions JSON_OPTIONS = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    public const string ROLE_DEFAULT = "o.br.ДИТ";
    public const string ROLE_ADMIN = "";
}
