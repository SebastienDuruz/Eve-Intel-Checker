using System;
using System.Threading;
using System.Threading.Tasks;

namespace EveIntelCheckerLib.Services
{
    public interface ILogFileReader : IAsyncDisposable
    {
        event EventHandler<string>? LineRead;
        event EventHandler? Tick;

        string? FilePath { get; }
        bool IsRunning { get; }
        bool IsEnabled { get; }

        void SetFilePath(string? filePath);
        void SetEnabled(bool enabled);
        Task StartAsync(int intervalMs, CancellationToken token = default);
        Task StopAsync();
    }
}
