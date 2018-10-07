using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Line.Messaging;
using Line.Messaging.Webhooks;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public class InitContext : ContextBase<ReplyableEvent>
    {
        protected override async Task ExecuteImplAsync(ContextState contextState, ReplyableEvent ev)
        {
            await contextState.Client.ReplyMessageAsync(ev.ReplyToken, LineMessages.GetGreetingMessage());
            contextState.Settings.ChatStatus = ChatStatusType.General;
        }
    }
}
