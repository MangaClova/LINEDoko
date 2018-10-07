using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhereAreYouApp.Models
{
    public class MessagingSessionData : TableEntity
    {
        public string StatefulContext { get; set; }
        public MessagingSessionData()
        {
            PartitionKey = nameof(MessagingSessionData);
        }
    }
}
