using System;
using System.Windows.Forms;

namespace DuckTales_Remastered_SaveFileManager
{
    public partial class CRCEditForm : Form
    {
        public CRCEditForm(SAVStream SAVFile)
        {
            InitializeComponent();
            TxtNewCRC.Text = SAVFile.CRC.ToString("X8");
        }

        public uint NewCRC { get; set; }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                NewCRC = uint.Parse(TxtNewCRC.Text, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is OverflowException)
                {
                    MessageBox.Show("Failed to parse the value!", "Edit CRC's Residue Constant - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
