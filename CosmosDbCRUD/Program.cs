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
            MongoApiRun().GetAwaiter().GetResult();
        }

        private static async Task SQLApiRun()
        {
            using (ICommonApi sqlApi = new PartitionsSQLApi())
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
            using (ICommonApi sqlApi = new MongoAPI())
            {
                ////await sqlApi.CreateDatabase();
                ////await sqlApi.CreateCollection();
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

    }
}
