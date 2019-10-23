// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class MonthCalendar
    {
        /// <summary>
        ///  MonthCalendar set color constants.
        ///  Copied form CommCtrl.h
        /// </summary>
        public enum Part : uint
        {
            MCGIP_CALENDARCONTROL = 0,
            MCGIP_NEXT = 1,
            MCGIP_PREV = 2,
            MCGIP_FOOTER = 3,
            MCGIP_CALENDAR = 4,
            MCGIP_CALENDARHEADER = 5,
            MCGIP_CALENDARBODY = 6,
            MCGIP_CALENDARROW = 7,
            MCGIP_CALENDARCELL = 8
        }
    }
}
