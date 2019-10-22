// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal class CalendarRowAccessibleObject : CalendarGridChildAccessibleObject
        {
            int _rowIndex;

            public CalendarRowAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, CalendarBodyAccessibleObject parentAccessibleObject, int rowIndex)
                : base(calendarAccessibleObject, calendarIndex, CalendarChildType.CalendarRow, parentAccessibleObject, rowIndex)
            {
                _rowIndex = rowIndex;
            }

            protected override RECT CalculateBoundingRectangle()
            {
                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, Interop.MonthCalendar.Part.MCGIP_CALENDARROW, _rowIndex, -1, out RECT calendarPartRectangle);
                return calendarPartRectangle;
            }

            internal override int GetChildId() => _rowIndex + 1;

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.NextSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, _parentAccessibleObject, _rowIndex + 1),
                    UnsafeNativeMethods.NavigateDirection.PreviousSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, _parentAccessibleObject, _rowIndex - 1),
                    UnsafeNativeMethods.NavigateDirection.FirstChild =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, this, 0),
                    UnsafeNativeMethods.NavigateDirection.LastChild =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, this, _calendarAccessibleObject.ColumnCount - 1),
                    _ => base.FragmentNavigate(direction)
                };

            public int RowIndex => _rowIndex;

            internal override int[] RuntimeId
            {
                get
                {
                    int[] runtimeId = new int[4];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = _calendarAccessibleObject.Owner.Handle.ToInt32();
                    runtimeId[2] = Parent.GetChildId();
                    runtimeId[3] = GetChildId();
                    return runtimeId;
                }
            }

            // Note: it is not clear whether we need the row names as these are meaningful only for weeks.
            // public override string Name => _rowIndex >= 0 ? string.Format("Week {0}", _rowIndex) : string.Empty;
        }
    }
}
