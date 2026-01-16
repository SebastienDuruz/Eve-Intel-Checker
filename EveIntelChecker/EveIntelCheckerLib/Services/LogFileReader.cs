using EveIntelCheckerLib.Data;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EveIntelCheckerLib.Services
{
    public sealed class LogFileReader : ILogFileReader
    {
        private readonly object _sync = new object();
        private PeriodicTimer? _timer;
        private CancellationTokenSource? _cts;
        private Task? _loopTask;
        private long _position;
        private Encoding? _encoding;
        private string? _filePath;
        private bool _enabled;

        public event EventHandler<string>? LineRead;
        public event EventHandler? Tick;

        public string? FilePath
        {
            get
            {
                lock (_sync)
                    return _filePath;
            }
        }

        public bool IsRunning
        {
            get
            {
                lock (_sync)
                    return _timer != null;
            }
        }

        public bool IsEnabled
        {
            get
            {
                lock (_sync)
                    return _enabled;
            }
        }

        public void SetFilePath(string? filePath)
        {
            lock (_sync)
            {
                _filePath = filePath;
                ResetUnsafe();
            }
        }

        public void SetEnabled(bool enabled)
        {
            lock (_sync)
            {
                if (_enabled == enabled)
                    return;

                _enabled = enabled;
                if (_enabled)
                    ResetUnsafe();
            }
        }

        public Task StartAsync(int intervalMs, CancellationToken token = default)
        {
            lock (_sync)
            {
                if (_timer != null)
                    return Task.CompletedTask;

                _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));
                _loopTask = Task.Run(() => RunAsync(_cts.Token));
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            Task? loopTask = null;
            CancellationTokenSource? cts = null;

            lock (_sync)
            {
                if (_timer == null || _cts == null || _loopTask == null)
                    return;

                _cts.Cancel();
                _timer.Dispose();
                loopTask = _loopTask;
                cts = _cts;
                _timer = null;
                _loopTask = null;
                _cts = null;
            }

            try
            {
                await loopTask;
            }
            catch (OperationCanceledException)
            {
                // Expected on cancellation
            }
            catch (ObjectDisposedException)
            {
                // Timer disposed while waiting
            }
            catch (Exception ex)
            {
                LogsWriter.Instance.Log(StaticData.LogLevel.Warning, ex.Message);
            }
            finally
            {
                cts?.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }

        private async Task RunAsync(CancellationToken token)
        {
            try
            {
                while (await _timer!.WaitForNextTickAsync(token))
                {
                    OnTick();
                    if (IsEnabled)
                    {
                        try
                        {
                            if (TryReadLastLine(out string? line))
                                OnLineRead(line!);
                        }
                        catch (Exception ex)
                        {
                            LogsWriter.Instance.Log(StaticData.LogLevel.Error, ex.Message);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on cancellation
            }
            catch (ObjectDisposedException)
            {
                // Timer disposed while waiting
            }
        }

        private bool TryReadLastLine(out string? lastLine)
        {
            lastLine = null;
            string? filePath;
            long position;
            Encoding? encoding;

            lock (_sync)
            {
                filePath = _filePath;
                position = _position;
                encoding = _encoding;
            }

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (position > stream.Length)
                position = 0;

            stream.Seek(position, SeekOrigin.Begin);
            using StreamReader reader = encoding == null
                ? new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true)
                : new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: false, leaveOpen: true);

            string? line;
            while ((line = reader.ReadLine()) != null)
                lastLine = line;

            if (encoding == null)
                encoding = reader.CurrentEncoding;

            position = stream.Position;
            lock (_sync)
            {
                _position = position;
                _encoding = encoding;
            }

            return lastLine != null;
        }

        private void ResetUnsafe()
        {
            _position = 0;
            _encoding = null;
        }

        private void OnLineRead(string line)
        {
            try
            {
                LineRead?.Invoke(this, line);
            }
            catch (Exception ex)
            {
                LogsWriter.Instance.Log(StaticData.LogLevel.Error, ex.Message);
            }
        }

        private void OnTick()
        {
            try
            {
                Tick?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                LogsWriter.Instance.Log(StaticData.LogLevel.Error, ex.Message);
            }
        }
    }
}
