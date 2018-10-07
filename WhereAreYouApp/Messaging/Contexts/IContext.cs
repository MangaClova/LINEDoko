using Line.Messaging;
using Line.Messaging.Webhooks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public interface IContext
    {
        Task ExecuteAsync(ContextState contextState);
    }
}
