using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbCRUD
{
    public interface ICommonDocument
    {
        string Id { get; set; }
        
        string DeviceId { get; set; }

        DateTime ReadingTime { get; set; }

        string MetricType { get; set; }

        string Unit { get; set; }

        double MetricValue { get; set; }
    }
}
