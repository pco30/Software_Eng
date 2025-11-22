using System;
using System.Net;
using System.Text;
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

        public static String PrettyString(Object obj)
        {
            try
            {
                if (obj == null)
                {
                    return null;
                }

                if (obj is Byte[] byteArrayObj)
                {
                    return BitConverter.ToString(byteArrayObj);
                }

                if (obj is String[] stringArrayObj)
                {
                    StringBuilder sb = new StringBuilder();
                    Boolean isFirst = true;
                    foreach(String s in stringArrayObj)
                    {
                        if (isFirst)
                        {
                            sb.Append((Char)0);
                        }
                        sb.Append(s);
                        isFirst = true;
                    }
                    return sb.ToString();
                }

                return obj.ToString();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public static String HandleValueWithZeroDelimiter(String value)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                String[] split = value.Split((Char)0); // handle REG_MULTI_SZ where (Char)0 is the delimiter
                foreach (String s in split)
                {
                    sb.AppendLine($"<div>{WebUtility.HtmlEncode(s)}</div>");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }
    }
}