public record Rule()
{
    public string Name { get; init; }
    public string SourcePath { get; init; }
    public string DestinationHost { get; init; }
    public string DestinationUserName { get; init; }
    public string DestinationPassword { get; init; }
    public string DestinationPath { get; init;}
    public int DeleteOldFilesAfterExceedFileSizesInGb { get; set; }
    public string[] AllowedFileExtensions { get; init; }
    public string[] ProhibitedFileExtensions { get; init; }
}
