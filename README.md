# FontInfo-Editor
Tool for editing font data in message windows of AJ, AAI and AAI2 mobile ports

# How does font work?
The font of message window is saved as serialised FontInfo object file.<br>
You can see its structure and how it works in this video: https://www.youtube.com/watch?v=7NSOCuuXff8

# Where and how can I get a font-info file?
It is saved in one of assets files in cache obb file.<br>
AJ: e62c942efa54d4497bdd56076b699760<br>
AAI: e62c942efa54d4497bdd56076b699760<br>
AAI2: 79e6b2596cfcb1f4082a79fa4f9d7bd5<br>
Extract monobehaviour saved inside of this assets file with UnityEX or Assets Bundle Extractor.

# How to use this tool?
1. Open extracted font-info monobehaviour file and font atlas image.
2. Choose the characters you want to edit in list box or in the image box of atlas and edit them.
3. Save it or save as some other file.

# How does spacing work in these ports?
Every glyph is spaced after the length of last glyph in each game. The length of the glyph is saved in:<br>
AJ: as number that equals font_info.CharacterSize - characterData.RightMargin - characterData.LeftMargin<br>
AAI: as 28 or a number from gyakuten_width_us array that contains lengths for every character from first 128<br>
AAI2: as the Width(W) of the character<br>
GK2: as 28.

# How can I add new characters support into the game?
AJ: Just make sure that new characters don't have the same codes as some other characters.<br>
AAI: Make sure that new characters don't have the same codes as some other characters and rewrite the game's code so it can get right codes to the bytes in the game's script.<br>
AAI2: Just make sure that new characters don't have the same codes as some other characters (Recomendation: use Unicode codes)

# What are these codes of the characters?
AJ: It is the number of their first appearance in the script. 2 is the first number found in the script so its code is 0, 0 is the second so it's 1 and etc.<br>
AAI: It's jis_codes, but since AAI script is saved as bytes, all the bytes are being replaced with jis_codes;<br>
AAI2: It's Unicode.

# This tool is able to edit only font info. How can I edit the font atlas?
Use Photoshop or Paint Tool Sai or whatever you use for graphics editing.

# What is this font's ttf analog?
This is the FOT-UDMarugo_Large Pro M. The font size is 46.
