using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;

namespace RimDev.Automation.StorageEmulator.Tests
{
    public class TestHelper
    {
        private static readonly CloudStorageAccount StorageAccount =
            CloudStorageAccount.DevelopmentStorageAccount;

        public static void VerifyAzureStorageEmulatorIsRunning()
        {
            var tableName = "Test" + Guid.NewGuid().ToString().Replace("-", "");
            var cloudTable = GetCloudTable(tableName);

            var tableExists = cloudTable.Exists();
            if (tableExists)
            {
                throw new ApplicationException("Table unexpectedly exists.");
            }
        }

        private static CloudTable GetCloudTable(string tableName)
        {
            var tableClient = StorageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(tableName);
            return table;
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

        private class TestTableEntity : TableEntity
        {
            public string TestProperty { get; set; }
        }
    }
}
