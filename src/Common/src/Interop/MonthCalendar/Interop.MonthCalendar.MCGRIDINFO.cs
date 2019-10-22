// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static System.Windows.Forms.NativeMethods;

internal static partial class Interop
{
    internal static partial class MonthCalendar
    {
        /// <summary>
        ///  MonthCalendar grid info structure.
        ///  Copied form CommCtrl.h
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        unsafe public struct MCGRIDINFO
        {
            public uint cbSize;
            public uint dwPart;
            public uint dwFlags;
            public int iCalendar;
            public int iRow;
            public int iCol;
            public bool bSelected;
            public SYSTEMTIME stStart;
            public SYSTEMTIME stEnd;
            public RECT rc;
            public string pszName;
            public uint cchName;
        }
    }
}
