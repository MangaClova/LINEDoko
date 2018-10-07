using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public class ContextState
    {
        public MessagingChatSettings Settings { get; set; }
        public MessagingSessionData SessionData { get; set; }
        public LineMessagingClient Client { get; set; }
        public WebhookEvent WebhookEvent { get; set; }
        public CloudTable StateStoreTable { get; set; }
        public IBinder Binder { get; set; }
    }
}
