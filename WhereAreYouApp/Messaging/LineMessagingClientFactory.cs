using Line.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhereAreYouApp.Messaging
{
    public class LineMessagingClientFactory
    {
        private static readonly object _sync = new object();
        private static LineMessagingClient Instance { get; set; }
        public static LineMessagingClient GetLineMessagingClient(string appSecret)
        {
            if (Instance == null)
            {
                lock(_sync)
                {
                    if (Instance == null)
                    {
#if SIMULATOR
                        Instance = new LineMessagingClient(appSecret, "http://localhost:8080");
#else
                        Instance = new LineMessagingClient(appSecret);
#endif
                    }
                }
            }

            return Instance;
        }
    }
}
