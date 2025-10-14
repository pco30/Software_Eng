using Microsoft.Win32;
using System;
using System.Drawing;
using System.Security;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class RegistryKeyBrowserDialog : Form
    {
        public String SelectedKeyPath {  get; private set; }

        private const String HKCR = "HKEY_CLASSES_ROOT";
        private const String HKCU = "HKEY_CURRENT_USER";
        private const String HKLM = "HKEY_LOCAL_MACHINE";
        private const String HKU = "HKEY_USERS";
        private const String HKCC = "HKEY_CURRENT_CONFIG";

        public RegistryKeyBrowserDialog()
        {
            InitializeComponent();
        }

        private void RegistryKeyBrowserDialog_Load(Object sender, EventArgs e)
        {
            try
            {
                this.tvBrowser.Nodes.Clear();

                this.tvBrowser.Nodes.Add(HKCR);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Tag = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Nodes.Add("");
                this.tvBrowser.Nodes.Add(HKCU);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Tag = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Nodes.Add("");
                this.tvBrowser.Nodes.Add(HKLM);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Tag = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Nodes.Add("");
                this.tvBrowser.Nodes.Add(HKU);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Tag = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Nodes.Add("");
                this.tvBrowser.Nodes.Add(HKCC);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Tag = RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64);
                this.tvBrowser.Nodes[this.tvBrowser.Nodes.Count - 1].Nodes.Add("");

                this.lSelectedKey.Text = "";
                this.bOK.Enabled = false;

                CenterOnScreen();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void PopulateTreeView(RegistryKey parentKey, TreeNode parentNode)
        {
            try
            {
                foreach (String subkey in parentKey.GetSubKeyNames())
                {
                    try
                    {
                        TreeNode childNode = parentNode.Nodes.Add(subkey);
                        childNode.Tag = parentKey.OpenSubKey(subkey);

                        childNode.Nodes.Add("");
                    }
                    catch (SecurityException) { }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void tvBrowser_BeforeExpand(Object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                if ((e.Node.Nodes.Count > 0) && (e.Node.Nodes[0].Text == String.Empty))
                {
                    RegistryKey key = (RegistryKey)e.Node.Tag;

                    e.Node.Nodes.Clear();

                    PopulateTreeView(key, e.Node);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void tvBrowser_AfterSelect(Object sender, TreeViewEventArgs e)
        {
            try
            {
                this.lSelectedKey.Text = e.Node.FullPath;
                this.bOK.Enabled = true;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void bOK_Click(Object sender, EventArgs e)
        {
            try
            {
                this.SelectedKeyPath = this.lSelectedKey.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void bCancel_Click(Object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public void CenterOnScreen()
        {
            try
            {
                Screen displayScreen = Screen.FromControl(this);
                this.Location = new Point((displayScreen.WorkingArea.Width - this.Width) / 2, (displayScreen.WorkingArea.Height - this.Height) / 2);
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
        }
    }
}
