using System.Diagnostics;

namespace RimDev.Automation.StorageEmulator
{
    public class ProcessHelper
    {
        public static string RunAndGetStandardOutputAsString(string path, string parameter)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(path, parameter)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            process.Start();

            var sr = process.StandardOutput;
            var output = sr.ReadToEnd();

            process.WaitForExit();

            return output;
        }
    }
}
