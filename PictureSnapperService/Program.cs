using System.CommandLine;
using System.CommandLine.Parsing;
using System.Runtime.InteropServices;

namespace PictureSnapperService;

public static class Program
{
    [DllImport("PictureSnapper.dll", ExactSpelling = true, SetLastError = false,
        EntryPoint = "grab_image", CallingConvention = CallingConvention.StdCall)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static extern void GrabImage(out IntPtr image, out int imageSize);

    private static void Execute(DirectoryInfo directory, TimeSpan interval)
    {
        Console.WriteLine($"Hello world: {directory.FullName} in interval: {interval}");
    }

    public static unsafe void Main(string[] args)
    {
        var options = new Option[]
        {
            new Option<DirectoryInfo>(
                name: "--directory",
                description: "Directory in which to store snapshots.")
            {
                IsRequired = true
            },
            new Option<TimeSpan>(
                "--interval", arg =>
                {
                    var token = arg.Tokens[0].Value;
                    if (!TimeSpan.TryParse(token, out var ts))
                    {
                        arg.ErrorMessage = $"Could not parse {token} as timespan";
                    }
                    return ts;
                })
            {
                Arity = ArgumentArity.ExactlyOne,
                IsRequired = false,
            }
        };

        var rootCommand = new RootCommand("Tool that takes snapshots with the default web cam.");
        foreach (var option in options)
        {
            rootCommand.AddOption(option);
        }
        rootCommand.SetHandler<DirectoryInfo, TimeSpan>(Execute, options);
        rootCommand.Invoke(args);

        //GrabImage(out var imagePtr, out var imageSize);
        //using var _ = new CoTaskMemAllocatedPointer(imagePtr);

        //var image = new Span<byte>(imagePtr.ToPointer(), imageSize);
        //using var fw = File.OpenWrite(@"c:\tmp\test.jpg");
        //fw.Write(image);
        //Console.WriteLine("All finished");
    }
}
