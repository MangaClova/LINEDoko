using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhereAreYouApp.Messaging;

namespace WhereAreYouApp.Models
{
    public class MessagingChatSettings : TableEntity
    {
        public string YourName { get; set; } = LineMessages.DefaultYourName;
        public string ChatStatus { get; set; } = ChatStatusType.Init;
        public string HistoryJson { get; set; }

        public MessagingChatSettings()
        {
            PartitionKey = nameof(MessagingChatSettings);
        }

        public static async Task<MessagingChatSettings> GetSettingsByUserIdAsync(CloudTable cloudTable, string userId)
        {
            var r = await cloudTable.ExecuteAsync(TableOperation.Retrieve<MessagingChatSettings>(nameof(MessagingChatSettings), userId));
            if (r.HttpStatusCode != 200)
            {
                return null;
            }

            return (MessagingChatSettings)r.Result;
        }

    }
}
