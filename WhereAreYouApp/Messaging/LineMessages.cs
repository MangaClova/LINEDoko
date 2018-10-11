using Line.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using WhereAreYouApp.Models;
using WhereAreYouApp.Utils;

namespace WhereAreYouApp.Messaging
{
    class LineMessages
    {
        public static string DefaultYourName { get; } = "パパ";
        public static string Yes { get; } = "はい";
        public static string No { get; } = "いいえ";

        public static string AskCommentMessage { get; } = @"メッセージも添えますか？
(例「多分5時に帰るよ」)
(ボイスメッセージにも対応しています)";

        public static string[] YesNo { get; } = new[] { Yes, No };

        public static bool IsYesOrNo(string text) => YesNo.Any(x => x == text);

        public static bool IsYes(string text) => Yes == text;

        public static IntentType DetectIntent(string text)
        {
            switch (text)
            {
                case "呼ばれ方設定": return IntentType.ChangeName;
                case "呼び出し履歴": return IntentType.ShowHistory;
                default: return IntentType.None;
            }
        }

        public static IList<ISendMessage> GetHistoriesMessage(IEnumerable<DateTimeOffset> histories)
        {
            if (!histories.Any())
            {
                return new List<ISendMessage>
                {
                    new TextMessage("Clova からの確認履歴はありません。"),
                };
            }

            var historiesTexts = histories.Select(x => DateTimeOffsetUtils.ToJstDateTimeOffset(x))
                .Select(x => $"- {x.ToString("yyyy年MM月dd日 HH時mm分")}")
                .Reverse();
            return new List<ISendMessage>
            {
                new TextMessage($@"以下の時間に Clova から確認がありました。
{string.Join(Environment.NewLine, historiesTexts)}"),
            };
        }

        public static IList<ISendMessage> GetGreetingMessage() => new List<ISendMessage>
        {
            new TextMessage(@"友達追加ありがとうございます😊 

私は、大切な家族の帰りを待つ方向けの Clova スキル「イマドコ」です 💁 

あなたの帰りを待つご家族が、おうちの Clova に 
👧「ねぇ Clova、イマドコにつないで」 
と話しかけたら、

ここで登録しておいた位置情報🗺(「品川駅」など)を送れます。 
その際、メッセージも一緒に送ることができます。"),
        };

        public static IList<ISendMessage> GetAskYourNameMessage() => new List<ISendMessage>
        {
            new TextMessage(@"あなたの呼ばれかたを教えてください 💁

(おうちの Clova が
 🔊「パパは今新宿にいます」
 などと話すのに使います)"),
            new TemplateMessage(
            "呼ばれ方",
            new ButtonsTemplate("呼ばれ方",
                actions: new List<ITemplateAction>
                {
                    new MessageTemplateAction("パパ", "パパ"),
                    new MessageTemplateAction("ママ", "ママ"),
                    new MessageTemplateAction("その他", "その他"),
                })),
        };

        public static IList<ISendMessage> GetManualInputMessage() => new List<ISendMessage>
        {
            new TextMessage(@"あなたの呼ばれ方を入力してください。
(画面左下のキーボードアイコンからどうぞ)"),
        };

        public static string GetFinishGreetingMessage(string yourName) => $@"「{yourName}」に設定しました 💁

それでは、早速、今どちらにいるか教えてください 🗺📍

位置情報の送信は下↓のリッチメニューからどうぞ";

        public static string GetReplyMessageForUnknownMessageType() => $@"すいません。わかりませんでした。";

        public static IList<ISendMessage> GetConfirmMessage(string confirmMessage) => new List<ISendMessage>
        {
            new TemplateMessage(confirmMessage,
                new ButtonsTemplate(confirmMessage,
                actions: new List<ITemplateAction>
                {
                    new MessageTemplateAction(Yes, Yes),
                    new MessageTemplateAction(No, No),
                }))
        };

        public static IList<ISendMessage> GetInputCommentMessage() => new List<ISendMessage>
        {
            new TextMessage(@"メッセージを入力してください 💁
(画面左下のキーボードアイコンからどうぞ)"),
        };

        public static IList<ISendMessage> GetFinishInputMessage(MessagingChatSettings settings, LocationLog locationLog)
        {
            string createBodyMessage()
            {
                if (string.IsNullOrEmpty(locationLog.Comment) && string.IsNullOrEmpty(locationLog.AudioCommentUrl))
                {
                    return $@"「{settings.YourName}は今{ (string.IsNullOrEmpty(locationLog.Name) ? locationLog.Address : locationLog.Name)}にいます。」";
                }

                return $@"🔊「{settings.YourName}は今{ (string.IsNullOrEmpty(locationLog.Name) ? locationLog.Address : locationLog.Name)}にいます。
また、{settings.YourName}からメッセージをもらっています。『{(string.IsNullOrEmpty(locationLog.Comment) ? "(送った音声メッセージが流れます)" : locationLog.Comment)}』」";
            }
            var message = $@"登録が完了しました。
Clova に話しかけたら以下のように答えます。

{createBodyMessage()}

情報の更新は下のリッチメニューからどうぞ 💁";
            return new List<ISendMessage>
            {
                new TextMessage(message),
            };
        }
    }
}
