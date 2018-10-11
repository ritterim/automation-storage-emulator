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

        /// <summary>
        /// Do one-time initialization needed by the emulator.
        /// Use this before <see cref="Start"/> if you find error "The storage emulator needs to be initialized. Please run the 'init' command."
        /// </summary>
        public static void Init()
        {
            if (!IsEmulatorRunning())
            {
                RunWithParameter("init");
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

        public void ClearAll()
        {
            RunWithParameter("clear all");
        }

        public void ClearBlobs()
        {
            RunWithParameter("clear blob");
        }

        public void ClearTables()
        {
            RunWithParameter("clear table");
        }

        public void ClearQueues()
        {
            RunWithParameter("clear queue");
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

        private static string GetPathToStorageEmulatorExecutable()
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
    }
}
