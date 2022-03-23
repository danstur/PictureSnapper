using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.NamingConventionBinder;

namespace PictureSnapperService;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used via reflection.")]
public sealed record CommandLineOptions(DirectoryInfo Directory, TimeSpan Interval);

public static class Program
{
    private static async Task Run(CommandLineOptions options)
    {
        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(options);
                services.AddHostedService<ScheduleImageSnapshotHostedService>();
            })
            .Build()
            .RunAsync();
    }

    public static async Task Main(string[] args)
    {
        var directoryOption = new Option<DirectoryInfo>(
                name: "--directory",
                description: "Directory in which to store snapshots.")
        {
            IsRequired = true
        };
        var intervalOption = new Option<TimeSpan>(
                "--interval",
                arg =>
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
        };
        intervalOption.SetDefaultValue(TimeSpan.FromMinutes(5));

        var rootCommand = new RootCommand("Tool that takes snapshots with the default web cam.")
        {
            directoryOption,
            intervalOption,
        };
        rootCommand.Handler = CommandHandler.Create(Run);
        await rootCommand.InvokeAsync(args);
    }
}
