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
            $"こんにちは。{name}は、今、{locationLog.Name ?? locationLog.Address}にいます。";

        public static string GetCommentMessage(string name, LocationLog locationLog)
        {
            if (string.IsNullOrEmpty(locationLog.Comment))
            {
                return "";
            }

            return $"また、{name}から、メッセージをもらっています。　　{locationLog.Comment}";
        }

        public static string GetNoLogMessage(string name) => 
            $"こんにちは。きょうは、まだ、{name}の位置情報の登録が、されていないので、今どこにいるか、{name}に LINE で聞いてみますね。少ししたら、また、聞いてください。";

        public static string GetAskLocationMessage(string name) =>
            $@"おうちの Clova から通知がありました。あなたが今どこにいるのか気にしています。

位置情報の送信は下↓のリッチメニューからどうぞ 💁";

        public static string GetOldLocationMessage(string name, LocationLog locationLog) =>
            $"{name}は、きょう、{DateTimeOffsetUtils.ToJstDateTimeOffset(locationLog.Timestamp).ToString("HH時")}に、{locationLog.Name ?? locationLog.Address}にいました。今どこにいるか、もう一度 LINE で聞いてみますね。少ししたら、また、聞いてください。";

        public static string GetVoiceMessagePrefixMessage(string name) => $"また、{name}からメッセージを貰っています。　　";
    }
}
