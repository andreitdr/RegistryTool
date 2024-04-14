using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using RegistryTool.Forms.CreateEntry;
using RegistryTool.Forms.RemoveEntry;

namespace RegistryTool
{
	public partial class ToolForm : Form
	{
		public ToolForm()
		{
			InitializeComponent();
            Load += FormLoaded;

        }

        private void FormLoaded(object sender, EventArgs e)
        {

			if(!IsWindows11() || PatchApplied())
			{
				button4.Enabled = false;
			}

            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("RegistryTool", true);
                key.OpenSubKey("ToolKeys", true);
                key.Close();
            }
            catch
            {
                RegistryKey key = Registry.ClassesRoot.CreateSubKey("RegistryTool");
                key = Registry.ClassesRoot.OpenSubKey("RegistryTool", true);
                key.CreateSubKey("ToolKeys");
                key.Close();
                MessageBox.Show("The tool has been installed successfully !\nThis message will not show up again (unless you click the Delete button)!", "RegistryTool",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

		private void button1_Click(object sender, EventArgs e)
		{
			new CreateEntryForm().Show();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			new RemoveEntryForm().Show();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				RegistryKey key = Registry.ClassesRoot.OpenSubKey("RegistryTool", true);
				key.OpenSubKey("ToolKeys", true);
				key.DeleteSubKeyTree("ToolKeys");
				key.Close();
				Registry.ClassesRoot.DeleteSubKey("RegistryTool");
				MessageBox.Show("Application can be removed by moving it to the recycle bin." +
					" All Registries created for this application to work had been deleted\n" +
	 "This does not include the registries created by YOU with this tool !",
	 "Registry Tool",MessageBoxButtons.OK,MessageBoxIcon.Information);
				this.Close();
				
			}
			catch
			{
				RegistryKey key = Registry.ClassesRoot.CreateSubKey("RegistryTool");
				key = Registry.ClassesRoot.OpenSubKey("RegistryTool", true);
				key.CreateSubKey("ToolKeys");
				key.Close();
				MessageBox.Show("The tool has been installed successfully !\nThis message will not show up again !", "RegistryTool",
					MessageBoxButtons.OK, MessageBoxIcon.Information);

			}
		}

        private bool PatchApplied()
        {
            var newKey = Registry.ClassesRoot.OpenSubKey("RegistryTool\\ToolKeys", true);
            if (newKey is null) return false;
            if(newKey.GetValue("Win11_Patch") != null)
            {
                return true;
            }

            return false;
        }

        private bool IsWindows11()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            var currentBuildStr = (string)reg.GetValue("CurrentBuild");
            var currentBuild = int.Parse(currentBuildStr);

            return currentBuild >= 22000;
        }

        private void button4_Click(object sender, EventArgs e)
        {
			var key = Registry.CurrentUser.OpenSubKey(@"SOftware\Classes\CLSID", true);
			key.CreateSubKey("{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
            key = key.OpenSubKey("{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", true);
			key.CreateSubKey("InprocServer32");
            key.Close();

            var newKey = Registry.ClassesRoot.OpenSubKey("RegistryTool\\ToolKeys", true);
            newKey.SetValue("Win11_Patch", true);
            newKey.Close();

            RestartExplorer();

            button4.Enabled = false;

        }

        private void RestartExplorer()
        {
            using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo
                {
                    FileName = "taskkill.exe",
                    Arguments = "-f -im explorer.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                };
                p.Start();
                p.WaitForExit();
                p.StartInfo = new ProcessStartInfo("explorer.exe");
                p.Start();
            }
        }
    }
}
