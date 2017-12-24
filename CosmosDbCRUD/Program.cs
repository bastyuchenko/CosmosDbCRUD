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
            SQLApiRun().GetAwaiter().GetResult();
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

                var result = sqlApi.ReadItemCollectionAcrossAllPartition();
                foreach (var item in result)
                {
                    await sqlApi.DeleteItem(item.Id, item.DeviceId);
                }
            }
        }

    }
}
