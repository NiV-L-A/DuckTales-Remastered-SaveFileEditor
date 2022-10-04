using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Diagnostics;

namespace DuckTales_Remastered_SaveFileManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TxtFilePath.Visible = false;
            LblFilePath.Visible = false;
            BtnCopyFilePath.Visible = false;
            tabControl1.Visible = false;
            ChkEdit.Visible = false;
            LblReloadFile.Visible = false;
            BtnReloadFile.Visible = false;
            BtnSave.Visible = false;
            BtnExit.Visible = false;
            LinkLblGithub.Visible = false;
            CmbDiff.BringToFront();
            CmbLang.BringToFront();
            TxtMusicVol.BringToFront();
            TxtSFXVol.BringToFront();
            TxtBright.BringToFront();

            GrpKeyboard.Controls.OfType<ComboBox>().ToList().ForEach(c => c.SelectionChangeCommitted += C_CheckedChanged);
        }

        SAVStream SAVFile = null;
        string SAVFilePath = "";

        private void label1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "DuckTales: Remastered Save File|*.sav|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SAVFilePath = ofd.FileName;
                TxtFilePath.Text = SAVFilePath;

                FileStream fs = new FileStream(SAVFilePath, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                SAVFile = new SAVStream(br);

                fs.Dispose();
                fs.Close();

                if (SAVFile.GameName != "DuckTales: Remastered")
                {
                    MessageBox.Show("Incorrect file loaded!", "Loading Save File - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

                LblIntro.Visible = false;
                TxtFilePath.Visible = true;
                LblFilePath.Visible = true;
                BtnCopyFilePath.Visible = true;
                tabControl1.Visible = true;
                ChkEdit.Visible = true;
                LblReloadFile.Visible = true;
                BtnReloadFile.Visible = true;
                BtnSave.Visible = true;
                BtnExit.Visible = true;
                LinkLblGithub.Visible = true;
                LoadFromFile();
            }
        }

        private void LoadFromFile()
        {
            LoadFromFileSettings();
            LoadFromFileBinds();
            LoadFromFileGallery();
            LoadFromFileStory();
        }

        private void LoadFromFileSettings()
        {
            ChkAntiAli.Checked = SAVFile.Settings.IsAntiAliasingOn;
            ChkIs8BitMusicAvailable.Checked = SAVFile.Settings.IsEightBitMusicAvailable;
            ChkIs8BitMusicOn.Checked = SAVFile.Settings.IsEightBitMusicOn;
            ChkIsExtModeAvailable.Checked = SAVFile.Settings.IsExtremeModeAvailable;
            ChkHardPogo.Checked = SAVFile.Settings.IsHardPogoOn;

            CmbDiff.SelectedIndex = SAVFile.Settings.Difficulty;
            CmbLang.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(Language)), (Language)SAVFile.Settings.Language);
            TxtMusicVol.Text = SAVFile.Settings.VolumeMusic.ToString();
            TxtSFXVol.Text = SAVFile.Settings.VolumeSFX.ToString();
            TxtBright.Text = SAVFile.Settings.BrightnessLevel.ToString();

            LblCRC.Text = $"CRC's Residue Constant: 0x{SAVFile.CRC.ToString("X8")}";
        }

        private void LoadFromFileBinds()
        {
            CmbESC.DataSource = Enum.GetValues(typeof(Keys));
            CmbENTER.DataSource = Enum.GetValues(typeof(Keys));
            CmbDOWN.DataSource = Enum.GetValues(typeof(Keys));
            CmbJUMP.DataSource = Enum.GetValues(typeof(Keys));
            CmbLEFT.DataSource = Enum.GetValues(typeof(Keys));
            CmbATTACK.DataSource = Enum.GetValues(typeof(Keys));
            CmbC.DataSource = Enum.GetValues(typeof(Keys));
            CmbRIGHT.DataSource = Enum.GetValues(typeof(Keys));
            CmbUP.DataSource = Enum.GetValues(typeof(Keys));

            CmbESC.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.ESC);
            CmbENTER.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.ENTER);
            CmbDOWN.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.DOWN);
            CmbJUMP.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.JUMP);
            CmbLEFT.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.LEFT);
            CmbATTACK.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.ATTACK);
            CmbC.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.C);
            CmbRIGHT.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.RIGHT);
            CmbUP.SelectedItem = ConvertByteToKey(SAVFile.KeyboardBinds.UP);

            LblESCCode.Text = "0x" + ConvertKeyToByte((Key)CmbESC.SelectedItem).ToString("X2");
            LblENTERCode.Text = "0x" + ConvertKeyToByte((Key)CmbENTER.SelectedItem).ToString("X2");
            LblDOWNCode.Text = "0x" + ConvertKeyToByte((Key)CmbDOWN.SelectedItem).ToString("X2");
            LblJUMPCode.Text = "0x" + ConvertKeyToByte((Key)CmbJUMP.SelectedItem).ToString("X2");
            LblLEFTCode.Text = "0x" + ConvertKeyToByte((Key)CmbLEFT.SelectedItem).ToString("X2");
            LblATTACKCode.Text = "0x" + ConvertKeyToByte((Key)CmbATTACK.SelectedItem).ToString("X2");
            LblCCode.Text = "0x" + ConvertKeyToByte((Key)CmbC.SelectedItem).ToString("X2");
            LblRIGHTCode.Text = "0x" + ConvertKeyToByte((Key)CmbRIGHT.SelectedItem).ToString("X2");
            LblUPCode.Text = "0x" + ConvertKeyToByte((Key)CmbUP.SelectedItem).ToString("X2");
        }

        private void LoadFromFileGallery()
        {
            if (SAVFile.Gallery.CoinsSpent >= 32000000)
            {
                ChkUnlockSecs.Checked = true;
            }
            else
            {
                ChkUnlockSecs.CheckState = CheckState.Indeterminate;
            }

            TxtCoins.Text = SAVFile.Gallery.Coins.ToString();

            for (int i = 0; i < 8; i++)
            {
                if (SAVFile.Gallery.GallerySectionsData[i].IsComplete)
                {
                    switch (i)
                    {
                        case 0:
                            ChkChars.Checked = true;
                            break;
                        case 1:
                            ChkArt.Checked = true;
                            break;
                        case 2:
                            ChkSk.Checked = true;
                            break;
                        case 3:
                            ChkPencil.Checked = true;
                            break;
                        case 4:
                            ChkPaints.Checked = true;
                            break;
                        case 5:
                            ChkMusic.Checked = true;
                            break;
                        case 6:
                            ChkTV1.Checked = true;
                            break;
                        case 7:
                            ChkTV2.Checked = true;
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            ChkChars.CheckState = CheckState.Indeterminate;
                            break;
                        case 1:
                            ChkArt.CheckState = CheckState.Indeterminate;
                            break;
                        case 2:
                            ChkSk.CheckState = CheckState.Indeterminate;
                            break;
                        case 3:
                            ChkPencil.CheckState = CheckState.Indeterminate;
                            break;
                        case 4:
                            ChkPaints.CheckState = CheckState.Indeterminate;
                            break;
                        case 5:
                            ChkMusic.CheckState = CheckState.Indeterminate;
                            break;
                        case 6:
                            ChkTV1.CheckState = CheckState.Indeterminate;
                            break;
                        case 7:
                            ChkTV2.CheckState = CheckState.Indeterminate;
                            break;
                    }
                }
            }
        }

        private void LoadFromFileStory()
        {
            TimeSpan time = TimeSpan.FromSeconds(SAVFile.Story.PlayTimeInSeconds);
            TxtHours.Text = ((int)time.TotalHours).ToString();
            TxtMins.Text = time.Minutes.ToString();
            TxtSeconds.Text = time.Seconds.ToString();
            TxtPlayTimeSeconds.Text = time.TotalSeconds.ToString();

            ChkContinueAvailable.Checked = SAVFile.Story.StoryFlags1.IsContinueAvailable;
        }

        private Keys ConvertByteToKey(byte Value)
        {
            return (Keys)KeyInterop.VirtualKeyFromKey(KeyInterop.KeyFromVirtualKey(Value));
        }

        private byte ConvertKeyToByte(Key Value)
        {
            return (byte)KeyInterop.KeyFromVirtualKey(KeyInterop.VirtualKeyFromKey(Value));
        }

        private void ModifyFont()
        {
            TxtHours.Font = new Font(TxtHours.Font, FontStyle.Bold);
            TxtMins.Font = new Font(TxtHours.Font, FontStyle.Bold);
            TxtSeconds.Font = new Font(TxtHours.Font, FontStyle.Bold);
            LblHours.Font = new Font(LblHours.Font, FontStyle.Bold);
            LblMins.Font = new Font(LblHours.Font, FontStyle.Bold);
            LblSeconds.Font = new Font(LblHours.Font, FontStyle.Bold);
            TxtPlayTimeSeconds.Font = new Font(TxtPlayTimeSeconds.Font, FontStyle.Regular);
            LblInSeconds.Font = new Font(LblInSeconds.Font, FontStyle.Regular);
        }

        private void HMSToSeconds()
        {
            if (GrpPlay.Enabled)
            {
                try
                {
                    TimeSpan time = TimeSpan.FromHours(Convert.ToDouble(TxtHours.Text));
                    time = time.Add(TimeSpan.FromMinutes(Convert.ToDouble(TxtMins.Text)));
                    time = time.Add(TimeSpan.FromSeconds(Convert.ToDouble(TxtSeconds.Text)));
                    TxtPlayTimeSeconds.Text = time.TotalSeconds.ToString();
                }
                catch (OverflowException)
                {
                    TxtHours.Text = "0";
                    TxtMins.Text = "0";
                    TxtSeconds.Text = "0";
                }
            }
        }

        private void SecondsToHMS()
        {
            if (GrpPlay.Enabled)
            {
                try
                {
                    TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(TxtPlayTimeSeconds.Text));
                    TxtHours.Text = ((int)time.TotalHours).ToString();
                    TxtMins.Text = time.Minutes.ToString();
                    TxtSeconds.Text = time.Seconds.ToString();
                }
                catch (OverflowException)
                {
                    TxtPlayTimeSeconds.Text = "0";
                }
            }
        }

        private void SetTxtOnlyNumbers(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != 0x2D)
            {
                e.Handled = true;
            }
        }

        private void SetTxtMaxN(object sender, uint Max)
        {
            if (!int.TryParse((sender as TextBox).Text, out _) && (sender as TextBox).Text != "-")
            {
                (sender as TextBox).Text = "0";
            }
            else if ((sender as TextBox).Text != "" && (sender as TextBox).Text != "-")
            {
                if (Convert.ToInt32((sender as TextBox).Text) > Max)
                {
                    (sender as TextBox).Text = "0";
                }
            }
            else if ((sender as TextBox).Text != "-")
            {
                (sender as TextBox).Text = "0";
            }
        }

        public static string ReadNullTerminatedString(BinaryReader stream, uint MaxLength)
        {
            string str = "";
            char ch;
            while ((ch = (char)stream.PeekChar()) != 0 && (str.Length < MaxLength))
            {
                ch = stream.ReadChar();
                str += ch;
            }
            stream.BaseStream.Seek(MaxLength - str.Length, SeekOrigin.Current);
            return str;
        }

        private void BtnCopyFilePath_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TxtFilePath.Text);
        }

        private void ChkEdit_CheckedChanged(object sender, EventArgs e)
        {
            LblCRC.Enabled = ChkEdit.Checked;
            LblClkToEdit.Visible = ChkEdit.Checked;

            GrpBools.Enabled = ChkEdit.Checked;
            GrpOptions.Enabled = ChkEdit.Checked;

            GrpKeyboard.Enabled = ChkEdit.Checked;
            //GrpXbox.Enabled = CheckBoxEdit.Checked;

            GrpVars.Enabled = ChkEdit.Checked;
            GrpSecs.Enabled = ChkEdit.Checked;

            GrpPlay.Enabled = ChkEdit.Checked;

            GrpStoryBools.Enabled = ChkEdit.Checked;
        }

        private void LblCRC_DoubleClick(object sender, EventArgs e)
        {
            using (CRCEditForm f2 = new CRCEditForm(SAVFile))
            {
                if (f2.ShowDialog() == DialogResult.OK)
                {
                    LblCRC.Text = $"CRC's Residue Constant: 0x{SAVFile.CRC.ToString("X8")} -> 0x{f2.NewCRC.ToString("X8")}";
                }
            }
        }

        private void C_CheckedChanged(object sender, EventArgs e)
        {
            var c = (ComboBox)sender;
            string value = "0x" + ConvertKeyToByte((Key)c.SelectedItem).ToString("X2");

            switch (c.Name)
            {
                case "CmbUP":
                    LblUPCode.Text = value;
                    break;
                case "CmbLEFT":
                    LblLEFTCode.Text = value;
                    break;
                case "CmbRIGHT":
                    LblRIGHTCode.Text = value;
                    break;
                case "CmbDOWN":
                    LblDOWNCode.Text = value;
                    break;
                case "CmbJUMP":
                    LblJUMPCode.Text = value;
                    break;
                case "CmbATTACK":
                    LblATTACKCode.Text = value;
                    break;
                case "CmbC":
                    LblCCode.Text = value;
                    break;
                case "CmbENTER":
                    LblENTERCode.Text = value;
                    break;
                case "CmbESC":
                    LblESCCode.Text = value;
                    break;
            }
        }

        private void TxtPlayTimeSeconds_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetTxtOnlyNumbers(e);
        }

        private void TxtPlayTimeSeconds_TextChanged(object sender, EventArgs e)
        {
            if (!uint.TryParse(TxtPlayTimeSeconds.Text, out uint res) || TxtPlayTimeSeconds.Text.Substring(0, 1) == "-" || TxtPlayTimeSeconds.Text == "")
            {
                TxtPlayTimeSeconds.Text = "0";
                TxtHours.Text = "0";
                TxtMins.Text = "0";
                TxtSeconds.Text = "0";
                return;
            }

            if (TxtPlayTimeSeconds.Font.Bold)
            {
                SecondsToHMS();
            }
        }

        private void TxtPlayTimeSeconds_MouseDown(object sender, MouseEventArgs e)
        {
            TxtPlayTimeSeconds.Font = new Font(TxtPlayTimeSeconds.Font, FontStyle.Bold);
            LblInSeconds.Font = new Font(LblInSeconds.Font, FontStyle.Bold);
            TxtHours.Font = new Font(TxtHours.Font, FontStyle.Regular);
            TxtMins.Font = new Font(TxtHours.Font, FontStyle.Regular);
            TxtSeconds.Font = new Font(TxtHours.Font, FontStyle.Regular);
            LblHours.Font = new Font(LblHours.Font, FontStyle.Regular);
            LblMins.Font = new Font(LblHours.Font, FontStyle.Regular);
            LblSeconds.Font = new Font(LblHours.Font, FontStyle.Regular);
        }

        private void BtnReloadFile_Click(object sender, EventArgs e)
        {
            LoadFromFile();
        }
        private void BtnReloadTab1_Click(object sender, EventArgs e)
        {
            LoadFromFileSettings();
        }
        private void BtnReloadTab2_Click(object sender, EventArgs e)
        {
            LoadFromFileBinds();
        }
        private void BtnReloadTab3_Click(object sender, EventArgs e)
        {
            LoadFromFileGallery();
        }
        private void BtnReloadTab4_Click(object sender, EventArgs e)
        {
            LoadFromFileStory();
        }

        private void TxtHours_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetTxtOnlyNumbers(e);
        }
        private void TxtHours_MouseDown(object sender, MouseEventArgs e)
        {
            ModifyFont();
        }
        private void TxtHours_TextChanged(object sender, EventArgs e)
        {
            if (TxtHours.Text == "" || TxtHours.Text.Contains("-"))
            {
                TxtHours.Text = "0";
                return;
            }
            if (TxtHours.Font.Bold)
            {
                HMSToSeconds();
            }
        }

        private void TxtMins_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetTxtOnlyNumbers(e);
        }
        private void TxtMins_MouseDown(object sender, MouseEventArgs e)
        {
            ModifyFont();
        }
        private void TxtMins_TextChanged(object sender, EventArgs e)
        {
            if (TxtMins.Text == "" || TxtMins.Text.Contains("-"))
            {
                TxtMins.Text = "0";
                return;
            }
            if (TxtMins.Font.Bold)
            {
                HMSToSeconds();
            }
        }

        private void TxtSeconds_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetTxtOnlyNumbers(e);
        }
        private void TxtSeconds_MouseDown(object sender, MouseEventArgs e)
        {
            ModifyFont();
        }
        private void TxtSeconds_TextChanged(object sender, EventArgs e)
        {
            if (TxtSeconds.Text == "" || TxtSeconds.Text.Contains("-"))
            {
                TxtSeconds.Text = "0";
                return;
            }
            if (TxtSeconds.Font.Bold)
            {
                HMSToSeconds();
            }
        }

        private void TxtMusicVol_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetTxtOnlyNumbers(e);
        }
        private void TxtMusicVol_TextChanged(object sender, EventArgs e)
        {
            SetTxtMaxN(sender, 255);
            if (TxtMusicVol.Text.Substring(0, 1) == "-")
            {
                TxtMusicVol.Text = "0";
                return;
            }
        }

        private void TxtSFXVol_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetTxtOnlyNumbers(e);
        }
        private void TxtSFXVol_TextChanged(object sender, EventArgs e)
        {
            SetTxtMaxN(sender, 255);
            if (TxtSFXVol.Text.Substring(0, 1) == "-")
            {
                TxtSFXVol.Text = "0";
                return;
            }
        }

        private void TxtBright_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetTxtOnlyNumbers(e);
        }
        private void TxtBright_TextChanged(object sender, EventArgs e)
        {
            SetTxtMaxN(sender, 255);
            if (TxtBright.Text.Substring(0, 1) == "-")
            {
                TxtBright.Text = "0";
                return;
            }
        }

        private void TxtCoins_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetTxtOnlyNumbers(e);
        }
        private void TxtCoins_TextChanged(object sender, EventArgs e)
        {
            SetTxtMaxN(sender, int.MaxValue);
        }

        private void ChkHardPogo_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkHardPogo.Checked && CmbDiff.SelectedIndex == 0)
            {
                LblHardPogoWarning.Visible = true;
            }
            else
            {
                LblHardPogoWarning.Visible = false;
            }
        }
        private void CmbDiff_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChkHardPogo.Checked && CmbDiff.SelectedIndex == 0)
            {
                LblHardPogoWarning.Visible = true;
            }
            else
            {
                LblHardPogoWarning.Visible = false;
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close the program?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "DuckTales: Remastered Save File|*.sav|All files (*.*)|*.*";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string NewSAVFilePath = sfd.FileName;
                if (NewSAVFilePath != SAVFilePath)
                {
                    File.Copy(SAVFilePath, NewSAVFilePath, true);
                }

                FileStream fs = new FileStream(NewSAVFilePath, FileMode.Open);
                BinaryWriter bw = new BinaryWriter(fs);

                uint newCRC = Convert.ToUInt32(LblCRC.Text.Substring(LblCRC.Text.Length - 10, 10), 16);
                if (newCRC != SAVFile.CRC)
                {
                    bw.BaseStream.Seek(0x80, SeekOrigin.Begin);
                    bw.Write(newCRC);
                }

                if (CmbDiff.SelectedIndex != SAVFile.Settings.Difficulty)
                {
                    bw.BaseStream.Seek(0x89, SeekOrigin.Begin);
                    bw.Write((byte)CmbDiff.SelectedIndex);
                }

                if (ChkContinueAvailable.Checked != SAVFile.Story.StoryFlags1.IsContinueAvailable)
                {
                    bw.BaseStream.Seek(0x90, SeekOrigin.Begin);
                    bw.Write((byte)((SAVFile.Story.StoryFlags1.IsContinueAvailable == true) ? SAVFile.Story.StoryFlags1.Flags1 - 1 : SAVFile.Story.StoryFlags1.Flags1 + 1));
                }

                if (ChkAntiAli.Checked != SAVFile.Settings.IsAntiAliasingOn)
                {
                    bw.BaseStream.Seek(0x130, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(ChkAntiAli.Checked));
                }

                if (CmbLang.SelectedIndex != Array.IndexOf(Enum.GetValues(typeof(Language)), (Language)SAVFile.Settings.Language))
                {
                    bw.BaseStream.Seek(0x131, SeekOrigin.Begin);
                    switch(CmbLang.SelectedIndex) //Maybe better to use the enum
                    {
                        case 0:
                            bw.Write(Convert.ToByte(0));
                            break;
                        case 1:
                            bw.Write(Convert.ToByte(2));
                            break;
                        case 2:
                            bw.Write(Convert.ToByte(3));
                            break;
                        case 3:
                            bw.Write(Convert.ToByte(4));
                            break;
                        case 4:
                            bw.Write(Convert.ToByte(5));
                            break;
                        case 5:
                            bw.Write(Convert.ToByte(9));
                            break;
                    }
                }

                if (Convert.ToByte(TxtMusicVol.Text) != SAVFile.Settings.VolumeMusic)
                {
                    bw.BaseStream.Seek(0x132, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(TxtMusicVol.Text));
                }

                if (Convert.ToByte(TxtSFXVol.Text) != SAVFile.Settings.VolumeSFX)
                {
                    bw.BaseStream.Seek(0x133, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(TxtSFXVol.Text));
                }

                if (Convert.ToByte(TxtBright.Text) != SAVFile.Settings.BrightnessLevel)
                {
                    bw.BaseStream.Seek(0x134, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(TxtBright.Text));
                }

                if (ChkHardPogo.Checked != SAVFile.Settings.IsHardPogoOn)
                {
                    bw.BaseStream.Seek(0x136, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(ChkHardPogo.Checked));
                }

                if (ChkIs8BitMusicOn.Checked != SAVFile.Settings.IsEightBitMusicOn)
                {
                    bw.BaseStream.Seek(0x137, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(ChkIs8BitMusicOn.Checked));
                }


                //*********************************************
                //******************* BINDS *******************
                //*********************************************

                if (Convert.ToByte(LblESCCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.ESC)
                {
                    bw.BaseStream.Seek(0x1AA, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblESCCode.Text.Substring(2, 2), 16));
                }

                if (Convert.ToByte(LblENTERCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.ENTER)
                {
                    bw.BaseStream.Seek(0x1AB, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblENTERCode.Text.Substring(2, 2), 16));
                }

                if (Convert.ToByte(LblDOWNCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.DOWN)
                {
                    bw.BaseStream.Seek(0x1AF, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblDOWNCode.Text.Substring(2, 2), 16));
                }

                if (Convert.ToByte(LblJUMPCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.JUMP)
                {
                    bw.BaseStream.Seek(0x1B4, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblJUMPCode.Text.Substring(2, 2), 16));
                }

                if (Convert.ToByte(LblLEFTCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.LEFT)
                {
                    bw.BaseStream.Seek(0x1B5, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblLEFTCode.Text.Substring(2, 2), 16));
                }

                if (Convert.ToByte(LblATTACKCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.ATTACK)
                {
                    bw.BaseStream.Seek(0x1B8, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblATTACKCode.Text.Substring(2, 2), 16));
                }

                if (Convert.ToByte(LblCCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.C)
                {
                    bw.BaseStream.Seek(0x1BA, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblCCode.Text.Substring(2, 2), 16));
                }

                if (Convert.ToByte(LblRIGHTCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.RIGHT)
                {
                    bw.BaseStream.Seek(0x1C1, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblRIGHTCode.Text.Substring(2, 2), 16));
                }

                if (Convert.ToByte(LblUPCode.Text.Substring(2, 2), 16) != SAVFile.KeyboardBinds.UP)
                {
                    bw.BaseStream.Seek(0x1C3, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(LblUPCode.Text.Substring(2, 2), 16));
                }

                if (ChkIsExtModeAvailable.Checked != SAVFile.Settings.IsExtremeModeAvailable)
                {
                    bw.BaseStream.Seek(0x1C4, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(ChkIsExtModeAvailable.Checked));
                }

                if (ChkIs8BitMusicAvailable.Checked != SAVFile.Settings.IsEightBitMusicAvailable)
                {
                    bw.BaseStream.Seek(0x1C5, SeekOrigin.Begin);
                    bw.Write(Convert.ToByte(ChkIs8BitMusicAvailable.Checked));
                }

                //*********************************************
                //****************** GALLERY ******************
                //*********************************************

                if (Convert.ToInt32(TxtCoins.Text) != SAVFile.Gallery.Coins)
                {
                    bw.BaseStream.Seek(0x1C8, SeekOrigin.Begin);
                    bw.Write(Convert.ToInt32(TxtCoins.Text));
                }

                switch (ChkUnlockSecs.CheckState)
                {
                    case CheckState.Unchecked:
                        bw.BaseStream.Seek(0x1CC, SeekOrigin.Begin);
                        bw.Write(0);
                        break;
                    case CheckState.Checked:
                        if (SAVFile.Gallery.CoinsSpent < 32000000)
                        {
                            bw.BaseStream.Seek(0x1CC, SeekOrigin.Begin);
                            bw.Write(32000000);
                        }
                        break;
                }

                if (ChkChars.CheckState == CheckState.Checked)
                {
                    bw.BaseStream.Seek(0x1D0, SeekOrigin.Begin);
                    bw.Write(0x000FFFFF);
                }
                else if (ChkChars.CheckState == CheckState.Unchecked)
                {
                    bw.BaseStream.Seek(0x1D0, SeekOrigin.Begin);
                    bw.Write(0);
                }

                if (ChkArt.CheckState == CheckState.Checked)
                {
                    bw.BaseStream.Seek(0x1D4, SeekOrigin.Begin);
                    bw.Write(0x000FFFFF);
                }
                else if (ChkArt.CheckState == CheckState.Unchecked)
                {
                    bw.BaseStream.Seek(0x1D4, SeekOrigin.Begin);
                    bw.Write(0);
                }

                if (ChkSk.CheckState == CheckState.Checked)
                {
                    bw.BaseStream.Seek(0x1D8, SeekOrigin.Begin);
                    bw.Write(0x0001FFFF);
                }
                else if (ChkSk.CheckState == CheckState.Unchecked)
                {
                    bw.BaseStream.Seek(0x1D8, SeekOrigin.Begin);
                    bw.Write(0);
                }

                if (ChkPencil.CheckState == CheckState.Checked)
                {
                    bw.BaseStream.Seek(0x1DC, SeekOrigin.Begin);
                    bw.Write(0x00007FFF);
                }
                else if (ChkPencil.CheckState == CheckState.Unchecked)
                {
                    bw.BaseStream.Seek(0x1DC, SeekOrigin.Begin);
                    bw.Write(0);
                }

                if (ChkPaints.CheckState == CheckState.Checked)
                {
                    bw.BaseStream.Seek(0x1E0, SeekOrigin.Begin);
                    bw.Write(0x000FFFFF);
                }
                else if (ChkPaints.CheckState == CheckState.Unchecked)
                {
                    bw.BaseStream.Seek(0x1E0, SeekOrigin.Begin);
                    bw.Write(0);
                }

                if (ChkMusic.CheckState == CheckState.Checked)
                {
                    bw.BaseStream.Seek(0x1E4, SeekOrigin.Begin);
                    bw.Write(0x000FFFFF);
                }
                else if (ChkMusic.CheckState == CheckState.Unchecked)
                {
                    bw.BaseStream.Seek(0x1E4, SeekOrigin.Begin);
                    bw.Write(0);
                }

                if (ChkTV1.CheckState == CheckState.Checked)
                {
                    bw.BaseStream.Seek(0x1E8, SeekOrigin.Begin);
                    bw.Write(0x000FFFFF);
                }
                else if (ChkTV1.CheckState == CheckState.Unchecked)
                {
                    bw.BaseStream.Seek(0x1E8, SeekOrigin.Begin);
                    bw.Write(0);
                }

                if (ChkTV2.CheckState == CheckState.Checked)
                {
                    bw.BaseStream.Seek(0x1EC, SeekOrigin.Begin);
                    bw.Write(0x000FFFFF);
                }
                else if (ChkTV2.CheckState == CheckState.Unchecked)
                {
                    bw.BaseStream.Seek(0x1EC, SeekOrigin.Begin);
                    bw.Write(0);
                }

                if (Convert.ToUInt32(TxtPlayTimeSeconds.Text) != SAVFile.Story.PlayTimeInSeconds)
                {
                    bw.BaseStream.Seek(0x200, SeekOrigin.Begin);
                    bw.Write(Convert.ToUInt32(TxtPlayTimeSeconds.Text));
                }

                fs.Dispose();
                fs.Close();

                MessageBox.Show($"File saved at:\n{NewSAVFilePath}", "Saved file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LinkLblGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/NiV-L-A");
        }

        public enum Language
        {
            ENGLISH = 0,
            GERMAN = 2,
            FRENCH = 3,
            SPANISH = 4,
            ITALIAN = 5,
            PORTUGUESE = 9
        }
        public enum XboxKeys
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3,
            Y = 4,
            B = 5,
            A = 6,
            X = 7,
            LB = 8,
            LT = 9,
            L = 10,
            RB = 11,
            RT = 12,
            R = 13,
            NULL = 14,
            BACK = 15,
            DASH = 255
        }
    }
}
