namespace FileManagerServer;

public class JsonResponse
{
    public int Code { get; set; } = 0;
    public string ExceptionName { get; set; } = "";
    public string[]? Message { get; set; }
    public string Path { get; set; } = "";
}
