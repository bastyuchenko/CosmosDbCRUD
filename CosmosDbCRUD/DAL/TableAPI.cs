using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CosmosDbCRUD
{
    public class TableAPI :IDisposable
    {
        CloudTable table;
        string tableName = "coll";
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private const string PartitionValue1 = "XMS-0001";
        private const string PartitionValue2 = "XMS-0002";

        public async Task CreateCollection()
        {
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=5507cc14-0ee0-4-231-b9ee;AccountKey=O863GbJvw4lQbK40qyvOdVxGHIuhJsTrP4BifkJPnmJwoe7h7cJYlYdcNqa7VarzrreWvPMwSo4YnyqbghgKfg==;TableEndpoint=https://5507cc14-0ee0-4-231-b9ee.documents.azure.com:443/");
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(tableName);

            try
            {
                await table.CreateIfNotExistsAsync();
            }
            catch (Exception)
            {

            }

        }

        public Task CreateDatabase()
        {
            throw new NotImplementedException("It's not possible to create db within cosmosdb account.");
        }

        public async Task CreateItems()
        {
            var batch = new TableBatchOperation();
            for (int i = 0; i < 10; i++)
            {
                batch.Insert((ITableEntity)new TableDeviceReading()
                {
                    RowKey = $"XMS-001-FE2{i}C",
                    PartitionKey = PartitionValue1,
                    MetricType = "Temperature",
                    MetricValue = 105.00,
                    Unit = "Fahrenheit",
                    ReadingTime = DateTime.UtcNow
                });
            }
            var result = await table.ExecuteBatchAsync(batch);

            batch = new TableBatchOperation();
            for (int i = 0; i < 8; i++)
            {
                batch.Insert((ITableEntity)new TableDeviceReading()
                {
                    RowKey = $"XMS-002-FE2{i}C",
                    PartitionKey = PartitionValue2,
                    MetricType = "Temperature",
                    MetricValue = 105.00,
                    Unit = "Fahrenheit",
                    ReadingTime = DateTime.UtcNow
                });
            }
            result = await table.ExecuteBatchAsync(batch);
        }

        public async Task DeleteItem(string documentId, object partitionValue)
        {
            TableOperation deleteOperation = TableOperation.Delete(new TableEntity { PartitionKey = (string)partitionValue, RowKey = (string)documentId, ETag="*" });
            await table.ExecuteAsync(deleteOperation);
        }

        public void Dispose()
        {

        }

        public async Task<TableDeviceReading> ReadItem(string documentId, string deviceId)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<TableDeviceReading>(deviceId, documentId);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            TableDeviceReading customer = result.Result as TableDeviceReading;
            return customer;
        }

        public List<TableDeviceReading> ReadItemCollection()
        {
            TableQuery<TableDeviceReading> rangeQuery = new TableQuery<TableDeviceReading>().Where(
                            TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("MetricType", QueryComparisons.Equal, "Temperature"),
                                TableOperators.And,
                                TableQuery.GenerateFilterConditionForDouble("MetricValue", QueryComparisons.GreaterThanOrEqual, 100.00)));

            return table.ExecuteQuery(rangeQuery).ToList<TableDeviceReading>();
        }

        public List<TableDeviceReading> ReadItemCollectionInPartition()
        {
            throw new NotImplementedException();
        }

        public List<TableDeviceReading> ReadItemCollectionParallelQuery()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItem(TableDeviceReading reading)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<TableDeviceReading>(reading.PartitionKey, reading.RowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            TableDeviceReading customer = result.Result as TableDeviceReading;
            customer.MetricValue = 200;
            TableOperation mergeOperation = TableOperation.Merge(customer);
            await table.ExecuteAsync(mergeOperation);
        }
    }
}
