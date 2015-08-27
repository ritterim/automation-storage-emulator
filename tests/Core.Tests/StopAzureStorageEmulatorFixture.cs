using System;

namespace RimDev.Automation.StorageEmulator.Tests
{
    public class StopAzureStorageEmulatorFixture : IDisposable
    {
        public StopAzureStorageEmulatorFixture()
        {
            var automation = new AzureStorageEmulatorAutomation();

            automation.Stop();

            TestHelper.VerifyAzureStorageEmulatorIsNotRunning();
        }

        public void Dispose()
        {
        }
    }
}
