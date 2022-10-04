using System.Collections.Generic;
using System.IO;

namespace DuckTales_Remastered_SaveFileManager
{
    public class SAVStream
    {
        public string GameName;
        public uint CRC;
        public CSettings Settings;
        public CKeyboardBinds KeyboardBinds;
        public CGallery Gallery;
        public CStory Story;

        public SAVStream(BinaryReader br)
        {
            GameName = Form1.ReadNullTerminatedString(br, 0x80);

            if (GameName == "DuckTales: Remastered")
            {
                CRC = br.ReadUInt32();

                Settings = new CSettings(br);

                KeyboardBinds = new CKeyboardBinds(br);

                Gallery = new CGallery(br);

                Story = new CStory(br);
            }
            else
            {
                return;
            }
        }

        public class CSettings
        {
            public byte Difficulty;
            public bool IsAntiAliasingOn;
            public byte Language;
            public byte VolumeMusic;
            public byte VolumeSFX;
            public byte BrightnessLevel;
            public bool IsHardPogoOn;
            public bool IsEightBitMusicOn;
            public bool IsExtremeModeAvailable;
            public bool IsEightBitMusicAvailable;

            public CSettings(BinaryReader br)
            {
                br.BaseStream.Seek(0x89, SeekOrigin.Begin);
                this.Difficulty = br.ReadByte();
                br.BaseStream.Seek(0x130, SeekOrigin.Begin);
                this.IsAntiAliasingOn = br.ReadBoolean();
                this.Language = br.ReadByte();
                this.VolumeMusic = br.ReadByte();
                this.VolumeSFX = br.ReadByte();
                this.BrightnessLevel = br.ReadByte();
                br.BaseStream.Seek(0x1, SeekOrigin.Current);
                this.IsHardPogoOn = br.ReadBoolean();
                this.IsEightBitMusicOn = br.ReadBoolean();
                br.BaseStream.Seek(0x1C4, SeekOrigin.Begin);
                this.IsExtremeModeAvailable = br.ReadBoolean();
                this.IsEightBitMusicAvailable = br.ReadBoolean();
            }
        }

        public class CKeyboardBinds
        {

            public byte ESC;
            public byte ENTER;
            public byte DOWN;
            public byte JUMP;
            public byte LEFT;
            public byte ATTACK;
            public byte C;
            public byte RIGHT;
            public byte UP;

            public CKeyboardBinds(BinaryReader br)
            {
                br.BaseStream.Seek(0x1AA, SeekOrigin.Begin);
                ESC = br.ReadByte();
                ENTER = br.ReadByte();
                br.BaseStream.Seek(0x3, SeekOrigin.Current);
                DOWN = br.ReadByte();
                br.BaseStream.Seek(0x4, SeekOrigin.Current);
                JUMP = br.ReadByte();
                LEFT = br.ReadByte();
                br.BaseStream.Seek(0x2, SeekOrigin.Current);
                ATTACK = br.ReadByte();
                br.BaseStream.Seek(0x1, SeekOrigin.Current);
                C = br.ReadByte();
                br.BaseStream.Seek(0x6, SeekOrigin.Current);
                RIGHT = br.ReadByte();
                br.BaseStream.Seek(0x1, SeekOrigin.Current);
                UP = br.ReadByte();
            }
        }

        public class CGallery
        {
            public int Coins;
            public int CoinsSpent;
            public List<CGallerySection> GallerySectionsData = new List<CGallerySection>();

            public CGallery(BinaryReader br)
            {
                br.BaseStream.Seek(0x1C8, SeekOrigin.Begin);
                Coins = br.ReadInt32();
                CoinsSpent = br.ReadInt32();

                for (int i = 0; i < 8; i++)
                {
                    GallerySectionsData.Add(new CGallerySection(br, i));
                }
            }
        }

        public class CGallerySection
        {
            public bool IsComplete = false;
            public byte Flag1;
            public byte Flag2;
            public byte Flag3;
            public byte Flag4;

            public CGallerySection(BinaryReader br, int i)
            {
                Flag1 = br.ReadByte();
                Flag2 = br.ReadByte();
                Flag3 = br.ReadByte();
                Flag4 = br.ReadByte();

                if (i < 2 || i > 3)
                {
                    if (Flag1 != 0xFF || Flag2 != 0xFF || Flag3 != 0x0F)
                    {
                        return;
                    }
                }
                else if (i == 2)
                {
                    if (Flag1 != 0xFF || Flag2 != 0xFF || Flag3 != 0x01)
                    {
                        return;
                    }
                }
                else //if (i == 3)
                {
                    if (Flag1 != 0xFF || Flag2 != 0x7F)
                    {
                        return;
                    }
                }

                IsComplete = true;
            }
        }

        public class CStory
        {
            public uint PlayTimeInSeconds;
            public CStoryFlags1 StoryFlags1;

            public CStory(BinaryReader br)
            {
                br.BaseStream.Seek(0x90, SeekOrigin.Begin);
                StoryFlags1 = new CStoryFlags1(br);
                br.BaseStream.Seek(0x200, SeekOrigin.Begin);
                PlayTimeInSeconds = br.ReadUInt32();
            }
        }

        public class CStoryFlags1
        {
            public byte Flags1;
            public bool IsContinueAvailable;

            public CStoryFlags1(BinaryReader br)
            {
                Flags1 = br.ReadByte();
                IsContinueAvailable = (Flags1 & 0x01) == 1;
            }
        }
    }
}
