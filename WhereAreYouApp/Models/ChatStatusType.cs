using System;
using System.Collections.Generic;
using System.Text;

namespace WhereAreYouApp.Models
{
    public static class ChatStatusType
    {
        public static string Init { get; } = nameof(Init);
        public static string AskingYourName { get; } = nameof(AskingYourName);
        public static string ManualInputYourName { get; } = nameof(ManualInputYourName);
        public static string General { get; } = nameof(General);
    }
}
