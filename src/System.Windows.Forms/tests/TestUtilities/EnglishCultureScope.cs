// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;

namespace System
{
    public class EnglishCultureScope : IDisposable
    {
        private CultureInfo _culture;
        private CultureInfo _uiculture;

        public EnglishCultureScope()
        {
            _culture = Thread.CurrentThread.CurrentCulture;
            _uiculture = Thread.CurrentThread.CurrentUICulture;

            var english = CultureInfo.GetCultureInfo("en-US");

            Thread.CurrentThread.CurrentCulture = english;
            Thread.CurrentThread.CurrentUICulture = english;
        }

        public void Dispose()
        {
            if (_culture != null)
            {
                Thread.CurrentThread.CurrentCulture = _culture;
                _culture = null;
            }

            if (_uiculture != null)
            {
                Thread.CurrentThread.CurrentUICulture = _uiculture;
                _uiculture = null;
            }
        }
    }
}
