namespace FileManager_Server
{
    public class FileInformation
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public DateTime Creation {  get; set; }
        public long Size { get; set; }
        public FileAttributes Attributes { get; set; }
    }
}
