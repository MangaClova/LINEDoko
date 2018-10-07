using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public string Comment { get; set; }
        public string AudioCommentUrl { get; set; }

        public LocationLog()
        {
            PartitionKey = nameof(LocationLog);
        }

        public static async Task<LocationLog> GetLocationLogByUserIdAsync(CloudTable cloudTable, string userId)
        {
            var r = await cloudTable.ExecuteAsync(TableOperation.Retrieve<LocationLog>(nameof(LocationLog), userId));
            if (r.HttpStatusCode != 200)
            {
                return null;
            }

            return (LocationLog)r.Result;
        }

        public static async Task InsertOrReplaceAsync(CloudTable cloudTable, LocationLog locationLog)
        {
            await cloudTable.ExecuteAsync(TableOperation.InsertOrReplace(locationLog));
        }
    }
}
