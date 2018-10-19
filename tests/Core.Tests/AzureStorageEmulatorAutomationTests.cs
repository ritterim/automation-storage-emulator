using System;
using System.Threading.Tasks;
using Xunit;

namespace RimDev.Automation.StorageEmulator.Tests
{
    public class AzureStorageEmulatorAutomationTests : IClassFixture<StopAzureStorageEmulatorFixture>
    {
        private readonly AzureStorageEmulatorAutomation storageEm;

        public AzureStorageEmulatorAutomationTests()
        {
            storageEm = new AzureStorageEmulatorAutomation();
        }

        [Fact]
        public void Dispose_ClosesStorageEmulatorIfStartedByAutomation()
        {
            new AzureStorageEmulatorAutomation().Stop();

            storageEm.Start();

            storageEm.Dispose();

            TestHelper.VerifyAzureStorageEmulatorIsNotRunning();
        }

        [Fact]
        public async Task Dispose_DoesNotCloseStorageEmulatorIfNotStartedByAutomation()
        {
            // Use a different instance of automation
            new AzureStorageEmulatorAutomation().Start();

            Assert.False(storageEm.StartedByAutomation, "StartedByAutomation should be false before calling dispose");

            storageEm.Dispose();

            await TestHelper.VerifyAzureStorageEmulatorIsRunningAsync();
        }

        [Fact]
        public async Task Start_StartsStorageEmulator()
        {
            new AzureStorageEmulatorAutomation().Stop();

            storageEm.Start();

            await TestHelper.VerifyAzureStorageEmulatorIsRunningAsync();
        }

        [Fact]
        public async Task Start_DoesNotThrowIfRanTwice()
        {
            storageEm.Start();

            await TestHelper.VerifyAzureStorageEmulatorIsRunningAsync();

            storageEm.Start();

            await TestHelper.VerifyAzureStorageEmulatorIsRunningAsync();
        }

        [Fact]
        public async Task Init_DoesNotClearBlobs()
        {
            const string testBlobContainer = "testcontainer";
            const string testTableName = "testtable";
            const string testQueueName = "testqueue";

            new AzureStorageEmulatorAutomation().Start();

            await TestHelper.AddTestBlobToContainerAsync(testBlobContainer);
            await TestHelper.AddTestRowToTableAsync(testTableName);
            await TestHelper.AddTestQueueItemToAsync(testQueueName);

            async Task<bool> BlobContainerContainsTestBlob() => await TestHelper.BlobContainerExistsAndContainsTestBlobAsync(testBlobContainer);
            async Task<bool> TableContainsTestRow() => await TestHelper.TableExistsAndContainsTestRowAsync(testTableName);
            async Task<bool> QueueContainsTestMessage() => await TestHelper.QueueExistsAndContainsTestMessageAsync(testQueueName);

            Assert.True(await BlobContainerContainsTestBlob());
            Assert.True(await TableContainsTestRow());
            Assert.True(await QueueContainsTestMessage());

            storageEm.Init();
            storageEm.Start();

            Assert.True(await BlobContainerContainsTestBlob());
            Assert.True(await TableContainsTestRow());
            Assert.True(await QueueContainsTestMessage());

            storageEm.ClearAll();
        }

        [Fact]
        public async Task Stop_StopsStorageEmulatorIfStartedByAutomation()
        {
            new AzureStorageEmulatorAutomation().Stop();

            storageEm.Start();

            await TestHelper.VerifyAzureStorageEmulatorIsRunningAsync();

            storageEm.Stop();

            TestHelper.VerifyAzureStorageEmulatorIsNotRunning();
        }

        [Fact]
        public void Stop_StopsStorageEmulatorIfNotStartedByAutomation()
        {
            // Use a different instance of automation
            new AzureStorageEmulatorAutomation().Start();

            storageEm.Stop();

            TestHelper.VerifyAzureStorageEmulatorIsNotRunning();
        }

        [Fact]
        public async Task ClearAll_RemovesAllData()
        {
            const string testBlobContainer = "testcontainer";
            const string testTableName = "testtable";
            const string testQueueName = "testqueue";

            storageEm.Start();

            await TestHelper.AddTestBlobToContainerAsync(testBlobContainer);
            await TestHelper.AddTestRowToTableAsync(testTableName);
            await TestHelper.AddTestQueueItemToAsync(testQueueName);

            async Task<bool> BlobContainerContainsTestBlob() => await TestHelper.BlobContainerExistsAndContainsTestBlobAsync(testBlobContainer);
            async Task<bool> TableContainsTestRow() => await TestHelper.TableExistsAndContainsTestRowAsync(testTableName);
            async Task<bool> QueueContainsTestMessage() => await TestHelper.QueueExistsAndContainsTestMessageAsync(testQueueName);

            Assert.True(await BlobContainerContainsTestBlob());
            Assert.True(await TableContainsTestRow());
            Assert.True(await QueueContainsTestMessage());

            storageEm.ClearAll();

            Assert.False(await BlobContainerContainsTestBlob());
            Assert.False(await TableContainsTestRow());
            Assert.False(await QueueContainsTestMessage());
        }

        [Fact]
        public async Task ClearBlobs_RemovesBlobData()
        {
            const string testBlobContainer = "testcontainer";

            storageEm.Start();

            await TestHelper.AddTestBlobToContainerAsync(testBlobContainer);

            async Task<bool> BlobContainerContainsTestBlob() => await TestHelper.BlobContainerExistsAndContainsTestBlobAsync(testBlobContainer);

            Assert.True(await BlobContainerContainsTestBlob());

            storageEm.ClearBlobs();

            Assert.False(await BlobContainerContainsTestBlob());
        }

        [Fact]
        public async Task ClearTables_RemovesTableData()
        {
            const string testTableName = "testtable";

            storageEm.Start();

            await TestHelper.AddTestRowToTableAsync(testTableName);

            async Task<bool> TableContainsTestRow() => await TestHelper.TableExistsAndContainsTestRowAsync(testTableName);

            Assert.True(await TableContainsTestRow());

            storageEm.ClearTables();

            Assert.False(await TableContainsTestRow());
        }

        [Fact]
        public async Task ClearQueues_RemovesQueueData()
        {
            const string testQueueName = "testqueue";

            storageEm.Start();

            await TestHelper.AddTestQueueItemToAsync(testQueueName);

            async Task<bool> QueueContainsTestMessage() => await TestHelper.QueueExistsAndContainsTestMessageAsync(testQueueName);

            Assert.True(await QueueContainsTestMessage());

            storageEm.ClearQueues();

            Assert.False(await QueueContainsTestMessage());
        }
    }
}
