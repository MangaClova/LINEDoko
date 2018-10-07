using System;
using System.Collections.Generic;
using System.Text;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public static class ContextFactory
    {
        private static Dictionary<string, Func<IContext>> FactoryMap { get; } = new Dictionary<string, Func<IContext>>
        {
            [ChatStatusType.Init] = () => new InitContext(),
            [ChatStatusType.AskingYourName] = () => new AskingYourNameContext(),
            [ChatStatusType.ManualInputYourName] = () => new ManualInputYourNameContext(),
            [ChatStatusType.General] = () => new GeneralContext(),
        };

        public static IContext Create(string status) => FactoryMap[status]();
    }
}
