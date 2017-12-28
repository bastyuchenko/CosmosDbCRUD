using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbCRUD
{
    public class SQLAPI : ICommonApi
    {
        private const string EndpointUrl = "https://micro-cosm.documents.azure.com:443/";
        private const string PrimaryKey = "X8dqnF4znVCS4XwWt5wkSLIXKb64lJmGf646h6Df5eOMVfT7kUfZGo7d0hQk7gYsmK4cZzvFKIVMIMF99MT57Q==";
        private DocumentClient client;
        private const string DatabaseName = "db";
        private const string DocumentCollectionName = "coll";
        private const string PartitionValue1 = "XMS-0001";
        private const string PartitionValue2 = "XMS-0002";


        public SQLAPI()
        {
            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
        }

        public async Task CreateItems()
        {
            for (int i = 0; i < 10; i++)
            {
                await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, DocumentCollectionName),
                    new SQLDeviceReading
                    {
                        Id = $"XMS-001-FE2{i}C",
                        DeviceId = PartitionValue1,
                        MetricType = "Temperature",
                        MetricValue = 105.00,
                        Unit = "Fahrenheit",
                        ReadingTime = DateTime.UtcNow
                    });
            }

            for (int i = 0; i < 8; i++)
            {
                await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, DocumentCollectionName),
                    new SQLDeviceReading
                    {
                        Id = $"XMS-002-FE2{i}C",
                        DeviceId = PartitionValue2,
                        MetricType = "Temperature",
                        MetricValue = 105.00,
                        Unit = "Fahrenheit",
                        ReadingTime = DateTime.UtcNow
                    });
            }
        }

        public async Task CreateCollection()
        {
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = DocumentCollectionName;
            myCollection.PartitionKey.Paths.Add("/deviceId");
            await client.CreateDocumentCollectionIfNotExistsAsync(
            UriFactory.CreateDatabaseUri(DatabaseName),
            myCollection,
            new RequestOptions { OfferThroughput = 2500 });
        }

        public async Task CreateDatabase()
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
        }

        public async Task<ICommonDocument> ReadItem(string documentId)
        {
            Document result = await client.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseName, DocumentCollectionName, (string)documentId),
                new RequestOptions { PartitionKey = new PartitionKey(PartitionValue1) });
            return (SQLDeviceReading)(dynamic)result;
        }

        public async Task UpdateItem(ICommonDocument reading)
        {
            // Update the document. Partition key is not required, again extracted from the document
            reading.MetricValue = 104;
            reading.ReadingTime = DateTime.UtcNow;
            await client.ReplaceDocumentAsync(
            UriFactory.CreateDocumentUri(DatabaseName, DocumentCollectionName, reading.Id), reading, 
            new RequestOptions { PartitionKey = new PartitionKey(PartitionValue1) });
        }

        public async Task DeleteItem(string documentId, object partitionValue)
        {
            // Delete a document. The partition key is required.
            await client.DeleteDocumentAsync(
            UriFactory.CreateDocumentUri(DatabaseName, DocumentCollectionName, (string)documentId),
            new RequestOptions { PartitionKey = new PartitionKey(partitionValue) });
        }

        public List<ICommonDocument> ReadItemCollectionInPartition()
        {
            IQueryable<ICommonDocument> query = client.CreateDocumentQuery<SQLDeviceReading>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, DocumentCollectionName))
                .Where(m => m.MetricType == "Temperature" && m.DeviceId == PartitionValue1);
            return query.ToList();
        }

        public List<ICommonDocument> ReadItemCollectionParallelQuery()
        {
            // Cross-partition Order By queries
            IQueryable<ICommonDocument> crossPartitionQuery = client.CreateDocumentQuery<SQLDeviceReading>(
            UriFactory.CreateDocumentCollectionUri(DatabaseName, DocumentCollectionName),
            new FeedOptions
            {
                EnableCrossPartitionQuery = true,
                MaxDegreeOfParallelism = 10,
                MaxBufferedItemCount =
            100
            })
            .Where(m => m.MetricType == "Temperature" && m.MetricValue > 100)
            .OrderBy(m => m.MetricValue);
            return crossPartitionQuery.ToList();
        }

        public List<ICommonDocument> ReadItemCollection()
        {
            IQueryable<ICommonDocument> crossPartitionQuery = client.CreateDocumentQuery<SQLDeviceReading>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, DocumentCollectionName),
                new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(m => m.MetricType == "Temperature" && m.MetricValue > 100);
            return crossPartitionQuery.ToList();
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
