using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FontInfoEditorGUI
{
    class FontInfo
    {
        private byte[] header; //Unity Header data
        public float TextureSizeX; //Atlas width
        public float TextureSizeY; //Atlas height
        public int CharacterNum; //is not used anywhere
        public int CharacterSize; //used in AJ's spacing
        public int length; //list length
        public CharacterData[] list;
        public static int indexOfSelected; //index of last selected character data

        public FontInfo() { }
        public FontInfo(byte[] data) 
        {
            ReadData(data);
        }
        public FontInfo(string path) 
        {
            ReadData(File.ReadAllBytes(path));
        }

        public class CharacterData
        {
            public int code, rm, lm, xo, yo;
            public float rx, ry, rw, rh;
            public bool isSpace;

            public string toString()
            {
                string st = "[Code]" + code + "[RectTransform:[x]" + rx +
                    "[y]" + ry + "[w]" + rw + "[h]" + rh + "][righttMargin]" + rm +
                    "[leftMargin]" + lm + "[xOffset]" + xo + "[yOffset]" + yo +
                    "[isSpace]" + (isSpace ? "true" : "false");
                return st;
            }

            public void ReadData(BinaryReader br)
            {
                this.code = br.ReadInt32();
                this.rx = br.ReadSingle();
                this.ry = br.ReadSingle();
                this.rw = br.ReadSingle();
                this.rh = br.ReadSingle();
                this.rm = br.ReadInt32();
                this.lm = br.ReadInt32();
                this.xo = br.ReadInt32();
                this.yo = br.ReadInt32();
                if (br.ReadInt32() == 1)
                {
                    this.isSpace = true;
                }
                else
                    this.isSpace = false;
            }

            public void WriteData(BinaryWriter br)
            {
                br.Write(this.code);
                br.Write(this.rx);
                br.Write(this.ry);
                br.Write(this.rw);
                br.Write(this.rh);
                br.Write(this.rm);
                br.Write(this.lm);
                br.Write(this.xo);
                br.Write(this.yo);
                int x = 0;
                if (this.isSpace)
                {
                    x = 1;
                }
                br.Write(x);
            }

            public static CharacterData CreateEmpty()
            {
                CharacterData cd = new CharacterData();
                cd.code = 0;
                cd.rx = 0;
                cd.ry = 0;
                cd.rw = 0;
                cd.rh = 0;
                cd.rm = 0;
                cd.lm = 0;
                cd.xo = 0;
                cd.yo = 0;
                cd.isSpace = false;
                return cd;
            }
        }

        public void ReadData(BinaryReader br) // Reading font data
        {
            this.TextureSizeX = br.ReadSingle();
            this.TextureSizeY = br.ReadSingle();
            this.CharacterNum = br.ReadInt32();
            this.CharacterSize = br.ReadInt32();
            this.length = br.ReadInt32();
            list = new CharacterData[length];
            for (int i = 0; i < length; i++)
            {
                CharacterData cd = new CharacterData();
                cd.ReadData(br);
                list[i] = cd;
            }
        }

        void ReadData(byte[] data) //Unity Data Header Reading
        {
            header = new byte[44];
            for (int i = 0; i < 44; i++)
            {
                header[i] = data[i];
            }
            if (header[40] != 111 || header[39] != 102 || header[38] != 110 || header[37] != 105 || header[36] != 95 || header[35] != 116 || header[34] != 110 || header[33] != 111 || header[32] != 102)
            {
                throw new Exception();  //If monobehaviour file's name is not font_info exception is being thrown
            }
            byte[] nd = new byte[data.Length - 44];
            for (int i = 44; i < data.Length; i++) //The font info data is being written in another array
            {
                nd[i - 44] = data[i];
            };
            using (BinaryReader br = new BinaryReader(new MemoryStream(nd)))
                ReadData(br);
        }

        public byte[] WriteData() //writing font data in byte array
        {
            byte[] data = new byte[64 + list.Length * 40];
            for (int i = 0; i < 44; i++)
            {
                data[i] = header[i];
            }
            byte[] nd = new byte[data.Length - 44];
            using (BinaryWriter br = new BinaryWriter(new MemoryStream(nd)))
            {
                br.Write(TextureSizeX);
                br.Write(TextureSizeY);
                br.Write(CharacterNum);
                br.Write(CharacterSize);
                br.Write(length);
                for (int i = 0; i < length; i++)
                {
                    list[i].WriteData(br);
                }
            }
            for (int i = 44; i < data.Length; i++)
            {
                data[i] = nd[i - 44];
            }
            return data;
        }

        public List<string> GetCodes() //Getting codes of characters
        {
            List<string> res = new List<string>();
            for (int i = 0; i < list.Length; i++) 
            {
                res.Add("" + list[i].code);
            }
            return res;
        }

        public void AddEmpty()
        {
            length++;
            List<CharacterData> dat = list.ToList<CharacterData>();
            dat.Add(CharacterData.CreateEmpty());
            list = dat.ToArray();
        }

        public CharacterData GetCharacterData(int index)
        {
            return list[index];
        }

        public void DeleteData(CharacterData cd)
        {
            length--;
            List<CharacterData> dat = list.ToList<CharacterData>();
            dat.Remove(cd);
            list = dat.ToArray();
        }

        public CharacterData FindGlyph(int x, int y) //finding the first glyph that has this point in its area
        {
            y = (int)TextureSizeY - y;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].rx < x && x - list[i].rx < list[i].rw && list[i].ry < y && y - list[i].ry < list[i].rh)
                {
                    indexOfSelected = i;
                    return list[i];
                }
            }
            return null;
        }
    }
}
