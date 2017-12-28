using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CosmosDbCRUD
{
    class Program
    {
        static void Main(string[] args)
        {
            //SQLApiRun().GetAwaiter().GetResult();
            //MongoApiRun().GetAwaiter().GetResult();
            TableAPIRun().GetAwaiter().GetResult();
        }

        private static async Task SQLApiRun()
        {
            using (ICommonApi sqlApi = new SQLAPI())
            {
                await sqlApi.CreateDatabase();
                await sqlApi.CreateCollection();
                await sqlApi.CreateItems();
                ICommonDocument doc = await sqlApi.ReadItem("XMS-001-FE24C");
                Console.WriteLine(doc.MetricValue);

                var result = sqlApi.ReadItemCollection();

                foreach (var item in result)
                {
                    Console.WriteLine($"Update {item.Id}");
                    await sqlApi.DeleteItem(item.Id, item.DeviceId);
                }

                foreach (var item in result)
                {
                    Console.WriteLine($"Delete {item.Id}");
                    await sqlApi.DeleteItem(item.Id, item.DeviceId);
                }
            }
        }

        private static async Task MongoApiRun()
        {
            ///////////////////////////////
            ////***** to create a collection with partition key you should create sharded collection *****///
            // download MongoDB comunity and install it 
            // run cmd
            // connect to azure cosmos db using the command from "Quick start"->"Mongo DB shell"
            // db.runCommand( { shardCollection: "Tasks.coll", key: { deviceId: "hashed" } } )
            // where db.runCommand( { shardCollection: "<database name>.<collection name>", key: { <partition key-field-column name>: "hashed" } } )
            ////////////////////////////////

            using (ICommonApi sqlApi = new MongoAPI())
            {
                ////await sqlApi.CreateDatabase();
                ////await sqlApi.CreateCollection();
                //await sqlApi.CreateItems();
                

                var result = sqlApi.ReadItemCollection();

                foreach (var item in result)
                {
                    Console.WriteLine($"Update {item.Id}");
                    await sqlApi.UpdateItem(item);
                    ICommonDocument doc = await sqlApi.ReadItem(item.Id);
                    Console.WriteLine(doc.MetricValue);
                }

                foreach (var item in result)
                {
                    Console.WriteLine($"Delete {item.Id}");
                    await sqlApi.DeleteItem(item.Id, item.DeviceId);
                }
            }
        }

        private static async Task TableAPIRun()
        {
            using (TableAPI sqlApi = new TableAPI())
            {
                await sqlApi.CreateCollection();
                await sqlApi.CreateItems();
                var result = sqlApi.ReadItemCollection();

                foreach (var item in result)
                {
                    Console.WriteLine($"Update {item.RowKey}");
                    await sqlApi.UpdateItem(item);
                    var doc = await sqlApi.ReadItem(item.RowKey, item.PartitionKey);
                    Console.WriteLine(doc.MetricValue);
                }

                foreach (var item in result)
                {
                    Console.WriteLine($"Delete {item.RowKey}");
                    await sqlApi.DeleteItem(item.RowKey, item.PartitionKey);
                }
            }
        }

    }
}
