using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbCRUD
{
    public class SQLDeviceReading : ICommonDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        [JsonProperty("readingTime")]
        public DateTime ReadingTime { get; set; }

        [JsonProperty("metricType")]
        public string MetricType { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("metricValue")]
        public double MetricValue { get; set; }
    }
}
