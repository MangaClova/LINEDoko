using CEK.CSharp;
using CEK.CSharp.Models;
using Line.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhereAreYouApp.Clovas;
using WhereAreYouApp.Messaging;
using WhereAreYouApp.Models;
using WhereAreYouApp.Utils;

namespace WhereAreYouApp
{
    public static class ClovaFunctions
    {
        [FunctionName("Clova")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req,
            [Table("LocationLogs")]CloudTable locationLogs,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("Function started!!");

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
            var taskForSettings = MessagingChatSettings.GetSettingsByUserIdAsync(locationLogs, request.Session.User.UserId);
            var taskForLocationLog = LocationLog.GetLocationLogByUserIdAsync(locationLogs, request.Session.User.UserId);
            await Task.WhenAll(taskForSettings, taskForLocationLog);
            var settings = taskForSettings.Result ?? new MessagingChatSettings { RowKey = request.Session.User.UserId };
            var locationLog = taskForLocationLog.Result;
            try
            {
                if (!settings.IsLineFrend)
                {
                    response.AddText(ClovaMessages.GetAddingLineFrendMessage());
                    return new OkObjectResult(response);
                }

                AddHistory(settings);
                response.AddText(ClovaMessages.GetGuideMessage(settings.YourName));
                if (locationLog == null || !DateTimeOffsetUtils.IsToday(locationLog.Timestamp))
                {
                    // データが無い
                    response.AddText(ClovaMessages.GetNoLogMessage(settings.YourName));
                    await AskCurrentLocationAsync(request, config, settings);
                    return new OkObjectResult(response);
                }

                if (DateTimeOffsetUtils.IsBefore(locationLog.Timestamp, TimeSpan.Parse(config.Cek.IsBeforeThreshold ?? Clova.IsBeforeThresholdDefaultValue)))
                {
                    // 古いデータ
                    response.AddText(ClovaMessages.GetOldLocationMessage(settings.YourName, locationLog));
                    await AskCurrentLocationAsync(request, config, settings);
                    return new OkObjectResult(response);
                }

                // データがある
                response.AddText(ClovaMessages.GetLocationMessage(settings.YourName, locationLog));
                if (!string.IsNullOrEmpty(locationLog.Comment))
                {
                    response.AddText(ClovaMessages.GetCommentMessage(settings.YourName, locationLog));
                    return new OkObjectResult(response);
                }
                else if (!string.IsNullOrEmpty(locationLog.AudioCommentUrl))
                {
                    response.AddText(ClovaMessages.GetVoiceMessagePrefixMessage(settings.YourName));
                    response.AddUrl(locationLog.AudioCommentUrl);
                    return new OkObjectResult(response);
                }
            }
            finally
            {
                await locationLogs.ExecuteAsync(TableOperation.InsertOrReplace(settings));
            }

            return new OkObjectResult(response);
        }

        private static void AddHistory(MessagingChatSettings settings)
        {
            var histories = JsonConvert.DeserializeObject<List<DateTimeOffset>>(settings.HistoryJson ?? "[]");
            histories.Add(DateTimeOffset.UtcNow);
            while (histories.Count > 10)
            {
                histories.RemoveAt(0);
            }
            settings.HistoryJson = JsonConvert.SerializeObject(histories);
        }

        private static async Task AskCurrentLocationAsync(CEKRequest request, AppConfiguration config, MessagingChatSettings settings)
        {
            await LineMessagingClientFactory.GetLineMessagingClient(config.MessagingApi.AccessToken).PushMessageAsync(
                request.Session.User.UserId, new List<ISendMessage>
                {
                    new TextMessage(ClovaMessages.GetAskLocationMessage(settings.YourName)),
                });
        }
    }
}
