using System;
using System.Collections.Generic;
using System.Text;

namespace WhereAreYouApp.Utils
{
    public static class DateTimeOffsetUtils
    {
        // TODO: 実装必要
        public static DateTimeOffset ToJstDateTimeOffset(DateTimeOffset dateTimeOffset) => dateTimeOffset;
        // JST 対応必須
        public static bool IsToday(DateTimeOffset dateTimeOffset) => DateTimeOffset.UtcNow.Day == dateTimeOffset.Day;
        public static bool IsBefore(DateTimeOffset target, TimeSpan span) =>
            target <= DateTimeOffset.UtcNow - span;
    }
}
