// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace WinformsControlsTest
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException); //UnhandledExceptionMode.ThrowException
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Application.Run(new Form1());
            }
            catch (System.Exception)
            {
                Environment.Exit(-1);
            }
            Environment.Exit(0);
        }
    }

    class Form1 : Form
    {
        public Form1()
        {
            this.IsMdiContainer = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var child = new Form();
            child.MdiParent = this;
            child.WindowState = FormWindowState.Maximized;
            child.Show();
        }
    }
}
