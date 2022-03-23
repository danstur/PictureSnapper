namespace PictureSnapperService;

public sealed record CommandLineOptions(DirectoryInfo Directory, TimeSpan Interval);
