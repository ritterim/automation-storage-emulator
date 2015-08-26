using System;
using System.IO;

namespace RimDev.Automation.StorageEmulator
{
    public class AzureStorageEmulatorAutomation : IDisposable
    {
        public bool StartedByAutomation { get; private set; }

        public void Dispose()
        {
            if (StartedByAutomation)
            {
                Stop();
            }
        }

        public void Start()
        {
            if (!IsEmulatorRunning())
            {
                RunWithParameter("start");
                StartedByAutomation = true;
            }
        }

        public void Stop()
        {
            RunWithParameter("stop");
        }

        public void ClearTables()
        {
            RunWithParameter("clear table");
        }

        public static string GetPathToStorageEmulatorExecutable()
        {
            var paths = new[]
            {
                Path.Combine(AzureSdkDirectory, @"Storage Emulator\AzureStorageEmulator.exe"),
                Path.Combine(AzureSdkDirectory, @"Storage Emulator\WAStorageEmulator.exe")
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            throw new FileNotFoundException(
                "Unable to locate Azure storage emulator at any of the expected paths.",
                string.Join(", ", paths));
        }

        public static bool IsEmulatorRunning()
        {
            var path = GetPathToStorageEmulatorExecutable();

            var output = ProcessHelper.RunAndGetStandardOutputAsString(path, "status");

            if (output.Contains("IsRunning: True"))
            {
                return true;
            }
            else if (output.Contains("IsRunning: False"))
            {
                return false;
            }

            throw new ApplicationException("Unable to determine if Azure Storage Emulator is running.");
        }

        private static void RunWithParameter(string parameter)
        {
            var path = GetPathToStorageEmulatorExecutable();

            ProcessHelper.RunAndGetStandardOutputAsString(path, parameter);
        }

        private static string AzureSdkDirectory
        {
            get
            {
                var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                var path = Path.Combine(programFilesX86, @"Microsoft SDKs\Azure");

                return path;
            }
        }
    }
}
