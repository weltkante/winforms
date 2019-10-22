// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class MonthCalendar
    {
        /// <summary>
        ///  MonthCalendar Control Messages.
        ///  Copied form CommCtrl.h
        /// </summary>
        public static class Messages
        {
            public const int MCM_FIRST = 0x1000;
            public const int MCM_GETCURSEL = (MCM_FIRST + 1);
            public const int MCM_SETMAXSELCOUNT = (MCM_FIRST + 4);
            public const int MCM_GETSELRANGE = (MCM_FIRST + 5);
            public const int MCM_SETSELRANGE = (MCM_FIRST + 6);
            public const int MCM_GETMONTHRANGE = (MCM_FIRST + 7);
            public const int MCM_GETMINREQRECT = (MCM_FIRST + 9);
            public const int MCM_SETCOLOR = (MCM_FIRST + 10);
            public const int MCM_SETTODAY = (MCM_FIRST + 12);
            public const int MCM_GETTODAY = (MCM_FIRST + 13);
            public const int MCM_HITTEST = (MCM_FIRST + 14);
            public const int MCM_SETFIRSTDAYOFWEEK = (MCM_FIRST + 15);
            public const int MCM_GETRANGE = (MCM_FIRST + 17);
            public const int MCM_SETRANGE = (MCM_FIRST + 18);
            public const int MCM_SETMONTHDELTA = (MCM_FIRST + 20);
            public const int MCM_GETMAXTODAYWIDTH = (MCM_FIRST + 21);
            public const int MCM_GETCALENDARGRIDINFO = (MCM_FIRST + 24);
        }
    }
}
