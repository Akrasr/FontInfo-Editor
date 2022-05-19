using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FontInfoEditorGUI
{
    class PictureBoxManager
    {
        public float SCALE = 2; //Scale of an image
        private const int BrushWidth = 2;
        private Color MouseOnColor = Color.Blue;
        private Color MousePressingColor = Color.Yellow;
        private Color ChosenColor = Color.Red;
        PictureBox box;
        Image Atlas;
        List<Label> CursorInfo; //The x, y and code labels
        MainManager mm;
        FontInfo.CharacterData cursoredCD;
        FontInfo.CharacterData chosenCD;
        private int lastdrawn = -1; //The variable used for optimising
        private bool loaded = true;

        public PictureBoxManager(PictureBox pb, string atlasPath, List<Label> ci, MainManager m)
        {
            box = pb;
            Atlas = Image.FromFile(atlasPath);
            CursorInfo = ci;
            box.MouseMove += pictureBox1_MouseMove;
            box.MouseLeave += pictureBox1_MouseOut;
            box.MouseDown += pictureBox1_MousePressed;
            box.MouseUp += pictureBox1_MouseUP;
            mm = m;
        }

        public void DrawAtlas()
        {
            box.Image = GetScaledImage(Atlas);
        }

        public Image GetScaledImage(Image orig) //Scaling Image with a Scale variable
        {
            Bitmap tmp = new Bitmap((int)(orig.Width * SCALE), (int)(orig.Height * SCALE));
            using (Graphics graphics = Graphics.FromImage(tmp))
            {
                graphics.Clear(Color.Black);
                graphics.DrawImage(orig, 0, 0, Atlas.Width * SCALE, Atlas.Height * SCALE);
            }
            return tmp;
        }

        private Image ShowCharacterOnImage(FontInfo.CharacterData cd, Image orig, Color col) //Allocating character with a colored rectangle
        {
            Image tmp = (Image)orig.Clone();
            if (cd != null)
                using (Graphics graphics = Graphics.FromImage(tmp))
                {
                    int x = (int)cd.rx - 1;
                    int w = (int)cd.rw + 2;
                    int y = (int)(mm.GetTextureY() - cd.ry - cd.rh) - 1;
                    int h = (int)cd.rh + 2;
                    graphics.DrawRectangle(new Pen(col, BrushWidth),
                        new Rectangle(x, y, w, h));
                }
            return tmp;
        }

        public void UpdateDraw(Color col) //Allocating cursored and selected characters on atlas
        {
            box.Image.Dispose(); //Disposing image for optimisation
            Bitmap tmp = new Bitmap(Atlas.Width, Atlas.Height);
            using (Graphics graphics = Graphics.FromImage(tmp)) //Drawing atlas on a black background
            {
                graphics.Clear(Color.Black);
                graphics.DrawImage(Atlas, 0, 0, Atlas.Width, Atlas.Height);
            }
            Image res = tmp;
            if (cursoredCD != null)
                res = ShowCharacterOnImage(cursoredCD, res, col);
            if (chosenCD != null)
                res = ShowCharacterOnImage(chosenCD, res, ChosenColor);
            box.Image = GetScaledImage(res);
            res.Dispose(); //Disposing images for optimisation
            tmp.Dispose();
        }

        public void SelectGlyphOnImage(FontInfo.CharacterData cd)
        {
            if (!loaded)
                return;
            if (cd == null)
                return;
            chosenCD = cd;
            UpdateDraw(MouseOnColor);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int xCoordinate = e.X;
            int yCoordinate = e.Y;
            xCoordinate = (int)(xCoordinate / SCALE);
            yCoordinate = (int)(yCoordinate / SCALE);
            cursoredCD = mm.FindGlyph(xCoordinate, yCoordinate); //Getting the character that the cursor is on
            if (cursoredCD != null)
            {
                UpdateCursorInfo("" + (xCoordinate), "" + (mm.GetTextureY() - yCoordinate), " " + cursoredCD.code); //if cursor is on character change text on labels
                if (lastdrawn != cursoredCD.code) //And if the cursored character is not the same as the last one, update image
                {
                    lastdrawn = cursoredCD.code;
                    UpdateDraw(MouseOnColor);
                }
            }
            else
            {
                if (lastdrawn != -1) //if cursor was on the character last time, update Image
                {
                    lastdrawn = -1;
                    UpdateDraw(MouseOnColor);
                }
                UpdateCursorInfo("" + xCoordinate, "" + (mm.GetTextureY() - yCoordinate), ""); //There will be no code
            }
        }

        private void pictureBox1_MouseOut(object sender, EventArgs e)
        {
            UpdateCursorInfo("", "", ""); //the cursor is not at any character
            cursoredCD = null;
            UpdateDraw(MouseOnColor);
        }

        private void pictureBox1_MousePressed(object sender, EventArgs e)
        {

            if (cursoredCD == null)
                return;
            UpdateDraw(MousePressingColor); //while pressing on character it is allocated with yellow rectangle
        }

        private void pictureBox1_MouseUP(object sender, EventArgs e)
        {
            if (cursoredCD == null)
                return;
            chosenCD = cursoredCD; //Selecting a character
            UpdateDraw(MouseOnColor);
            loaded = false; //loaded is used for not loading the same image twice
            mm.SelectGlyphUI(); //selecting it on form list
            loaded = true;
        }

        private void UpdateCursorInfo(string x, string y, string code) //Updating the labels' text
        {
            CursorInfo[0].Text = "X: " + x;
            CursorInfo[1].Text = "Y: " + y;
            CursorInfo[2].Text = "Code: " + code;
        }
    }
}
