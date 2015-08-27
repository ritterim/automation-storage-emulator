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
        public void ClearAll_RemovesAllData()
        {
            _sut.Start();

            TestHelper.AddTestBlobToContainer("testcontainer");
            TestHelper.AddTestRowToTable("testtable");
            TestHelper.AddTestQueueItemTo("testqueue");

            Func<bool> blobContainerContainsTestBlob =
                () => TestHelper.BlobContainerExistsAndContainsTestBlob("testcontainer");
            Func<bool> tableContainsTestRow =
                () => TestHelper.TableExistsAndContainsTestRow("testtable");
            Func<bool> queueContainsTestMessage =
                () => TestHelper.QueueExistsAndContainsTestMessage("testqueue");

            Assert.True(blobContainerContainsTestBlob());
            Assert.True(tableContainsTestRow());
            Assert.True(queueContainsTestMessage());

            _sut.ClearAll();

            Assert.False(blobContainerContainsTestBlob());
            Assert.False(tableContainsTestRow());
            Assert.False(queueContainsTestMessage());
        }

        [Fact]
        public void ClearBlobs_RemovesBlobData()
        {
            _sut.Start();

            TestHelper.AddTestBlobToContainer("testcontainer");

            Func<bool> blobContainerContainsTestBlob =
                () => TestHelper.BlobContainerExistsAndContainsTestBlob("testcontainer");

            Assert.True(blobContainerContainsTestBlob());

            _sut.ClearBlobs();

            Assert.False(blobContainerContainsTestBlob());
        }

        [Fact]
        public void ClearTables_RemovesTableData()
        {
            _sut.Start();

            TestHelper.AddTestRowToTable("testtable");

            Func<bool> tableContainsTestRow =
                () => TestHelper.TableExistsAndContainsTestRow("testtable");

            Assert.True(tableContainsTestRow());

            _sut.ClearTables();

            Assert.False(tableContainsTestRow());
        }

        [Fact]
        public void ClearQueues_RemovesQueueData()
        {
            _sut.Start();

            TestHelper.AddTestQueueItemTo("testqueue");

            Func<bool> queueContainsTestMessage =
                () => TestHelper.QueueExistsAndContainsTestMessage("testqueue");

            Assert.True(queueContainsTestMessage());

            _sut.ClearQueues();

            Assert.False(queueContainsTestMessage());
        }
    }
}
