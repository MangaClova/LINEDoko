using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Line.Messaging;
using Line.Messaging.Webhooks;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public class InitContext : ContextBase<FollowEvent>
    {
        protected override async Task ExecuteImplAsync(ContextState contextState, FollowEvent ev)
        {
            await contextState.Client.ReplyMessageAsync(ev.ReplyToken, LineMessages.GetGreetingMessage());
            contextState.Settings.ChatStatus = ChatStatusType.General;
        }
    }
}
