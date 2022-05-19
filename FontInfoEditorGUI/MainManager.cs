using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace FontInfoEditorGUI
{
    class MainManager
    {
        FontInfo font_info;
        public FontInfo.CharacterData CurrentCD;
        string InfoPath;
        PictureBoxManager pbm;
        public Form1 form;

        public MainManager(string infoPath, string atlasPath, PictureBox pb, List<Label> lb, Form1 f)
        {
            Load(infoPath, atlasPath, pb, lb, f);
        }

        public void Load(string infoPath, string atlasPath, PictureBox pb, List<Label> lb, Form1 f)
        {
            InfoPath = infoPath;
            font_info = new FontInfo(infoPath);
            pbm = new PictureBoxManager(pb, atlasPath, lb, this);
            form = f;
        }

        public List<string> GetItems() //Getting codes of characters
        {
            return font_info.GetCodes();
        }

        public FontInfo.CharacterData GetCharacterData(int index) //Getting character data with this index in font_info's list
        {
            CurrentCD = font_info.GetCharacterData(index);
            return this.CurrentCD;
        }

        //Setting selected characterdata params

        public void SetCode(int code)
        {
            CurrentCD.code = code;
        }

        public void SetX(float val)
        {
            CurrentCD.rx = val;
        }

        public void SetY(float val)
        {
            CurrentCD.ry = val;
        }

        public void SetW(float val)
        {
            CurrentCD.rw = val;
        }

        public void SetH(float val)
        {
            CurrentCD.rh = val;
        }

        public void SetRM(int val)
        {
            CurrentCD.rm = val;
        }

        public void SetLM(int val)
        {
            CurrentCD.lm = val;
        }

        public void SetXO(int val)
        {
            CurrentCD.xo = val;
        }

        public void SetYO(int val)
        {
            CurrentCD.yo = val;
        }

        public void SetSpace(bool val)
        {
            CurrentCD.isSpace = val;
        }

        //saving font_info

        public void SaveAs(string newPath) 
        {
            InfoPath = newPath;
            Save();
        }

        public void Save()
        {
            File.WriteAllBytes(InfoPath, font_info.WriteData());
        }

        public void DeleteCurrentCD() //deleting selected character data
        {
            font_info.DeleteData(CurrentCD);
            CurrentCD = null;
            pbm.SelectGlyphOnImage(null);
        }

        public void AddData()
        {
            font_info.AddEmpty();
        }

        public Size GetAtlasSize()
        {
            return new Size((int)(font_info.TextureSizeX * pbm.SCALE), (int)(font_info.TextureSizeY * pbm.SCALE));
        }

        public void ShowAtlas()
        {
            pbm.DrawAtlas();
        }

        public FontInfo.CharacterData FindGlyph(int x, int y) //finding glyph that has this point in its area
        {
            return font_info.FindGlyph(x, y);
        }

        public float GetTextureY()
        {
            return font_info.TextureSizeY;
        }

        public float GetTextureX()
        {
            return font_info.TextureSizeX;
        }

        public void SelectGlyphUI()
        {
            form.SelectCharacter(FontInfo.indexOfSelected);
        }

        public void SelectGlyphOnImage()
        {
            pbm.SelectGlyphOnImage(CurrentCD);
        }

        public void SetScale(float val)
        {
            pbm.SCALE = val;
        }
    }
}
