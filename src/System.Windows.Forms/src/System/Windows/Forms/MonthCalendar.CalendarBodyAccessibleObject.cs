// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        /// Represents the calendar body accessible object.
        /// </summary>
        internal class CalendarBodyAccessibleObject : CalendarChildAccessibleObject
        {
            private const int ChildId = 4;

            public CalendarBodyAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex)
                : base(calendarAccessibleObject, calendarIndex, CalendarChildType.CalendarBody)
            {
            }

            protected override RECT CalculateBoundingRectangle()
            {
                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, NativeMethods.MCGIP_CALENDARBODY, 0, 0, out RECT calendarPartRectangle);
                return calendarPartRectangle;
            }

            internal override int GetChildId() => ChildId;

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.NextSibling => new Func<AccessibleObject>(() =>
                    {
                        MonthCalendar owner = (MonthCalendar)_calendarAccessibleObject.Owner;
                        return owner.ShowToday ? _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.TodayLink) : null;
                    })(),
                    UnsafeNativeMethods.NavigateDirection.PreviousSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarHeader),
                    UnsafeNativeMethods.NavigateDirection.FirstChild =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, _calendarAccessibleObject.HasHeaderRow ? -1 : 0),
                    UnsafeNativeMethods.NavigateDirection.LastChild =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, _calendarAccessibleObject.RowCount - 1),
                    _ => base.FragmentNavigate(direction),

                };

            public CalendarChildAccessibleObject GetFromPoint(NativeMethods.MCHITTESTINFO_V6 hitTestInfo)
            {
                switch (hitTestInfo.uHit)
                {
                    case NativeMethods.MCHT_CALENDARDAY:
                    case NativeMethods.MCHT_CALENDARWEEKNUM:
                    case NativeMethods.MCHT_CALENDARDATE:
                        AccessibleObject rowAccessibleObject =
                            _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, hitTestInfo.iRow);
                        return
                            _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, rowAccessibleObject, hitTestInfo.iCol);
                }

                return this;
            }

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_ControlTypePropertyId => NativeMethods.UIA_TableControlTypeId,
                    NativeMethods.UIA_NamePropertyId => "Calendar body",
                    NativeMethods.UIA_IsGridPatternAvailablePropertyId => true,
                    NativeMethods.UIA_IsTablePatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_GridPatternId ||
                    patternId == NativeMethods.UIA_TablePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaders()
            {
                return null;
            }

            internal override UnsafeNativeMethods.RowOrColumnMajor RowOrColumnMajor => _calendarAccessibleObject.RowOrColumnMajor;

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaderItems() => _calendarAccessibleObject.GetRowHeaderItems();

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaderItems() => _calendarAccessibleObject.GetColumnHeaderItems();

            internal override UnsafeNativeMethods.IRawElementProviderSimple GetItem(int row, int column) => _calendarAccessibleObject.GetItem(row, column);

            internal override int RowCount => _calendarAccessibleObject.RowCount;

            internal override int ColumnCount => _calendarAccessibleObject.ColumnCount;
        }
    }
}
