using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WhereAreYouApp
{
    public class AppConfiguration
    {
        private static readonly object _sync = new object();
        private static AppConfiguration Instance { get; set; }
        public static AppConfiguration GetConfiguration(ExecutionContext context)
        {
            if (Instance == null)
            {
                lock(_sync)
                {
                    if (Instance == null)
                    {
                        var c = new ConfigurationBuilder()
                            .AddEnvironmentVariables()
                            .AddJsonFile(Path.Combine(context.FunctionAppDirectory, "local.settings.json"), true)
                            .Build();
                        Instance = new AppConfiguration();
                        c.Bind(Instance);
                    }
                }
            }

            return Instance;
        }

        public MessagingApi MessagingApi { get; set; }
    }

    public class MessagingApi
    {
        public string AppSecret { get; set; }
        public string AccessToken { get; set; }
    }

    public class Clova
    {
        public string ExtensionId { get; set; }
    }
}
