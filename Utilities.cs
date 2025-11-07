using System;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    internal static class Utilities
    {
        public static void HandleException(Exception ex)
        {
            MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        delegate void CheckBoxSetCheckedDelegate(CheckBox checkBox, Boolean value);
        public static void CheckBoxSetChecked(CheckBox checkBox, Boolean value)
        {
            try
            {
                if (checkBox.InvokeRequired)
                {
                    checkBox.Invoke(new CheckBoxSetCheckedDelegate(CheckBoxSetChecked), new Object[] { checkBox, value });
                }
                else
                {
                    checkBox.Checked = value;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        delegate void ToggleSwitchSetCheckedDelegate(JCS.ToggleSwitch toggleSwitch, Boolean value);
        public static void ToggleSwitchSetChecked(JCS.ToggleSwitch toggleSwitch, Boolean value)
        {
            try
            {
                if (toggleSwitch.InvokeRequired)
                {
                    toggleSwitch.Invoke(new ToggleSwitchSetCheckedDelegate(ToggleSwitchSetChecked), new object[] { toggleSwitch, value });
                }
                else
                {
                    toggleSwitch.Checked = value;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        delegate void ControlSetTextDelegate(Control control, String text);
        public static void ControlSetText(Control control, String text)
        {
            try
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(new ControlSetTextDelegate(ControlSetText), new Object[] { control, text });
                }
                else
                {
                    control.Text = text;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }        

        delegate void TextBoxAppendTextDelegate(TextBox textBox, String text);
        public static void TextBoxAppendText(TextBox textBox, String text)
        {
            try
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke(new TextBoxAppendTextDelegate(TextBoxAppendText), new Object[] { textBox, text });
                }
                else
                {
                    textBox.AppendText(text);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        delegate void TextBoxClearDelegate(TextBox textBox);
        public static void TextBoxClear(TextBox textBox)
        {
            try
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke(new TextBoxClearDelegate(TextBoxClear), new Object[] { textBox });
                }
                else
                {
                    textBox.Clear();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
