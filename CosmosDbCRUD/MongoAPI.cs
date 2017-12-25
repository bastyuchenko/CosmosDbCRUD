using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;
using System.Security.Authentication;

namespace CosmosDbCRUD
{
    public class MongoAPI : ICommonApi
    {
        private bool disposed = false;
        // To do: update the connection string with the DNS name
        // or IP address of your server.
        //For example, "mongodb://testlinux.cloudapp.net
        private string connectionString = "mongodb://6df6b9fc-0ee0-4-231-b9ee:HD08kALPSugIrqfPy6C5qNWrL6gtVyWs3N4xnfyF5Tg8XX0O0uOBvwkR8TZWXJphKks4b1sYrXqyYha3HodW7g==@6df6b9fc-0ee0-4-231-b9ee.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
        private string userName = "6df6b9fc-0ee0-4-231-b9ee";
        private string host = "6df6b9fc-0ee0-4-231-b9ee.documents.azure.com";
        private string password = "HD08kALPSugIrqfPy6C5qNWrL6gtVyWs3N4xnfyF5Tg8XX0O0uOBvwkR8TZWXJphKks4b1sYrXqyYha3HodW7g==";
        // This sample uses a database named "Tasks" and a
        //collection named "TasksList". The database and collection
        //will be automatically created if they don't already exist.
        private string dbName = "Tasks";
        private string collectionName = "TasksList";
        private const string PartitionValue1 = "XMS-0001";
        private const string PartitionValue2 = "XMS-0002";
        // Default constructor.
        public MongoAPI()
        {
        }
        // Gets all Task items from the MongoDB server.
        public List<ICommonDocument> ReadItemCollection()
        {
            try
            {
                var collection = GetTasksCollection();
                return collection.Find(new BsonDocument()).ToList().Select<MONGODeviceReading,ICommonDocument>(x=>(ICommonDocument)x).ToList();
            }
            catch (MongoConnectionException)
            {
                return new List<ICommonDocument>();
            }
        }
        // Creates a Task and inserts it into the collection in MongoDB.
        public async Task CreateItems()
        {
            var collection = GetTasksCollectionForEdit();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    await collection.InsertOneAsync(new MONGODeviceReading
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
                    await collection.InsertOneAsync(new MONGODeviceReading
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
            catch (MongoCommandException ex)
            {
                string msg = ex.Message;
            }
        }

        private IMongoCollection<MONGODeviceReading> GetTasksCollection()
        {
            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(host, 10255);
            settings.UseSsl = true;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
            MongoIdentity identity = new MongoInternalIdentity(dbName, userName);
            MongoIdentityEvidence evidence = new PasswordEvidence(password);
            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);
            MongoClient client = new MongoClient(settings);
            var database = client.GetDatabase(dbName);
            var todoTaskCollection = database.GetCollection<MONGODeviceReading>(collectionName);
            return todoTaskCollection;
        }

        private IMongoCollection<MONGODeviceReading> GetTasksCollectionForEdit()
        {
            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(host, 10255);
            settings.UseSsl = true;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
            MongoIdentity identity = new MongoInternalIdentity(dbName, userName);
            MongoIdentityEvidence evidence = new PasswordEvidence(password);
            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);
            MongoClient client = new MongoClient(settings);
            var database = client.GetDatabase(dbName);
            var todoTaskCollection = database.GetCollection<MONGODeviceReading>(collectionName);
            return todoTaskCollection;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
            }
            this.disposed = true;
        }

        public Task CreateCollection()
        {
            throw new NotImplementedException();
        }

        public Task CreateDatabase()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteItem(string documentId, object partitionValue)
        {
            var collection = GetTasksCollectionForEdit();
            await collection.DeleteOneAsync(Builders<MONGODeviceReading>.Filter.Eq("_id", documentId));
        }

        public Task<ICommonDocument> ReadItem(string documentId)
        {
            throw new NotImplementedException();
        }

        public List<ICommonDocument> ReadItemCollectionInPartition()
        {
            throw new NotImplementedException();
        }

        public List<ICommonDocument> ReadItemCollectionParallelQuery()
        {
            throw new NotImplementedException();
        }

        public Task UpdateItem(ICommonDocument reading, string documentId)
        {
            throw new NotImplementedException();
        }
    }
}