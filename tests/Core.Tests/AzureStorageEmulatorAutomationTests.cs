using System;
using Xunit;

namespace RimDev.Automation.StorageEmulator.Tests
{
    public class AzureStorageEmulatorAutomationTests : IClassFixture<StopAzureStorageEmulatorFixture>
    {
        private readonly AzureStorageEmulatorAutomation _sut;

        public AzureStorageEmulatorAutomationTests()
        {
            _sut = new AzureStorageEmulatorAutomation();
        }

        [Fact]
        public void Dispose_ClosesStorageEmulatorIfStartedByAutomation()
        {
            new AzureStorageEmulatorAutomation().Stop();

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
            new AzureStorageEmulatorAutomation().Stop();

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
        public void Init_DoesNotClearBlobs()
        {
            const string TestBlobContainer = "testcontainer";
            const string TestTableName = "testtable";
            const string TestQueueName = "testqueue";

            new AzureStorageEmulatorAutomation().Start();

            TestHelper.AddTestBlobToContainer(TestBlobContainer);
            TestHelper.AddTestRowToTable(TestTableName);
            TestHelper.AddTestQueueItemTo(TestQueueName);

            Func<bool> blobContainerContainsTestBlob =
                () => TestHelper.BlobContainerExistsAndContainsTestBlob(TestBlobContainer);
            Func<bool> tableContainsTestRow =
                () => TestHelper.TableExistsAndContainsTestRow(TestTableName);
            Func<bool> queueContainsTestMessage =
                () => TestHelper.QueueExistsAndContainsTestMessage(TestQueueName);

            Assert.True(blobContainerContainsTestBlob());
            Assert.True(tableContainsTestRow());
            Assert.True(queueContainsTestMessage());

            _sut.Init();
            _sut.Start();

            Assert.True(blobContainerContainsTestBlob());
            Assert.True(tableContainsTestRow());
            Assert.True(queueContainsTestMessage());

            _sut.ClearAll();
        }

        [Fact]
        public void Stop_StopsStorageEmulatorIfStartedByAutomation()
        {
            new AzureStorageEmulatorAutomation().Stop();

            _sut.Start();

            TestHelper.VerifyAzureStorageEmulatorIsRunning();

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
            const string TestBlobContainer = "testcontainer";
            const string TestTableName = "testtable";
            const string TestQueueName = "testqueue";

            _sut.Start();

            TestHelper.AddTestBlobToContainer(TestBlobContainer);
            TestHelper.AddTestRowToTable(TestTableName);
            TestHelper.AddTestQueueItemTo(TestQueueName);

            Func<bool> blobContainerContainsTestBlob =
                () => TestHelper.BlobContainerExistsAndContainsTestBlob(TestBlobContainer);
            Func<bool> tableContainsTestRow =
                () => TestHelper.TableExistsAndContainsTestRow(TestTableName);
            Func<bool> queueContainsTestMessage =
                () => TestHelper.QueueExistsAndContainsTestMessage(TestQueueName);

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
            const string TestBlobContainer = "testcontainer";

            _sut.Start();

            TestHelper.AddTestBlobToContainer(TestBlobContainer);

            Func<bool> blobContainerContainsTestBlob =
                () => TestHelper.BlobContainerExistsAndContainsTestBlob(TestBlobContainer);

            Assert.True(blobContainerContainsTestBlob());

            _sut.ClearBlobs();

            Assert.False(blobContainerContainsTestBlob());
        }

        [Fact]
        public void ClearTables_RemovesTableData()
        {
            const string TestTableName = "testtable";

            _sut.Start();

            TestHelper.AddTestRowToTable(TestTableName);

            Func<bool> tableContainsTestRow =
                () => TestHelper.TableExistsAndContainsTestRow(TestTableName);

            Assert.True(tableContainsTestRow());

            _sut.ClearTables();

            Assert.False(tableContainsTestRow());
        }

        [Fact]
        public void ClearQueues_RemovesQueueData()
        {
            const string TestQueueName = "testqueue";

            _sut.Start();

            TestHelper.AddTestQueueItemTo(TestQueueName);

            Func<bool> queueContainsTestMessage =
                () => TestHelper.QueueExistsAndContainsTestMessage(TestQueueName);

            Assert.True(queueContainsTestMessage());

            _sut.ClearQueues();

            Assert.False(queueContainsTestMessage());
        }
    }
}
