// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class MonthCalendar
    {
        /// <summary>
        ///  MonthCalendar Control HitTest values.
        ///  Copied form CommCtrl.h
        /// </summary>
        public static class HitTest
        {
            public const int MCHT_TITLE = 0x00010000;
            public const int MCHT_CALENDAR = 0x00020000;
            public const int MCHT_TODAYLINK = 0x00030000;

            public const int MCHT_TITLEBK = (MCHT_TITLE);
            public const int MCHT_TITLEMONTH = (MCHT_TITLE | 0x0001);
            public const int MCHT_TITLEYEAR = (MCHT_TITLE | 0x0002);
            public const int MCHT_TITLEBTNNEXT = (MCHT_TITLE | 0x01000000 | 0x0003);
            public const int MCHT_TITLEBTNPREV = (MCHT_TITLE | 0x02000000 | 0x0003);

            public const int MCHT_CALENDARBK = (MCHT_CALENDAR);
            public const int MCHT_CALENDARDATE = (MCHT_CALENDAR | 0x0001);
            public const int MCHT_CALENDARDATENEXT = ((MCHT_CALENDAR | 0x0001) | 0x01000000);
            public const int MCHT_CALENDARDATEPREV = ((MCHT_CALENDAR | 0x0001) | 0x02000000);
            public const int MCHT_CALENDARDAY = (MCHT_CALENDAR | 0x0002);
            public const int MCHT_CALENDARWEEKNUM = (MCHT_CALENDAR | 0x0003);
        }
    }
}
