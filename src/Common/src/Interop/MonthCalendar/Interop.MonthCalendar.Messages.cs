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
        public enum Messages
        {
            MCM_FIRST = 0x1000,
            MCM_GETCURSEL = (MCM_FIRST + 1),
            MCM_SETMAXSELCOUNT = (MCM_FIRST + 4),
            MCM_GETSELRANGE = (MCM_FIRST + 5),
            MCM_SETSELRANGE = (MCM_FIRST + 6),
            MCM_GETMONTHRANGE = (MCM_FIRST + 7),
            MCM_GETMINREQRECT = (MCM_FIRST + 9),
            MCM_SETCOLOR = (MCM_FIRST + 10),
            MCM_SETTODAY = (MCM_FIRST + 12),
            MCM_GETTODAY = (MCM_FIRST + 13),
            MCM_HITTEST = (MCM_FIRST + 14),
            MCM_SETFIRSTDAYOFWEEK = (MCM_FIRST + 15),
            MCM_GETRANGE = (MCM_FIRST + 17),
            MCM_SETRANGE = (MCM_FIRST + 18),
            MCM_SETMONTHDELTA = (MCM_FIRST + 20),
            MCM_GETMAXTODAYWIDTH = (MCM_FIRST + 21),
            MCM_GETCALENDARGRIDINFO = (MCM_FIRST + 24)
        }
    }
}
