using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Line.Messaging;
using Line.Messaging.Webhooks;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public abstract class ContextBase<TEvent> : IContext
        where TEvent : WebhookEvent
    {
        public Task ExecuteAsync(ContextState contextState)
        {
            if (contextState.WebhookEvent is TEvent t)
            {
                return ExecuteImplAsync(contextState, contextState.WebhookEvent as TEvent);
            }

            throw new InvalidOperationException($"Expected event type is {typeof(TEvent).FullName}, but {contextState.WebhookEvent.GetType().FullName}");
        }

        protected abstract Task ExecuteImplAsync(ContextState contextState, TEvent ev);
    }
}
