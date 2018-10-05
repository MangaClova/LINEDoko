using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhereAreYouApp.Models
{
    public class LocationLog : TableEntity
    {
        public string Name { get; set; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Longitude { get; set; }
        /// <summary>
        /// 経度
        /// </summary>
        public decimal Latitude { get; set; }

        public string Address { get; set; }

        public LocationLog()
        {
            PartitionKey = nameof(LocationLog);
        }
    }
}
