using Microsoft.Win32;
using System;
using System.Windows.Forms;


namespace RegistryTool.Forms.CreateEntry
{
	public partial class CreateEntryForm : Form
	{
		public CreateEntryForm()
		{
			InitializeComponent();
			Startup();
		}

		void Startup()
		{
			optionsBox.DropDownStyle = ComboBoxStyle.DropDownList;
			optionsBox.Items.Add("Right-Click Menu for Desktop");
			optionsBox.Items.Add("Right-Click Menu for Files");
			optionsBox.Items.Add("Richt-Click Menu for Folders");
			richTextBox1.Visible = false;
			comboBox1.Items.Add("Top");
			comboBox1.Items.Add("Middle");
			comboBox1.Items.Add("Bottom");
			comboBox2.Items.Add("None");
			comboBox2.Items.Add("Application's own");
			comboBox2.Items.Add("Custom");
			
			textBox3.Visible = false;
			optionsBox.SelectedIndex = 0;
			comboBox1.SelectedIndex = 0;
			comboBox2.SelectedIndex = 0;
			try
			{
				this.Icon = new System.Drawing.Icon(@".\RegT.ico");
			}
			catch
			{
				//MessageBox.Show("Could not locate the icon file !", "Registry Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			Update_Form(0);
		}
		void Update_Form(int Option)
		{
			if(Option == 0) // desktop
			{
				label2.Text = "Please insert your Menu Item Name (ex. Open MyFile)";
				label3.Text = "Please insert your Application (.bat or .exe) Path";
			}
			if(Option == 1) // file
			{
				label2.Text = "Please insert your Menu Item Name (ex. Open this file with MyApp)";
				label3.Text = "Please insert your Application (.exe) Path";
			}
			if(Option == 2) // folder
			{
				label2.Text = "Please insert your Menu Item Name (ex. Open this folder with MyApp)";
				label3.Text = "Please insert your Application (.exe or .bat) Path";
			}
		}

		private void ChangedItem(object sender, EventArgs e)
		{
			Update_Form(optionsBox.SelectedIndex);
		}

		private void FileFinder(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
				textBox2.Text = @ofd.FileName;
		}
		private void button1_Click(object sender, EventArgs e)
		{
			
			int option = optionsBox.SelectedIndex;
			string KeyName = textBox1.Text;
			if (KeyName.Length > 1)
			{
				string path = textBox2.Text;
				if (path.Contains(".exe") || path.Contains(".bat") || path.Contains(".cmd"))
				{
					string Key = "";
					switch (option)
					{
						case 0: // desktop
							Key = "Directory\\Background\\shell";
							break;
						case 1: // file
							Key = "*\\shell";
							break;
						case 2: // folder
							Key = "Directory\\shell";
							break;

					}
					if (textBox3.Text.Length < 3)
						MessageBox.Show("Can not create a registry with a custom icon file that has a path with less then 3 characters !", "Registry Tool",MessageBoxButtons.OK,MessageBoxIcon.Information);
					else
						ChangeRegistry(KeyName, path, Key);
				}
				else
				{
					MessageBox.Show("Can not create a registry with a file defferent then .exe , .bat or .cmd", "Registry Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show("Can not create a registry with a name that has less then [1] Character !", "Registry Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			
		}
		void ChangeRegistry(string Name, string filep, string regkey)
		{
			try
			{
				richTextBox1.Clear();
				richTextBox1.Visible = true;
				RegistryKey key = Registry.ClassesRoot.OpenSubKey(regkey, true);
				richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Successfully opened SubKey [{regkey}] ! \n");
				progressBar1.Value = 1 / 11 * 100;
				if (!KeyExistance(regkey, Name))
				{
					key.CreateSubKey(Name);
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Successfully created SubKey [{regkey}] with name [{Name}] !\n");
					progressBar1.Value = 2 / 11 * 100;
					key.Close();
					regkey += "\\" + Name;
					key = Registry.ClassesRoot.OpenSubKey(regkey, true);
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Successfully opened SubKey [{regkey}] ! \n");
					progressBar1.Value = 3 / 11 * 100;
					if(comboBox2.SelectedIndex == 1) // application's
					{
						key.SetValue("Icon", @filep);
					}
					else
					{
						if(comboBox2.SelectedIndex == 2) // custom
						{
							key.SetValue("Icon", @textBox3.Text);
						}
					}
					string pos = comboBox1.SelectedItem.ToString();
					key.SetValue("Position", pos);
					key.CreateSubKey("command");
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Successfully created SubKey [{regkey}] with name [{Name}] !\n");
					progressBar1.Value = 4 / 11 * 100;
					key.Close();
					regkey += "\\command";
					key = Registry.ClassesRoot.OpenSubKey(regkey, true);
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Successfully opened SubKey [{regkey}] ! \n");
					progressBar1.Value = 5 / 11 * 100;
					key.SetValue("", @filep);
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Successfully updated Key [{Name}] from [{regkey}] with value [{filep}] !\n");
					progressBar1.Value = 6 / 11 * 100;
					key.Close();
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Storing the data...\n");
					progressBar1.Value = 7 / 11 * 100;
					var newKey = Registry.ClassesRoot.OpenSubKey("RegistryTool\\ToolKeys", true);
					progressBar1.Value = 10 / 11 * 100;
					newKey.SetValue(Name, regkey);
					progressBar1.Value = 11 / 11 * 100;
					richTextBox1.AppendText($"[{DateTime.Now.ToLongTimeString()}] Operation has been finished successfully !\n");
					newKey.Close();

				}
				else
				{
					richTextBox1.Clear();
					richTextBox1.AppendText($"Could not add a key with the same name !\nThere already exist a key with this name ([{Name}])\n");
					progressBar1.Value = 0;
				}

				textBox1.Text = "";
				textBox2.Text = "";
			}
			catch (Exception ex)
			{
				progressBar1.Value = 0;
				try
				{
					richTextBox1.AppendText("[ERROR]"+ex.Message + "\nPlease check the Dump Message created in the 'Error' Folder\n");
                }
				catch
				{
					MessageBox.Show(ex.ToString(),"Registry Tool", MessageBoxButtons.OK,MessageBoxIcon.Error);
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
				MessageBox.Show("Could not open the key " + path + " !", "Registry Tool", MessageBoxButtons.OK,MessageBoxIcon.Error);
				return false;
			}


		}

		private void IconPathCustom(object sender, EventArgs e)
		{
			string iconpath = "";
			OpenFileDialog d = new OpenFileDialog();
			d.Title = "Select your custom .ico file";
			if (d.ShowDialog() == DialogResult.OK)
				iconpath = d.FileName;
			if (!iconpath.Contains(".ico"))
				MessageBox.Show("Required .ico file !","Registry Tool", MessageBoxButtons.OK,MessageBoxIcon.Information);
			else
				textBox3.Text = iconpath;
		}

		private void NewIndexChanged(object sender, EventArgs e)
		{
			if (comboBox2.SelectedIndex == 2)
				textBox3.Visible = true;
			else
			{
				textBox3.Visible = false;
				textBox3.Text = "Click here to select the Icon File";
			}
		}
	}
}
