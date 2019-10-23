// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        [ComVisible(true)]
        internal class MonthCalendarAccessibleObject : ControlAccessibleObject
        {
            internal const int MAX_DAYS = 7;
            internal const int MAX_WEEKS = 6;

            private readonly MonthCalendar _owner;
            private int _calendarIndex = 0;
            private AccessibleObject _focused;

            public MonthCalendarAccessibleObject(Control owner)
                : base(owner)
            {
                _owner = owner as MonthCalendar;
            }

            public int ControlType
            {
                get
                {
                    if (string.IsNullOrEmpty(base.Name))
                    {
                        return NativeMethods.UIA_CalendarControlTypeId;
                    }

                    return NativeMethods.UIA_TableControlTypeId;
                }
            }

            public bool HasHeaderRow
            {
                get
                {
                    bool result = GetCalendarGridInfoText(Interop.MonthCalendar.Part.MCGIP_CALENDARCELL, _calendarIndex, -1, 0, out string text);
                    if (!result || string.IsNullOrEmpty(text))
                    {
                        return false;
                    }

                    return true;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    if (_owner != null)
                    {
                        AccessibleRole role = _owner.AccessibleRole;
                        if (role != AccessibleRole.Default)
                        {
                            return role;
                        }
                    }
                    return AccessibleRole.Table;
                }
            }

            public override string Help
            {
                get
                {
                    var help = base.Help;
                    if (help != null)
                    {
                        return help;
                    }
                    else
                    {
                        if (_owner != null)
                        {
                            return _owner.GetType().Name + "(" + _owner.GetType().BaseType.Name + ")";
                        }
                    }
                    return string.Empty;
                }
            }

            public override string Name
            {
                get
                {
                    string name = base.Name;
                    if (name != null)
                    {
                        return name;
                    }

                    name = string.Empty;
                    if (_owner == null)
                    {
                        return name;
                    }

                    if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH)
                    {
                        if (DateTime.Equals(_owner.SelectionStart.Date, _owner.SelectionEnd.Date))
                        {
                            return string.Format(SR.MonthCalendarSingleDateSelected, _owner.SelectionStart.ToLongDateString());
                        }
                        else
                        {
                            return string.Format(SR.MonthCalendarRangeSelected, _owner.SelectionStart.ToLongDateString(), _owner.SelectionEnd.ToLongDateString());
                        }
                    }
                    else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_YEAR)
                    {
                        if (DateTime.Equals(_owner.SelectionStart.Month, _owner.SelectionEnd.Month))
                        {
                            return string.Format(SR.MonthCalendarSingleDateSelected, _owner.SelectionStart.ToString("y"));
                        }
                        else
                        {
                            return string.Format(SR.MonthCalendarRangeSelected, _owner.SelectionStart.ToString("y"), _owner.SelectionEnd.ToString("y"));
                        }
                    }
                    else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_DECADE)
                    {
                        if (DateTime.Equals(_owner.SelectionStart.Year, _owner.SelectionEnd.Year))
                        {
                            return string.Format(SR.MonthCalendarSingleYearSelected, _owner.SelectionStart.ToString("yyyy"));
                        }
                        else
                        {
                            return string.Format(SR.MonthCalendarYearRangeSelected, _owner.SelectionStart.ToString("yyyy"), _owner.SelectionEnd.ToString("yyyy"));
                        }
                    }
                    else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_CENTURY)
                    {
                        return string.Format(SR.MonthCalendarSingleDecadeSelected, _owner.SelectionStart.ToString("yyyy"));
                    }

                    return name;
                }
            }

            public override string Value
            {
                get
                {
                    var value = string.Empty;
                    if (_owner == null)
                    {
                        return value;
                    }

                    try
                    {
                        if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH)
                        {
                            if (System.DateTime.Equals(_owner.SelectionStart.Date, _owner.SelectionEnd.Date))
                            {
                                value = _owner.SelectionStart.ToLongDateString();
                            }
                            else
                            {
                                value = string.Format("{0} - {1}", _owner.SelectionStart.ToLongDateString(), _owner.SelectionEnd.ToLongDateString());
                            }
                        }
                        else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_YEAR)
                        {
                            if (System.DateTime.Equals(_owner.SelectionStart.Month, _owner.SelectionEnd.Month))
                            {
                                value = _owner.SelectionStart.ToString("y");
                            }
                            else
                            {
                                value = string.Format("{0} - {1}", _owner.SelectionStart.ToString("y"), _owner.SelectionEnd.ToString("y"));
                            }
                        }
                        else
                        {
                            value = string.Format("{0} - {1}", _owner.SelectionRange.Start.ToString(), _owner.SelectionRange.End.ToString());
                        }
                    }
                    catch
                    {
                        value = base.Value;
                    }

                    return value;
                }
                set
                {
                    base.Value = value;
                }
            }

            internal override int ColumnCount
            {
                get
                {
                    GetCalendarGridInfo(
                        Interop.MonthCalendar.GridInfoFlags.MCGIF_RECT,
                        Interop.MonthCalendar.Part.MCGIP_CALENDARBODY,
                        _calendarIndex,
                        -1,
                        -1,
                        out RECT calendarBodyRectangle,
                        out NativeMethods.SYSTEMTIME endDate,
                        out NativeMethods.SYSTEMTIME startDate);

                    int columnCount = 0;
                    bool success = true;
                    while (success)
                    {
                        success = GetCalendarGridInfo(
                            Interop.MonthCalendar.GridInfoFlags.MCGIF_RECT,
                            Interop.MonthCalendar.Part.MCGIP_CALENDARCELL,
                            _calendarIndex,
                            0,
                            columnCount,
                            out RECT calendarPartRectangle,
                            out endDate,
                            out startDate);

                        // Out of the body, so this is out of the grid column.
                        if (calendarPartRectangle.right > calendarBodyRectangle.right)
                        {
                            break;
                        }

                        columnCount++;
                    }

                    return columnCount;
                }
            }

            internal override int RowCount
            {
                get
                {
                    GetCalendarGridInfo(
                        Interop.MonthCalendar.GridInfoFlags.MCGIF_RECT,
                        Interop.MonthCalendar.Part.MCGIP_CALENDARBODY,
                        _calendarIndex,
                        -1,
                        -1,
                        out RECT calendarBodyRectangle,
                        out NativeMethods.SYSTEMTIME endDate,
                        out NativeMethods.SYSTEMTIME startDate);

                    int rowCount = 0;
                    bool success = true;
                    while (success)
                    {
                        success = GetCalendarGridInfo(
                            Interop.MonthCalendar.GridInfoFlags.MCGIF_RECT,
                            Interop.MonthCalendar.Part.MCGIP_CALENDARCELL,
                            _calendarIndex,
                            rowCount,
                            0,
                            out RECT calendarPartRectangle,
                            out endDate,
                            out startDate);

                        // Out of the body, so this is out of the grid row.
                        if (calendarPartRectangle.bottom > calendarBodyRectangle.bottom)
                        {
                            break;
                        }

                        rowCount++;
                    }

                    return rowCount;
                }
            }

            internal override UnsafeNativeMethods.RowOrColumnMajor RowOrColumnMajor => UnsafeNativeMethods.RowOrColumnMajor.RowOrColumnMajor_RowMajor;

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaderItems() => null;

            internal override UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                int innerX = (int)x;
                int innerY = (int)y;

                NativeMethods.MCHITTESTINFO_V6 hitTestInfo = GetHitTestInfo(innerX, innerY);
                switch ((Interop.MonthCalendar.HitTest)hitTestInfo.uHit)
                {
                    case Interop.MonthCalendar.HitTest.MCHT_TITLEBTNPREV:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.PreviousButton);

                    case Interop.MonthCalendar.HitTest.MCHT_TITLEBTNNEXT:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.NextButton);

                    case Interop.MonthCalendar.HitTest.MCHT_TITLE:
                    case Interop.MonthCalendar.HitTest.MCHT_TITLEMONTH:
                    case Interop.MonthCalendar.HitTest.MCHT_TITLEYEAR:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarHeader);

                    case Interop.MonthCalendar.HitTest.MCHT_CALENDARDAY:
                    case Interop.MonthCalendar.HitTest.MCHT_CALENDARWEEKNUM:
                    case Interop.MonthCalendar.HitTest.MCHT_CALENDARDATE:
                        // Get calendar body's child.
                        CalendarBodyAccessibleObject calendarBodyAccessibleObject = (CalendarBodyAccessibleObject)GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);
                        return calendarBodyAccessibleObject.GetFromPoint(hitTestInfo);

                    case Interop.MonthCalendar.HitTest.MCHT_TODAYLINK:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.TodayLink);
                }

                return base.ElementProviderFromPoint(x, y);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.PreviousButton);
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
                        return _owner.ShowTodayCircle
                            ? GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.TodayLink)
                            : GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);
                }

                return base.FragmentNavigate(direction);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment GetFocus() => _focused;

            public override AccessibleObject GetFocused() => _focused;

            public NativeMethods.MCHITTESTINFO_V6 GetHitTestInfo(int xScreen, int yScreen)
            {
                NativeMethods.MCHITTESTINFO_V6 hitTestInfo = new NativeMethods.MCHITTESTINFO_V6();
                hitTestInfo.cbSize = (int)Marshal.SizeOf(hitTestInfo);
                hitTestInfo.pt = new NativeMethods.Win32Point();
                hitTestInfo.st = new NativeMethods.SYSTEMTIME();

                // NativeMethods.GetCursorPos(out Point pt);
                Point point = new Point(xScreen, yScreen);
                NativeMethods.MapWindowPoints(IntPtr.Zero, Handle, ref point, 1);
                hitTestInfo.pt.x = point.X;
                hitTestInfo.pt.y = point.Y;

                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)Interop.MonthCalendar.Messages.MCM_HITTEST, 0, ref hitTestInfo);

                return hitTestInfo;
            }

            public CalendarChildAccessibleObject GetCalendarChildAccessibleObject(int calendarIndex, CalendarChildType calendarChildType, AccessibleObject parentAccessibleObject = null, int index = -1) =>
                 calendarChildType switch
                 {
                     CalendarChildType.PreviousButton => new CalendarPreviousButtonAccessibleObject(this, _calendarIndex),
                     CalendarChildType.NextButton => new CalendarNextButtonAccessibleObject(this, _calendarIndex),
                     CalendarChildType.CalendarHeader => new CalendarHeaderAccessibleObject(this, _calendarIndex),
                     CalendarChildType.CalendarBody => new CalendarBodyAccessibleObject(this, _calendarIndex),
                     CalendarChildType.CalendarRow => GetCalendarRow(calendarIndex, parentAccessibleObject, index),
                     CalendarChildType.CalendarCell => GetCalendarCell(calendarIndex, parentAccessibleObject, index),
                     CalendarChildType.TodayLink => new CalendarTodayLinkAccessibleObject(this, (int)CalendarChildType.TodayLink, calendarChildType),
                     _ => null
                 };

            public string GetCalendarChildName(int calendarIndex, CalendarChildType calendarChildType, AccessibleObject parentAccessibleObject = null, int index = -1)
            {
                switch (calendarChildType)
                {
                    case CalendarChildType.CalendarHeader:
                        GetCalendarGridInfoText(Interop.MonthCalendar.Part.MCGIP_CALENDARHEADER, calendarIndex, 0, 0, out string text);
                        return text;
                    case CalendarChildType.TodayLink:
                        return string.Format(SR.MonthCalendarTodayButtonAccessibleName, _owner.TodayDate.ToShortDateString());
                };

                return string.Empty;
            }

            private CalendarCellAccessibleObject GetCalendarCell(int calendarIndex, AccessibleObject parentAccessibleObject, int columnIndex)
            {
                if (columnIndex < 0 ||
                    columnIndex >= MAX_DAYS ||
                    columnIndex >= ColumnCount)
                {
                    return null;
                }

                CalendarRowAccessibleObject parentRowAccessibleObject = (CalendarRowAccessibleObject)parentAccessibleObject;
                int rowIndex = parentRowAccessibleObject.RowIndex;
                bool getNameResult = GetCalendarGridInfoText(Interop.MonthCalendar.Part.MCGIP_CALENDARCELL, calendarIndex, rowIndex, columnIndex, out string text);
                bool getDateResult = GetCalendarGridInfo(Interop.MonthCalendar.GridInfoFlags.MCGIF_DATE, Interop.MonthCalendar.Part.MCGIP_CALENDARCELL,
                    calendarIndex,
                    rowIndex,
                    columnIndex,
                    out RECT rectangle,
                    out NativeMethods.SYSTEMTIME systemEndDate,
                    out NativeMethods.SYSTEMTIME systemStartDate);

                DateTime endDate = DateTimePicker.SysTimeToDateTime(systemEndDate).Date;
                DateTime startDate = DateTimePicker.SysTimeToDateTime(systemStartDate).Date;

                if (getNameResult && !string.IsNullOrEmpty(text))
                {
                    string cellName = GetCalendarCellName(endDate, startDate, text, rowIndex == -1);

                    // The cell is present on the calendar, so create accessible object for it.
                    return new CalendarCellAccessibleObject(this, calendarIndex, parentAccessibleObject, rowIndex, columnIndex, cellName);
                }

                return null;
            }

            private string GetCalendarCellName(DateTime endDate, DateTime startDate, string defaultName, bool headerCell)
            {
                if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH)
                {
                    if (headerCell)
                    {
                        return startDate.ToString("dddd");
                    }

                    return startDate.ToString("dddd, MMMM dd, yyyy");
                }
                else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_YEAR)
                {
                    return startDate.ToString("MMMM yyyy");
                }

                return defaultName;
            }

            private CalendarRowAccessibleObject GetCalendarRow(int calendarIndex, AccessibleObject parentAccessibleObject, int rowIndex)
            {
                if ((HasHeaderRow ? rowIndex < -1 : rowIndex < 0) ||
                    rowIndex >= RowCount)
                {
                    return null;
                }

                // Search name for the first cell in the row.
                bool success = GetCalendarGridInfo(
                    Interop.MonthCalendar.GridInfoFlags.MCGIF_DATE,
                    Interop.MonthCalendar.Part.MCGIP_CALENDARCELL,
                    calendarIndex,
                    rowIndex,
                    0,
                    out RECT calendarPartRectangle,
                    out NativeMethods.SYSTEMTIME endDate,
                    out NativeMethods.SYSTEMTIME startDate);

                if (!success)
                {
                    // Not able to get cell date for the row.
                    return null;
                }

                SelectionRange cellsRange = _owner.GetDisplayRange(false);

                if (cellsRange.Start > DateTimePicker.SysTimeToDateTime(endDate) || cellsRange.End < DateTimePicker.SysTimeToDateTime(startDate))
                {
                    // Do not create row if the row's first cell is out of the current calendar's view range.
                    return null;
                }

                return new CalendarRowAccessibleObject(this, calendarIndex, (CalendarBodyAccessibleObject)parentAccessibleObject, rowIndex);
            }

            private bool GetCalendarGridInfo(
                Interop.MonthCalendar.GridInfoFlags dwFlags,
                Interop.MonthCalendar.Part dwPart,
                int calendarIndex,
                int row,
                int column,
                out RECT rectangle,
                out NativeMethods.SYSTEMTIME endDate,
                out NativeMethods.SYSTEMTIME startDate)
            {
                Debug.Assert(
                    (dwFlags & ~(Interop.MonthCalendar.GridInfoFlags.MCGIF_DATE | Interop.MonthCalendar.GridInfoFlags.MCGIF_RECT)) == 0,
                    "GetCalendarGridInfo() should be used only to obtain Date and Rect,"
                    + "dwFlags has flag bits other that MCGIF_DATE and MCGIF_RECT");

                Interop.MonthCalendar.MCGRIDINFO gridInfo = new Interop.MonthCalendar.MCGRIDINFO();
                gridInfo.dwFlags = (uint)dwFlags;
                gridInfo.cbSize = (uint)Marshal.SizeOf(gridInfo);
                gridInfo.dwPart = (uint)dwPart;
                gridInfo.iCalendar = calendarIndex;
                gridInfo.iCol = column;
                gridInfo.iRow = row;
                bool result;

                try
                {
                    result = GetCalendarGridInfo(ref gridInfo);
                    rectangle = gridInfo.rc;
                    endDate = gridInfo.stEnd;
                    startDate = gridInfo.stStart;
                }
                catch
                {
                    rectangle = new RECT();
                    endDate = new NativeMethods.SYSTEMTIME();
                    startDate = new NativeMethods.SYSTEMTIME();
                    result = false;
                }

                return result;
            }

            private bool GetCalendarGridInfo(ref Interop.MonthCalendar.MCGRIDINFO gridInfo)
            {
                // Do not use this if gridInfo.dwFlags contains MCGIF_NAME;
                // use GetCalendarGridInfoText() instead.
                Debug.Assert(
                    (gridInfo.dwFlags & (uint)Interop.MonthCalendar.GridInfoFlags.MCGIF_NAME) == 0,
                    "Param dwFlags contains MCGIF_NAME, use GetCalendarGridInfoText() to retrieve the text of a calendar part.");

                gridInfo.dwFlags &= ~(uint)Interop.MonthCalendar.GridInfoFlags.MCGIF_NAME;

                return _owner.SendMessage((int)Interop.MonthCalendar.Messages.MCM_GETCALENDARGRIDINFO, 0, ref gridInfo) != IntPtr.Zero;
            }

            private bool GetCalendarGridInfoText(Interop.MonthCalendar.Part dwPart, int calendarIndex, int row, int column, out string text)
            {
                const int nameLength = 128;

                Interop.MonthCalendar.MCGRIDINFO gridInfo = new Interop.MonthCalendar.MCGRIDINFO();
                gridInfo.cbSize = (uint)Marshal.SizeOf(gridInfo);
                gridInfo.dwPart = (uint)dwPart;
                gridInfo.iCalendar = calendarIndex;
                gridInfo.iCol = column;
                gridInfo.iRow = row;
                gridInfo.pszName = new string('\0', nameLength + 2);
                gridInfo.cchName = (uint)gridInfo.pszName.Length - 1;

                bool result = GetCalendarGridInfoText(ref gridInfo);
                text = gridInfo.pszName;

                return result;
            }

            // Use to retrieve MCGIF_NAME only.
            private bool GetCalendarGridInfoText(ref Interop.MonthCalendar.MCGRIDINFO gridInfo)
            {
                Debug.Assert(
                    gridInfo.dwFlags == 0,
                    "gridInfo.dwFlags should be 0 when calling GetCalendarGridInfoText");

                gridInfo.dwFlags = (uint)Interop.MonthCalendar.GridInfoFlags.MCGIF_NAME;

                return _owner.SendMessage((int)Interop.MonthCalendar.Messages.MCM_GETCALENDARGRIDINFO, 0, ref gridInfo) != IntPtr.Zero;
            }

            public bool GetCalendarPartRectangle(int calendarIndex, Interop.MonthCalendar.Part dwPart, int row, int column, out RECT calendarPartRectangle)
            {
                bool success = GetCalendarGridInfo(
                    Interop.MonthCalendar.GridInfoFlags.MCGIF_RECT,
                    dwPart,
                    calendarIndex,
                    row,
                    column,
                    out calendarPartRectangle,
                    out NativeMethods.SYSTEMTIME endDate, out NativeMethods.SYSTEMTIME startDate);

                if (success)
                {
                    success = UnsafeNativeMethods.MapWindowPoints(new HandleRef(this, Owner.Handle), new HandleRef(null, IntPtr.Zero), ref calendarPartRectangle, 2) != 0;
                }

                if (!success)
                {
                    calendarPartRectangle = new RECT();
                }

                return success;
            }

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_ControlTypePropertyId => ControlType,
                    NativeMethods.UIA_NamePropertyId => Name,
                    NativeMethods.UIA_IsGridPatternAvailablePropertyId => true,
                    NativeMethods.UIA_IsTablePatternAvailablePropertyId => true,
                    NativeMethods.UIA_IsLegacyIAccessiblePatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(int patternId) =>
                patternId switch
                {
                    var p when
                        p == NativeMethods.UIA_ValuePatternId ||
                        p == NativeMethods.UIA_GridPatternId ||
                        p == NativeMethods.UIA_TablePatternId ||
                        p == NativeMethods.UIA_LegacyIAccessiblePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            public void RaiseMouseClick(MouseEventArgs mouseEventArgs)
            {
                _owner.RaiseMouseClick(mouseEventArgs);
            }

            public void RaiseAutomationEventForChild(int automationEventId, DateTime selectionStart, DateTime selectionEnd)
            {
                AccessibleObject calendarChildAccessibleObject = GetCalendarChildAccessibleObject(selectionStart, selectionEnd);
                if (calendarChildAccessibleObject != null)
                {
                    calendarChildAccessibleObject.RaiseAutomationEvent(automationEventId);

                    if (automationEventId == NativeMethods.UIA_AutomationFocusChangedEventId)
                    {
                        _focused = calendarChildAccessibleObject;
                    }
                }
            }

            private AccessibleObject GetCalendarChildAccessibleObject(DateTime selectionStart, DateTime selectionEnd)
            {
                int columnCount = ColumnCount;

                AccessibleObject bodyAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);
                for (int row = 0; row < RowCount; row++)
                {
                    AccessibleObject rowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, bodyAccessibleObject, row);
                    for (int column = 0; column < columnCount; column++)
                    {
                        bool success = GetCalendarGridInfo(
                            Interop.MonthCalendar.GridInfoFlags.MCGIF_DATE,
                            Interop.MonthCalendar.Part.MCGIP_CALENDARCELL,
                            _calendarIndex,
                            row,
                            column,
                            out RECT calendarPartRectangle,
                            out NativeMethods.SYSTEMTIME systemEndDate,
                            out NativeMethods.SYSTEMTIME systemStartDate);

                        if (!success)
                        {
                            continue;
                        }

                        AccessibleObject cellAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, rowAccessibleObject, column);
                        if (cellAccessibleObject == null)
                        {
                            continue;
                        }

                        DateTime endDate = DateTimePicker.SysTimeToDateTime(systemEndDate);
                        DateTime startDate = DateTimePicker.SysTimeToDateTime(systemStartDate);

                        if (DateTime.Compare(selectionEnd, endDate) <= 0 &&
                            DateTime.Compare(selectionStart, startDate) >= 0)
                        {
                            return cellAccessibleObject;
                        }
                    }
                }

                return null;
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaders() => null;

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                if (!HasHeaderRow)
                {
                    return null;
                }

                UnsafeNativeMethods.IRawElementProviderSimple[] headers =
                    new UnsafeNativeMethods.IRawElementProviderSimple[MonthCalendarAccessibleObject.MAX_DAYS];
                AccessibleObject headerRowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, -1);
                for (int columnIndex = 0; columnIndex < MonthCalendarAccessibleObject.MAX_DAYS; columnIndex++)
                {
                    headers[columnIndex] = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, headerRowAccessibleObject, columnIndex);
                }

                return headers;
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple GetItem(int row, int column)
            {
                AccessibleObject rowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, row);

                if (rowAccessibleObject == null)
                {
                    return null;
                }

                return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, rowAccessibleObject, column);
            }
        }
    }
}
