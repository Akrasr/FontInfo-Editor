using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FontInfoEditorGUI
{
    public partial class Form1 : Form
    {
        MainManager mm;
        bool loaded = true;
        FontInfo.CharacterData tempcd = null; //copied CharacterData
        public Form1()
        {
            InitializeComponent();
            GroupBoxDisable();
            button1.Enabled = false;
            copyCharacterDataToolStripMenuItem.Enabled = false;
            pasteCharacterDataToolStripMenuItem.Enabled = false;
            copyPasteToolStripMenuItem.Enabled = false;
            label12.Enabled = false;
            label13.Enabled = false;
            label14.Enabled = false;
            label15.Enabled = false;
            trackBar1.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            listBox1.Enabled = false;
        }

        public void Open() 
        {
            string infoPath = "", atlasPath = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) //Getting the path of Font-Info file
            {
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Open font_info file";
                openFileDialog.Filter = "Unity file (*.dat, *.114)|*.dat;*.114|Any file (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                DialogResult dr = openFileDialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    infoPath = openFileDialog.FileName;
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) //Getting the path of Font Atlas file
            {
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Open font atlas";
                openFileDialog.Filter = "Image (*.png)|*.png|Any file (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                DialogResult dr = openFileDialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    atlasPath = openFileDialog.FileName;
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }
            List<Label> labels = new List<Label>(); //Adding x, y, z and code labels to the list
            labels.Add(label14);
            labels.Add(label12);
            labels.Add(label13);
            panel1.Controls.Add(pictureBox1); //Adding picturebox to the panel (panel is used for scrolling)
            mm = new MainManager(infoPath, atlasPath, pictureBox1, labels, this); //Making a MainManager
            AdaptPictureBox();
            listBox1.Items.AddRange(mm.GetItems().ToArray()); //Adding characters' codes to list
            button1.Enabled = true; //Enabling controls
            copyPasteToolStripMenuItem.Enabled = true;
            label12.Enabled = true;
            label13.Enabled = true;
            label14.Enabled = true;
            label15.Enabled = true;
            trackBar1.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            listBox1.Enabled = true;
        }

        private void AdaptPictureBox() //Adapting picturebox to the scale of an atlas
        {
            panel1.AutoScroll = false;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Size = mm.GetAtlasSize();
            panel1.AutoScroll = true;
            mm.ShowAtlas();
        }

        public void Save()
        {
            mm.Save();
        }

        private void SaveAs()
        {
            using (SaveFileDialog sfd = new SaveFileDialog()) //Getting file Path
            {
                sfd.RestoreDirectory = true;
                sfd.Title = "Save UI Atlas file as";
                sfd.Filter = "UnityEX MB file (*.114)|*.114|UABE MB file (*.dat)|*.dat|All files (*.*)|*.*";
                sfd.FilterIndex = 1;
                DialogResult dr = sfd.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    mm.SaveAs(sfd.FileName);
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }
        }

        void GroupBoxDisable()
        {
            loaded = false;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            checkBox1.Checked = false;
            button2.Enabled = false;
            groupBox1.Enabled = false;
            listBox1.ClearSelected();
            loaded = true;
            copyCharacterDataToolStripMenuItem.Enabled = false;
            pasteCharacterDataToolStripMenuItem.Enabled = false;
        }

        void LoadCharacterData(FontInfo.CharacterData cd)
        {
            button2.Enabled = true;
            groupBox1.Enabled = true;
            textBox1.Text = "" + cd.code;
            textBox2.Text = "" + cd.rx;
            textBox3.Text = "" + cd.ry;
            textBox4.Text = "" + cd.rw;
            textBox5.Text = "" + cd.rh;
            textBox6.Text = "" + cd.rm;
            textBox7.Text = "" + cd.lm;
            textBox8.Text = "" + cd.xo;
            textBox9.Text = "" + cd.yo;
            checkBox1.Checked = cd.isSpace;
            copyCharacterDataToolStripMenuItem.Enabled = true;
            if (tempcd != null)
                pasteCharacterDataToolStripMenuItem.Enabled = true;
            mm.SelectGlyphOnImage();
        }

        public void SelectCharacter(int index)
        {
            listBox1.SelectedIndex = index;
        }

        void DeleteCharacterData()
        {
            int ind = listBox1.SelectedIndex;
            if (ind == listBox1.Items.Count - 1)
                ind--;
            GroupBoxDisable();
            mm.DeleteCurrentCD();
            listBox1.Items.Clear(); //updating list
            listBox1.Items.AddRange(mm.GetItems().ToArray());
            if (listBox1.Items.Count != 0)
                listBox1.SelectedIndex = ind;
        }

        void AddCharacterData()
        {
            int ind = listBox1.Items.Count;
            mm.AddData();
            listBox1.Items.Clear(); //updating list
            listBox1.Items.AddRange(mm.GetItems().ToArray());
            listBox1.SelectedIndex = ind;
        }

        void CopyCharacterData()
        {
            tempcd = mm.CurrentCD;
            pasteCharacterDataToolStripMenuItem.Enabled = true;
        }

        void PasteCharacterData()
        {
            LoadCharacterData(tempcd);
        }

        void EditScale(float sc) //Changing the scale of an image
        {
            mm.SetScale(sc);
            AdaptPictureBox();
            mm.SelectGlyphOnImage();
        }

        private int GetInt(object sender) //function for parsing integers in textboxes
        {
            IntSave(sender);
            if (((TextBox)sender).Text == "-")
            {
                return 0;
            }
            if (((TextBox)sender).Text.Length == 0)
            {
                return 0;
            }
            return Int32.Parse(((TextBox)sender).Text);
        }

        public void IntSave(object sender) //function for deleting wrong characters in textboxes while parsing to int
        {
            string[] ints = { "-", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            TextBox tb = (TextBox)sender;
            string res = "";
            bool f = false;
            for (int i = 0; i < tb.Text.Length; i++)
            {
                if (("" + tb.Text.ToCharArray()[i]) == ints[0] && i != 0) // if '-' is not in the start, ignore it
                {
                    f = true;
                    continue;
                }
                if (ints.Contains("" + tb.Text.ToCharArray()[i]))
                {
                    res += tb.Text.ToCharArray()[i];
                }
                else
                    f = true;
            }
            int pos = tb.SelectionStart; //changing text and placing cursor in right position
            if (f) pos--;
            tb.Text = res;
            tb.Select(pos, 0);
        }

        private float GetFloat(object sender) //function for parsing floats in textboxes
        {
            if (((TextBox)sender).Text == "-")
            {
                return 0;
            }
            if (((TextBox)sender).Text == ",")
            {
                return 0;
            }
            if (((TextBox)sender).Text.Length == 0)
            {
                return 0;
            }
            FloatSave(sender);
            return float.Parse(((TextBox)sender).Text);
        }

        public void FloatSave(object sender) //function for deleting wrong characters in textboxes while parsing to float
        {
            string[] ints = { "-", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            TextBox tb = (TextBox)sender;
            if (tb.Text[tb.Text.Length - 1] == ',')
                tb.Text = tb.Text.Remove(tb.Text.Length - 1, 1);
            string res = "";
            bool df = false;
            bool f = false;
            if (tb.Text[0] == ',') //if ',' is at start begin with 0
            {
                res = "0";
            }
            for (int i = 0; i < tb.Text.Length; i++)
            {
                if (("" + tb.Text.ToCharArray()[i]) == ints[0] && i != 0)
                {
                    continue;
                }
                if (tb.Text[i] == ',')
                {
                    if (df) //deleting the second ','
                    {
                        f = true;
                    }
                    else
                    {
                        df = true;
                        res += ',';
                    }
                    continue;
                }
                if (ints.Contains("" + tb.Text.ToCharArray()[i]))
                {
                    res = res + tb.Text.ToCharArray()[i];
                }
                else
                    f = true;
            }
            int pos = tb.SelectionStart; //changing text and placing cursor in right position
            if (f) pos--;
            tb.Text = res;
            tb.Select(pos, 0);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            loaded = false;
            LoadCharacterData(mm.GetCharacterData(listBox1.SelectedIndex));
            loaded = true;
        }

        //Changing character data parameters

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            mm.SetSpace(checkBox1.Checked);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetCode(GetInt(sender));
                int last = listBox1.SelectedIndex;
                listBox1.Items.Clear();
                listBox1.Items.AddRange(mm.GetItems().ToArray());
                listBox1.SelectedIndex = last;
                loaded = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetX(GetFloat(sender));
                mm.SelectGlyphOnImage();
                loaded = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetY(GetFloat(sender));
                mm.SelectGlyphOnImage();
                loaded = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetW(GetFloat(sender));
                mm.SelectGlyphOnImage();
                loaded = true;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetH(GetFloat(sender));
                mm.SelectGlyphOnImage();
                loaded = true;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetRM(GetInt(sender));
                loaded = true;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetLM(GetInt(sender));
                loaded = true;
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetXO(GetInt(sender));
                loaded = true;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                loaded = false;
                mm.SetYO(GetInt(sender));
                loaded = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteCharacterData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddCharacterData();
        }

        private void copyCharacterDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyCharacterData();
        }

        private void pasteCharacterDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteCharacterData();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            EditScale((float)trackBar1.Value / 100);
            label15.Text = trackBar1.Value + "%";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
    }
}
