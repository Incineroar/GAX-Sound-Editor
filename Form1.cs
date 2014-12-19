using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ini;

namespace GAX_Sound_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string fileLocation;
        List<string> soundTable = new List<string>();
        DataTable dt = new DataTable();
        string gameCode;
        IniFile gameConfig = new IniFile(Application.StartupPath + "/games.ini");

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + "/games.ini") == false)
            {
                MessageBox.Show("Games INI file not found! Cannot continue!");
                return;
            }
            else
            {
                dataGridView1.DataSource = dt;
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    fileLocation = ofd.FileName;
                    BinaryReader br = new BinaryReader(new FileStream(fileLocation, FileMode.Open));
                    br.BaseStream.Seek(0xAC, SeekOrigin.Begin);
                    gameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
                    br.Close();
                    switch (gameConfig.IniReadValue(gameCode, "LoadFormat"))
                    {
                        case "0": //Case where audio table is known.
                            {
                                soundTable.Clear();
                                long tableOffset = long.Parse(gameConfig.IniReadValue(gameCode, "TableOffset"), System.Globalization.NumberStyles.HexNumber);
                                loadAudioTable(fileLocation, tableOffset);
                                break;
                            }
                        case "1":
                            {
                                soundTable.Clear();
                                loadTableFromINI();
                                break;
                            }
                        default:
                            {
                                listBox1.Items.Clear();
                                MessageBox.Show("Either there is no LoadFormat configuration for this particular game, or this game does not exist in the configuration! Cannot continue!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                    }

                    listBox1.Items.Clear();

                    foreach (string s in soundTable)
                    {
                        listBox1.Items.Add(s);
                    }
                }
            }
        }

        private void loadAudioTable(string fileLocation, long tableOffset)
        {
            bool isValid = true;
            FileStream fs = new FileStream(fileLocation, FileMode.Open);
            fs.Seek(tableOffset, SeekOrigin.Begin);
            while (isValid)
            {
                byte[] tempBuffer = new byte[4];
                fs.Read(tempBuffer, 0, tempBuffer.Length);
                if (tempBuffer[3] == 8)
                {
                    soundTable.Add(tempBuffer[2].ToString("X2") + tempBuffer[1].ToString("X2") + tempBuffer[0].ToString("X2"));
                }
                else
                    isValid = false;
            }
            fs.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dt.Reset();
            dataGridView1.Refresh();
            int tableCount = 1;
            bool isValid = true;
            FileStream fs = new FileStream(fileLocation, FileMode.Open);
            fs.Seek(long.Parse(listBox1.SelectedItem.ToString(), System.Globalization.NumberStyles.HexNumber) + 0xC, SeekOrigin.Begin);
            byte[] tmp = new byte[4];
            fs.Read(tmp, 0, tmp.Length);
            txtDataBlock.Text = "0x" + tmp[2].ToString("X2") + tmp[1].ToString("X2") + tmp[0].ToString("X2");
            fs.Read(tmp, 0, tmp.Length);
            txtSampleTable.Text = "0x" + tmp[2].ToString("X2") + tmp[1].ToString("X2") + tmp[0].ToString("X2");
            fs.Read(tmp, 0, tmp.Length);
            if (tmp[3] == 0x08)
            {
                txtUnknown.Text = "0x" + tmp[2].ToString("X2") + tmp[1].ToString("X2") + tmp[0].ToString("X2");
                fs.Seek(0x08, SeekOrigin.Current);
            }
            else
            {
                txtUnknown.Text = "Not present.";
                fs.Seek(0x04, SeekOrigin.Current);
            }
            

            while (isValid)
            {

                byte[] tempBuffer = new byte[4];
                fs.Read(tempBuffer, 0, tempBuffer.Length);
                if (tempBuffer[3] == 8)
                {
                    dt.Columns.Add("Channel " + tableCount.ToString() + " - 0x" + tempBuffer[2].ToString("X2") + tempBuffer[1].ToString("X2") + tempBuffer[0].ToString("X2"));
                    tableCount++;
                }
                else
                    isValid = false;
            }
            fs.Close();
        }

        private void loadTableFromINI()
        {
            bool isNotNull = true;
            int i = 1;
            while (isNotNull)
            {
                if (gameConfig.IniReadValue(gameCode, "Song" + i.ToString()) != "")
                {
                    soundTable.Add(gameConfig.IniReadValue(gameCode, "Song" + i.ToString()));
                    i++;
                }
                else
                    break;
            }
        }

        private void getReadFormat()
        {
            switch (gameConfig.IniReadValue(gameCode, "ReadFormat"))
            {

            }
        }
    }
}
