using System;
using System.Collections.Generic;
using System.Text;

namespace WhereAreYouApp.Utils
{
    public static class DateTimeOffsetUtils
    {
        // TODO: 仮実装
        public static DateTimeOffset ToJstDateTimeOffset(DateTimeOffset dateTimeOffset) => dateTimeOffset.ToOffset(TimeSpan.FromHours(9));
        public static bool IsToday(DateTimeOffset dateTimeOffset) => ToJstDateTimeOffset(DateTimeOffset.UtcNow).Day == ToJstDateTimeOffset(dateTimeOffset).Day;
        public static bool IsBefore(DateTimeOffset target, TimeSpan span) =>
            target <= DateTimeOffset.UtcNow - span;
    }
}
