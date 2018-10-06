
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
            response.AddText("����ɂ��́B���̎�����̂���ꏊ���m�F����ˁB");

            var tableResult = await locationLogs.ExecuteAsync(TableOperation.Retrieve<LocationLog>(nameof(LocationLog), request.Session.User.UserId));
            if (tableResult == null || tableResult.HttpStatusCode != 200)
            {
                response.AddText($"���߂�Ȃ����B�ꏊ���킩��܂���ł����B�������LINE�ŕ����Ă݂邩��A�܂���ŕ����ĂˁB");
                await AskCurrentLocationAsync(request, config);
                return new OkObjectResult(response);
            }

            var locationLog = (LocationLog)tableResult.Result;
            var isOldLog = locationLog.Timestamp <= DateTimeOffset.UtcNow - TimeSpan.FromHours(2);
            if (isOldLog)
            {
                await AskCurrentLocationAsync(request, config);
            }
            response.AddText($"{locationLog.Name ?? locationLog.Address}�ɂ��܂��B{(isOldLog ? "�ł������O�̏ꏊ�݂������ˁB������x�����Ă݂邩��A�܂���ŕ����ĂˁB�B" : "")}");
            return new OkObjectResult(response);
        }

        private static async Task AskCurrentLocationAsync(CEKRequest request, AppConfiguration config)
        {
            await LineMessagingClientFactory.GetLineMessagingClient(config.MessagingApi.AccessToken).PushMessageAsync(
                request.Session.User.UserId, new List<ISendMessage>
                {
                            new TextMessage("Clova����ꏊ���q�˂�ꂽ��B�悩������ꏊ�������ĂˁB", new QuickReply(
                                new List<QuickReplyButtonObject>
                                {
                                    new QuickReplyButtonObject(new LocationTemplateAction("���ݒn�𑗂�")),
                                })),
                });
        }
    }
}
