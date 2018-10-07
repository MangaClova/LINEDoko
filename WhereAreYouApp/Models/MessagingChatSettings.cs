using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhereAreYouApp.Models
{
    public class MessagingChatSettings : TableEntity
    {
        public string YourName { get; set; }
        public string Comment { get; set; }
        public bool IsCommentSended { get; set; }
        public string ChatStatus { get; set; } = ChatStatusType.Init;

        public MessagingChatSettings()
        {
            PartitionKey = nameof(MessagingChatSettings);
        }
    }
}
