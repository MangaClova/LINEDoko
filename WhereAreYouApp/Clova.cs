
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
using CEK.CSharp;
using System.Linq;
using WhereAreYouApp.Models;
using CEK.CSharp.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Line.Messaging;
using WhereAreYouApp.Messaging;
using System.Collections.Generic;

namespace WhereAreYouApp
{
    public static class ClovaFunctions
    {
        private static string WhereAreYouIntent { get; } = "WhereAreYou";

        [FunctionName("Clova")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, 
            [Table("LocationLogs")]CloudTable locationLogs,
            ExecutionContext context,
            ILogger log)
        {
            var config = AppConfiguration.GetConfiguration(context);
            var client = new ClovaClient();
            var request = await client.GetRequest(req.Headers["SignatureCEK"], req.Body);
            switch (request.Request.Type)
            {
                case RequestType.LaunchRequest:
                    return await ExecuteLaunchRequestAsync(request, locationLogs, config);
            }

            return new BadRequestResult();
        }

        private static async Task<IActionResult> ExecuteLaunchRequestAsync(CEKRequest request, CloudTable locationLogs, AppConfiguration config)
        {
            var response = new CEKResponse();
            response.AddText("こんにちは。私の持ち主のいる場所を確認するね。");

            var tableResult = await locationLogs.ExecuteAsync(TableOperation.Retrieve<LocationLog>(nameof(LocationLog), request.Session.User.UserId));
            if (tableResult == null || tableResult.HttpStatusCode != 200)
            {
                response.AddText($"ごめんなさい。場所がわかりませんでした。持ち主にLINEで聞いてみるから、また後で聞いてね。");
                await AskCurrentLocationAsync(request, config);
                return new OkObjectResult(response);
            }

            var locationLog = (LocationLog)tableResult.Result;
            var isOldLog = locationLog.Timestamp <= DateTimeOffset.UtcNow - TimeSpan.FromHours(2);
            if (isOldLog)
            {
                await AskCurrentLocationAsync(request, config);
            }
            response.AddText($"{locationLog.Name ?? locationLog.Address}にいます。{(isOldLog ? "でも少し前の場所みたいだね。もう一度聞いてみるから、また後で聞いてね。。" : "")}");
            return new OkObjectResult(response);
        }

        private static async Task AskCurrentLocationAsync(CEKRequest request, AppConfiguration config)
        {
            await LineMessagingClientFactory.GetLineMessagingClient(config.MessagingApi.AccessToken).PushMessageAsync(
                request.Session.User.UserId, new List<ISendMessage>
                {
                            new TextMessage("Clovaから場所が尋ねられたよ。よかったら場所を教えてね。", new QuickReply(
                                new List<QuickReplyButtonObject>
                                {
                                    new QuickReplyButtonObject(new LocationTemplateAction("現在地を送る")),
                                })),
                });
        }
    }
}
