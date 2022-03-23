using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace PictureSnapperService;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "StopAsync")]
public sealed class ScheduleImageSnapshotHostedService : IHostedService
{
    [DllImport("PictureSnapper.dll", ExactSpelling = true, SetLastError = false,
    EntryPoint = "grab_image", CallingConvention = CallingConvention.StdCall)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static extern bool GrabImage(out IntPtr image, out int imageSize);

    private Timer? _timer;
    private readonly CommandLineOptions _commandlineOptions;
    private readonly ILogger<ScheduleImageSnapshotHostedService> _logger;

    public ScheduleImageSnapshotHostedService(CommandLineOptions commandlineOptions, ILogger<ScheduleImageSnapshotHostedService> logger)
    {
        _commandlineOptions = commandlineOptions;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _commandlineOptions.Directory.Create();
        _timer = new Timer(TakeSnapshot, null, TimeSpan.Zero, _commandlineOptions.Interval);
        return Task.CompletedTask; 
    }

    private unsafe void TakeSnapshot(object? state)
    {
        var now = DateTime.Now;
        if (!GrabImage(out var imagePtr, out var imageSize))
        {
            _logger.LogInformation("Could not take snapshot.");
            return;
        }
        using var _ = new CoTaskMemAllocatedPointer(imagePtr);
        var image = new Span<byte>(imagePtr.ToPointer(), imageSize);
        var filePath = Path.Combine(_commandlineOptions.Directory.FullName, $"{now:yyyy-MM-dd HH_mm_ss}.jpeg");
        using var fw = File.OpenWrite(filePath);
        fw.Write(image);
        _logger.LogInformation("Successfully took snapshot.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await (_timer?.DisposeAsync() ?? default);
    }
}
