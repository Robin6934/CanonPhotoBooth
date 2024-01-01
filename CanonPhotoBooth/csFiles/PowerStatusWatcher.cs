using PowerStatus;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace PhotoBooth
{
    class PowerStatusWatcher
    {
        private static CancellationTokenSource? cancellationTokenSource;

        public static async Task StartDCWatcher()
        {
            cancellationTokenSource = new CancellationTokenSource();

            var statusProvider = new PowerStatusProvider();

            AcLineStatus? oldStatus = new AcLineStatus();

            oldStatus = AcLineStatus.Offline;

            PowerStatus.PowerStatus? status = null;

            await Task.Run(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        if(status is not null) { oldStatus = status.AcLineStatus; }

                        status = statusProvider.GetStatus();

                        if (status?.AcLineStatus == AcLineStatus.Offline && oldStatus == AcLineStatus.Online)
                        {
                            //ShutdownSystem();
                        }

                        await Task.Delay(5000, cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions (logging, etc.)
                        Debug.WriteLine($"Error: {ex.Message}");
                    }
                }
            });
        }

        public static void StopWatcher()
        {
            cancellationTokenSource?.Cancel();
        }

        private static void ShutdownSystem()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;

                // Check if the MainWindow is not null before closing
                if (mainWindow != null)
                {
                    mainWindow.Close();
                }
            });

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/s /t 5",
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            Process.Start(psi);
        }
    }
}
