using System;
using Xunit;

namespace RimDev.Automation.StorageEmulator.Tests
{
    public class AzureStorageEmulatorAutomationTests : StopAzureStorageEmulatorFixture
    {
        private readonly AzureStorageEmulatorAutomation _sut;

        public AzureStorageEmulatorAutomationTests()
        {
            _sut = new AzureStorageEmulatorAutomation();
        }

        [Fact]
        public void Dispose_ClosesStorageEmulatorIfStartedByAutomation()
        {
            _sut.Start();

            _sut.Dispose();

            TestHelper.VerifyAzureStorageEmulatorIsNotRunning();
        }

        [Fact]
        public void Dispose_DoesNotCloseStorageEmulatorIfNotStartedByAutomation()
        {
            // Use a different instance of automation
            new AzureStorageEmulatorAutomation().Start();

            Assert.False(_sut.StartedByAutomation, "StartedByAutomation should be false before calling dispose");

            _sut.Dispose();

            TestHelper.VerifyAzureStorageEmulatorIsRunning();
        }

        [Fact]
        public void Start_StartsStorageEmulator()
        {
            _sut.Start();

            TestHelper.VerifyAzureStorageEmulatorIsRunning();
        }

        [Fact]
        public void Start_DoesNotThrowIfRanTwice()
        {
            _sut.Start();

            TestHelper.VerifyAzureStorageEmulatorIsRunning();

            _sut.Start();

            TestHelper.VerifyAzureStorageEmulatorIsRunning();
        }

        [Fact]
        public void Stop_StopsStorageEmulatorIfStartedByAutomation()
        {
            _sut.Start();

            _sut.Stop();

            TestHelper.VerifyAzureStorageEmulatorIsNotRunning();
        }

        [Fact]
        public void Stop_StopsStorageEmulatorIfNotStartedByAutomation()
        {
            // Use a different instance of automation
            new AzureStorageEmulatorAutomation().Start();

            _sut.Stop();

            TestHelper.VerifyAzureStorageEmulatorIsNotRunning();
        }

        [Fact]
        public void ClearTables_RemovesTableData()
        {
            _sut.Start();

            TestHelper.AddTestRowToTable("TestTable");

            Func<bool> tableContainsTestRow =
                () => TestHelper.TableExistsAndContainsTestRow("TestTable");

            Assert.True(tableContainsTestRow());

            _sut.ClearTables();

            Assert.False(tableContainsTestRow());
        }
    }
}
