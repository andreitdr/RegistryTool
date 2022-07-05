using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace RegistryTool.Forms.RemoveEntry
{
	public partial class RemoveEntryForm : Form
	{
		public RemoveEntryForm()
		{
			InitializeComponent();
			StartUP();
		}
		void StartUP()
		{

			optionsBox.Items.Add("Right-Click Menu for Desktop");
			optionsBox.Items.Add("Right-Click Menu for Files");
			optionsBox.Items.Add("Richt-Click Menu for Folders");

			comboBox1.Items.Add("Menu Item created by Windows");
			comboBox1.Items.Add("Menu Item created with RegistryTool");
			richTextBox1.Visible = false;
			comboBox2.Visible = false;
			textBox1.Visible = false;
			
		}
		void PrepareDelete(string name)
		{
			string Key = GetPathOption();
			DeleteRegistry(Key, name);
		}
		private void button1_Click(object sender, EventArgs e)
		{

			if (comboBox1.SelectedIndex == 1)
			{
				if (comboBox2.SelectedItem != null)
					PrepareDelete(comboBox2.SelectedItem.ToString());
				else
					richTextBox1.AppendText("[ERROR] Could not delete an entry that does not has a name !");
			}
			else
			{
				if(textBox1.Text != null || textBox1.Text != "")
				   PrepareDelete(textBox1.Text);
				else
					richTextBox1.AppendText("[ERROR] Could not delete an entry that does not has a name !");
			}
		}
		void DeleteRegistry(string regkey, string keyCommand)
		{
			try
			{
				richTextBox1.Clear();
				richTextBox1.Visible = true;
				RegistryKey key = Registry.ClassesRoot.OpenSubKey(regkey, true);
				richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Successfully opened SubKey [{regkey}] ! \n");
				progressBar1.Value = 1 / 4 * 100;
				if (KeyExistance(regkey, keyCommand))
				{
					key.DeleteSubKeyTree(keyCommand);
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Successfully deleted Key [{keyCommand}] from [{regkey}] ! \n");
					progressBar1.Value = 2 / 4 * 100;
					key.Close();
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Deleting the key from Database... !\n");
					progressBar1.Value = 3 / 4 * 100;

					key = Registry.ClassesRoot.OpenSubKey("RegistryTool\\ToolKeys", true);
					key.DeleteValue(keyCommand);
					progressBar1.Value = 4 / 4 * 100;
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Operation has been finished successfully !\n");

					reload_registry();
				}
				else
				{
					richTextBox1.Clear();
					progressBar1.Value = 0;
					richTextBox1.AppendText($"Could not find [{keyCommand}] in [{regkey}] ! Please select another option !\n");
					return;
				}

			}
			catch (Exception e)
			{
				progressBar1.Value = 0;
				
				try
				{
					richTextBox1.AppendText("[ERROR]"+e.Message+ "\nPlease check the Dump Message created in the 'Error' Folder\n");
                }
				catch
				{
					MessageBox.Show(e.ToString(), "RegistryTool", MessageBoxButtons.OK,MessageBoxIcon.Error);
				}
			}
		}
		public bool KeyExistance(string path, string name)
		{
			try
			{
				RegistryKey key = Registry.ClassesRoot.OpenSubKey(path);
				foreach (string aux in key.GetSubKeyNames())
				{
					if (aux == name)
						return true;
				}
				key.Close();
				return false;

			}
			catch
			{
				MessageBox.Show("Could not open the key " + path + " !");
				return false;
			}


		}
		void GetRegistry(string option)
		{
			RegistryKey k = Registry.ClassesRoot.OpenSubKey("RegistryTool\\ToolKeys", true);
			foreach (string name in k.GetValueNames())
			{
				if (k.GetValue(name).ToString().Contains(option))
					comboBox2.Items.Add(name);
			}
		}
		private string GetPathOption()
		{
			int op = optionsBox.SelectedIndex;
			switch (op)
			{
				case 0: // desktop
					return "Directory\\Background\\shell";
				case 1: // file
					return "*\\shell";
				case 2: // folder
					return "Directory\\shell";
			}
			return "";
		}
		private void OptionChanged(object sender, EventArgs e)
		{
			reload_registry();
		}
		void reload_registry()
		{
			if (comboBox1.SelectedIndex == 0)
			{
				comboBox2.Items.Clear();
				comboBox2.Visible = false;
				textBox1.Visible = true;
			}
			else
			{
				comboBox2.Items.Clear();
				textBox1.Visible = false;
				comboBox2.Visible = true;
				GetRegistry(GetPathOption());
			}
		}
		private void ContextMenuChanged1(object sender, EventArgs e)
		{
			reload_registry();
		}
	}
}
