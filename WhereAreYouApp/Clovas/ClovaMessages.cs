using System;
using System.Collections.Generic;
using System.Text;
using WhereAreYouApp.Models;
using WhereAreYouApp.Utils;

namespace WhereAreYouApp.Clovas
{
    public static class ClovaMessages
    {
        public static string GetLocationMessage(string name, LocationLog locationLog) => 
            $"こんにちは。{name}は今{locationLog.Name ?? locationLog.Address}にいます。";

        public static string GetCommentMessage(string name, LocationLog locationLog)
        {
            if (string.IsNullOrEmpty(locationLog.Comment))
            {
                return "";
            }

            return $"また、{name}からメッセージをもらっています。　　{locationLog.Comment}";
        }

        public static string GetNoLogMessage(string name) => 
            $"こんにちは。今日はまだ{name}の位置情報の登録がされていないので、今どこにいるか{name}に LINE で聞いてみますね。少ししたらまた聞いてください。";

        public static string GetAskLocationMessage(string name) =>
            $@"{name}さんが今どこにいるか気にしています。
位置情報の送信は下のリッチメニューからどうぞ";

        public static string GetOldLocationMessage(string name, LocationLog locationLog) =>
            $"{name}は、今日{DateTimeOffsetUtils.ToJstDateTimeOffset(locationLog.Timestamp).ToString("HH時")}に{locationLog.Name ?? locationLog.Address}にいました。今どこにいるか、もう一度 LINE で聞いてみますね。少ししたら、また聞いてください。";
    }
}
