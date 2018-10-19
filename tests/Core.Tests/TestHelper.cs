using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace RimDev.Automation.StorageEmulator.Tests
{
    public class TestHelper
    {
        private const string TestBlobName = "TestBlob";
        private const string TestQueueMessage = "test-queue-message";

        private static readonly CloudStorageAccount StorageAccount =
            CloudStorageAccount.DevelopmentStorageAccount;

        public static async Task VerifyAzureStorageEmulatorIsRunningAsync()
        {
            var tableName = "test" + Guid.NewGuid().ToString().Replace("-", "");
            var cloudTable = GetCloudTable(tableName);

            var tableExists = await cloudTable.ExistsAsync();
            if (tableExists)
            {
                throw new ApplicationException("Table unexpectedly exists.");
            }
        }

        public static void VerifyAzureStorageEmulatorIsNotRunning()
        {
            var isRunning = AzureStorageEmulatorAutomation.IsEmulatorRunning();

            if (isRunning)
            {
                throw new ApplicationException("The Azure Storage Emulator is running.");
            }
        }

        public static async Task AddTestRowToTableAsync(string tableName)
        {
            var cloudTable = GetCloudTable(tableName);
            await cloudTable.CreateIfNotExistsAsync();

            var testTableEntity = new TestTableEntity
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString(),
                TestProperty = "test"
            };

            await cloudTable.ExecuteAsync(TableOperation.Insert(testTableEntity));
        }

        public static async Task AddTestBlobToContainerAsync(string blobContainer)
        {
            var cloudBlobContainer = GetCloudBlobContainer(blobContainer);
            await cloudBlobContainer.CreateIfNotExistsAsync();

            var blockBlob = cloudBlobContainer.GetBlockBlobReference(TestBlobName);

            await blockBlob.UploadTextAsync("test");
        }

        public static async Task AddTestQueueItemToAsync(string queueName)
        {
            var cloudQueue = GetCloudQueue(queueName);
            await cloudQueue.CreateIfNotExistsAsync();

            var cloudQueueMessage = new CloudQueueMessage(TestQueueMessage);

            await cloudQueue.AddMessageAsync(cloudQueueMessage);
        }

        public static async Task<bool> BlobContainerExistsAndContainsTestBlobAsync(string blobContainer)
        {
            var cloudBlobContainer = GetCloudBlobContainer(blobContainer);

            if (!await cloudBlobContainer.ExistsAsync())
            {
                return false;
            }

            var cloudBlob = await cloudBlobContainer.GetBlobReferenceFromServerAsync(TestBlobName);
            return await cloudBlob.ExistsAsync();
        }

        public static async Task<bool> TableExistsAndContainsTestRowAsync(string tableName)
        {
            var cloudTable = GetCloudTable(tableName);

            if (!await cloudTable.ExistsAsync())
            {
                return false;
            }

            var results = (await cloudTable.ExecuteQuerySegmentedAsync(new TableQuery(), new TableContinuationToken())).Results;

            if (results?.Count > 1 )
            {
                throw new ApplicationException(string.Format(
                    "count of test rows was {0}.",
                    results?.Count));
            }

            return results?.Count == 1;
        }

        public static async Task<bool> QueueExistsAndContainsTestMessageAsync(string queueName)
        {
            var cloudQueue = GetCloudQueue(queueName);

            if (!await cloudQueue.ExistsAsync())
            {
                return false;
            }

            var queueMessage = await cloudQueue.PeekMessageAsync();

            if (queueMessage != null && queueMessage.AsString == TestQueueMessage)
            {
                return true;
            }

            return false;
        }

        private static CloudTable GetCloudTable(string tableName)
        {
            var tableClient = StorageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(tableName);
            return table;
        }

        private static CloudBlobContainer GetCloudBlobContainer(string containerName)
        {
            var blobClient = StorageAccount.CreateCloudBlobClient();

            var blobContainer = blobClient.GetContainerReference(containerName);
            return blobContainer;
        }

        private static CloudQueue GetCloudQueue(string queueName)
        {
            var queueClient = StorageAccount.CreateCloudQueueClient();

            var cloudQueue = queueClient.GetQueueReference(queueName);
            return cloudQueue;
        }

        private class TestTableEntity : TableEntity
        {
            public string TestProperty { get; set; }
        }
    }
}
