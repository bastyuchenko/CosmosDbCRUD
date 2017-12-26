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
        // This sample uses a database named "Tasks" and a
        //collection named "TasksList". The database and collection
        //will be automatically created if they don't already exist.
        private string dbName = "Tasks";
        private string collectionName = "coll";
        private const string PartitionValue1 = "XMS-0001";
        private const string PartitionValue2 = "XMS-0002";
        private MongoClientSettings settings;
        // Default constructor.
        public MongoAPI()
        {
            settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        }
        // Gets all Task items from the MongoDB server.
        public List<ICommonDocument> ReadItemCollection()
        {
            try
            {
                var collection = GetTasksCollection();
                //return collection.Find(new BsonDocument()).ToList().Select<MONGODeviceReading,ICommonDocument>(x=>(ICommonDocument)x).ToList();
                return collection.Find(Builders<MONGODeviceReading>.Filter.Eq("Unit", "Fahrenheit")).ToList().Select<MONGODeviceReading, ICommonDocument>(x => (ICommonDocument)x).ToList();
            }
            catch (MongoConnectionException)
            {
                return new List<ICommonDocument>();
            }
        }
        // Creates a Task and inserts it into the collection in MongoDB.
        public async Task CreateItems()
        {
            var collection = GetTasksCollection();
            try
            {
                for (int i = 0; i < 100; i++)
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

                for (int i = 0; i < 80; i++)
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
            var collection = GetTasksCollection();
            await collection.DeleteOneAsync(Builders<MONGODeviceReading>.Filter.And(
                            Builders<MONGODeviceReading>.Filter.Eq("_id", documentId),
                            Builders<MONGODeviceReading>.Filter.Eq("deviceId", partitionValue)
                            ));
        }

        public async Task<ICommonDocument> ReadItem(string documentId)
        {
            var collection = GetTasksCollection();
            var filterResult = await collection.FindAsync(Builders<MONGODeviceReading>.Filter.Eq("_id", documentId));
            return await filterResult.SingleAsync();
        }

        public List<ICommonDocument> ReadItemCollectionInPartition()
        {
            throw new NotImplementedException();
        }

        public List<ICommonDocument> ReadItemCollectionParallelQuery()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItem(ICommonDocument reading)
        {
            var update = new UpdateDefinitionBuilder<MONGODeviceReading>()
                .Set(s => s.MetricValue, (double)500)
                .Set(s => s.ReadingTime, DateTime.UtcNow);

            var collection = GetTasksCollection();
            await collection.FindOneAndUpdateAsync(Builders<MONGODeviceReading>.Filter.And(
                            Builders<MONGODeviceReading>.Filter.Eq("_id", reading.Id),
                            Builders<MONGODeviceReading>.Filter.Eq("deviceId", reading.DeviceId)
                            ),update);
        }
    }
}