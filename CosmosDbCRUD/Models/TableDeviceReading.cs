using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CosmosDbCRUD
{
    public class TableDeviceReading : TableEntity
    {
        public DateTime ReadingTime { get; set; }

        public string MetricType { get; set; }

        public string Unit { get; set; }

        public double MetricValue { get; set; }
    }
}
