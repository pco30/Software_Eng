using System;
using System.Threading;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    internal static class Program
    {
        private static Mutex mutex = new Mutex(true, "WinChangeMonitor");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new WinChangeMonitorForm());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("WinChangeMonitor is already running. Please close it before running another copy.", "Multiple Copies Detected", MessageBoxButtons.OK);
                Application.Exit();
            }
        }
    }
}
