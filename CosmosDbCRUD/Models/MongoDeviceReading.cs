using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbCRUD
{
    public class MONGODeviceReading : ICommonDocument
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("DeviceId")]
        public string DeviceId { get; set; }

        [BsonElement("ReadingTime")]
        public DateTime ReadingTime { get; set; }

        [BsonElement("MetricType")]
        public string MetricType { get; set; }

        [BsonElement("Unit")]
        public string Unit { get; set; }

        [BsonElement("MetricValue")]
        public double MetricValue { get; set; }
    }
}
