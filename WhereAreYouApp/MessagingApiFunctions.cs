
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Line.Messaging.Liff;
using Line.Messaging;
using System.Net.Http;
using Line.Messaging.Webhooks;
using WhereAreYouApp.Messaging;
using WhereAreYouApp.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace WhereAreYouApp
{
    public static class MessagingApiFunctions
    {
        [FunctionName("MessagingAPI")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, 
            ExecutionContext context,
            [Table("LocationLogs")] CloudTable locationLogs,
            IBinder binder,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var config = AppConfiguration.GetConfiguration(context);
            var lineEvents = await req.GetWebhookEventsAsync(config.MessagingApi.AppSecret);
            var client = LineMessagingClientFactory.GetLineMessagingClient(config.MessagingApi.AccessToken);
            var app = new LineApp(client, locationLogs, binder);
            await app.RunAsync(lineEvents);
            return req.CreateResponse();
        }
    }
}
