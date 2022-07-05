using System;
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
			Start_1();

        }
        void Start_1()
		{
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
				MessageBox.Show("The tool has been installed successfully !\nThis message will not show up again (unless you click the Delete button)!\n" +
					"(if it does, please contact me on discord : Wizzy#9181)", "RegistryTool",
					MessageBoxButtons.OK,MessageBoxIcon.Information);

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
				MessageBox.Show("The tool has been installed successfully !\nThis message will not show up again !\n(if it does, please contact me on discord : Wizzy#9181)", "RegistryTool",
					MessageBoxButtons.OK, MessageBoxIcon.Information);

			}
		}
	}
}
