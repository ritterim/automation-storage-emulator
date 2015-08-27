using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;

namespace RimDev.Automation.StorageEmulator.Tests
{
    public class TestHelper
    {
        private const string TestBlobName = "TestBlob";
        private const string TestQueueMessage = "test-queue-message";

        private static readonly CloudStorageAccount StorageAccount =
            CloudStorageAccount.DevelopmentStorageAccount;

        public static void VerifyAzureStorageEmulatorIsRunning()
        {
            var tableName = "test" + Guid.NewGuid().ToString().Replace("-", "");
            var cloudTable = GetCloudTable(tableName);

            var tableExists = cloudTable.Exists();
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

        public static void AddTestRowToTable(string tableName)
        {
            var cloudTable = GetCloudTable(tableName);
            cloudTable.CreateIfNotExists();

            var testTableEntity = new TestTableEntity
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString(),
                TestProperty = "test"
            };

            cloudTable.Execute(TableOperation.Insert(testTableEntity));
        }

        public static void AddTestBlobToContainer(string blobContainer)
        {
            var cloudBlobContainer = GetCloudBlobContainer(blobContainer);
            cloudBlobContainer.CreateIfNotExists();

            var blockBlob = cloudBlobContainer.GetBlockBlobReference(TestBlobName);

            blockBlob.UploadText("test");
        }

        public static void AddTestQueueItemTo(string queueName)
        {
            var cloudQueue = GetCloudQueue(queueName);
            cloudQueue.CreateIfNotExists();

            var cloudQueueMessage = new CloudQueueMessage(TestQueueMessage);

            cloudQueue.AddMessage(cloudQueueMessage);
        }

        public static bool BlobContainerExistsAndContainsTestBlob(string blobContainer)
        {
            var cloudBlobContainer = GetCloudBlobContainer(blobContainer);

            if (!cloudBlobContainer.Exists())
            {
                return false;
            }

            var cloudBlob = cloudBlobContainer.GetBlobReferenceFromServer(TestBlobName);
            return cloudBlob.Exists();
        }

        public static bool TableExistsAndContainsTestRow(string tableName)
        {
            var cloudTable = GetCloudTable(tableName);

            if (!cloudTable.Exists())
            {
                return false;
            }

            var results = cloudTable.ExecuteQuery(new TableQuery());

            if (results.Count() > 1)
            {
                throw new ApplicationException(string.Format(
                    "count of test rows was {0}.",
                    results.Count()));
            }

            return results.Count() == 1;
        }

        public static bool QueueExistsAndContainsTestMessage(string queueName)
        {
            var cloudQueue = GetCloudQueue(queueName);

            if (!cloudQueue.Exists())
            {
                return false;
            }

            var queueMessage = cloudQueue.GetMessage();

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
