// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  This date/time editor is a UITypeEditor suitable for
    ///  visually editing DateTime objects.
    /// </summary>
    public class DateTimeEditor : UITypeEditor
    {
        /// <summary>
        ///  Edits the given object value using the editor style provided by
        ///  GetEditorStyle.  A service provider is provided so that any
        ///  required editing services can be obtained.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    using (DateTimeUI dateTimeUI = new DateTimeUI())
                    {
                        dateTimeUI.Start(edSvc, value);
                        edSvc.DropDownControl(dateTimeUI);
                        value = dateTimeUI.Value;
                        dateTimeUI.End();
                    }
                }
            }

            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method.  If the method
        ///  is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <include file='doc\DateTimeEditor.uex' path='docs/doc[@for="DateTimeEditor.DateTimeUI"]/*' />
        /// <devdoc>
        ///      UI we drop down to pick dates.
        /// </devdoc>
        private class DateTimeUI : Control
        {
            private MonthCalendar monthCalendar = new DateTimeMonthCalendar();
            private object value;
            private IWindowsFormsEditorService edSvc;

            /// <include file='doc\DateTimeEditor.uex' path='docs/doc[@for="DateTimeEditor.DateTimeUI.DateTimeUI"]/*' />
            /// <devdoc>
            /// </devdoc>
            public DateTimeUI()
            {
                InitializeComponent();
                Size = monthCalendar.SingleMonthSize;
                monthCalendar.Resize += new EventHandler(this.MonthCalResize);
            }

            /// <include file='doc\DateTimeEditor.uex' path='docs/doc[@for="DateTimeEditor.DateTimeUI.Value"]/*' />
            /// <devdoc>
            /// </devdoc>
            public object Value
            {
                get
                {
                    return value;
                }
            }

            /// <include file='doc\DateTimeEditor.uex' path='docs/doc[@for="DateTimeEditor.DateTimeUI.End"]/*' />
            /// <devdoc>
            /// </devdoc>
            public void End()
            {
                edSvc = null;
                value = null;
            }

            private void MonthCalKeyDown(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        OnDateSelected(sender, null);
                        break;
                }
            }

            protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
            {
                base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
                //if (!DpiHelper.EnableDpiChangedHighDpiImprovements)
                //{
                //    return;
                //}

                //Resizing the editor to fit to the SingleMonth size after Dpi changed.
                Size = monthCalendar.SingleMonthSize;
            }

            /// <include file='doc\DateTimeEditor.uex' path='docs/doc[@for="DateTimeEditor.DateTimeUI.InitializeComponent"]/*' />
            /// <devdoc>
            /// </devdoc>
            private void InitializeComponent()
            {
                monthCalendar.DateSelected += new DateRangeEventHandler(this.OnDateSelected);
                monthCalendar.KeyDown += new KeyEventHandler(this.MonthCalKeyDown);
                this.Controls.Add(monthCalendar);
            }

            private void MonthCalResize(object sender, EventArgs e)
            {
                this.Size = monthCalendar.Size;
            }

            /// <include file='doc\DateTimeEditor.uex' path='docs/doc[@for="DateTimeEditor.DateTimeUI.OnDateSelected"]/*' />
            /// <devdoc>
            /// </devdoc>
            private void OnDateSelected(object sender, DateRangeEventArgs e)
            {
                value = monthCalendar.SelectionStart;
                edSvc.CloseDropDown();
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                monthCalendar.Focus();
            }

            /// <include file='doc\DateTimeEditor.uex' path='docs/doc[@for="DateTimeEditor.DateTimeUI.Start"]/*' />
            /// <devdoc>
            /// </devdoc>
            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                this.edSvc = edSvc;
                this.value = value;

                if (value != null)
                {
                    DateTime dt = (DateTime)value;
                    monthCalendar.SetDate((dt.Equals(DateTime.MinValue)) ? DateTime.Today : dt);
                }
            }

            class DateTimeMonthCalendar : MonthCalendar
            {
                protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
                {
                    switch (keyData)
                    {
                        case Keys.Enter:
                            return true;
                    }
                    return base.IsInputKey(keyData);
                }
            }
        }
    }
}
