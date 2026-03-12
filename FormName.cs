using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Collector
{
    public partial class FormName : Form
    {
        public CoinStruc cs = new CoinStruc();
        public bool saveMode= false;
        public bool autoFillUp = false;

        public string coinName = "";
        private string _coinName = "";
        public const string MODELSDIRECTORY = "D:\\COINTER\\Images\\ModelsN\\";

        public FormName()
        {
            InitializeComponent();

            listBoxCoinStatus.Items.Add("Current");
            listBoxCoinStatus.Items.Add("Outdated");
            listBoxCoinStatus.Items.Add("Obsolete");

            listBoxNumberType.Items.Add("Numista");
            listBoxNumberType.Items.Add("ChangeBox");

            listBoxCoinFace.Items.Add("Obverse");
            listBoxCoinFace.Items.Add("Reverse");

            checkBoxAutoFillUp.Checked = true;

            CreateCoinName();

        }

        public struct CoinStruc 
        {
            
            //public string fromFile_Currency { get { return fromFile_Currency; } set { fromFile_Currency = value; } }
            
            public string fromFile_Currency = "NON";
            public string fromFile_Denomin = "00000000";
            public string fromFile_Diameter = "0000";
            public string fromFile_Magn1 = "0000";
            public string fromFile_Magn2 = "0000";
            public string fromFile_Magn3 = "0000";
            public string fromFile_Thickness = "0000";
            public string fromFile_Colour = "0000";
            public string fromFile_Material = "0000";
            public string fromFile_Weight = "0000";
            public string fromFile_Variant = "0000";

            public float fs_Diameter;
            public float fs_Magnetic1;
            public float fs_Magnetic2;
            public float fs_Magnetic3;
            public float fs_Thickness;
            public float fs_Colour;

            public CoinStruc() 
            { 
                fromFile_Currency = "NON";
                fromFile_Denomin = "00000000";
                fromFile_Diameter = "0000";
                fromFile_Magn1 = "0000";
                fromFile_Magn2 = "0000";
                fromFile_Magn3 = "0000";
                fromFile_Thickness = "0000";
                fromFile_Colour = "0000";
                fromFile_Material = "0000";
                fromFile_Weight = "0000";
                fromFile_Variant = "0000";
            }
        }

        public string GetCoinName()
        {
            return _coinName;
        }

        public void SetCoinName(string CName)
        {
            _coinName = CName;
        }

        public bool GetSaveMode()
        {            
            return saveMode;
        }
        
        /*
        public void ClearSaveMode()
        {
            saveMode = false;
        }*/

        public CoinStruc decodeCoinName(string coinName)
        {
            CoinStruc c = new CoinStruc() ;
            string[] ActualImageSplit = coinName.Split('_');
            string fromFile_CurrencyFull = ActualImageSplit[0];
            int stringLenght = fromFile_CurrencyFull.Length;
           
            if (ActualImageSplit.Length > 9)
            {                
                c.fromFile_Currency = fromFile_CurrencyFull.Substring(stringLenght - 3, 3);
                c.fromFile_Denomin = ActualImageSplit[1];
                c.fromFile_Denomin = ActualImageSplit[1];
                c.fromFile_Diameter = ActualImageSplit[3];
                c.fromFile_Magn1 = ActualImageSplit[4];
                c.fromFile_Magn2 = ActualImageSplit[5];
                c.fromFile_Magn3 = ActualImageSplit[6];
                c.fromFile_Thickness = ActualImageSplit[7];
                c.fromFile_Colour = ActualImageSplit[8];
                c.fromFile_Material = ActualImageSplit[9];
                c.fromFile_Weight = ActualImageSplit[10];
                c.fromFile_Variant = ActualImageSplit[2];
            }

            c.fs_Diameter = int.Parse(c.fromFile_Diameter);
            c.fs_Thickness = int.Parse(c.fromFile_Thickness);
            c.fs_Colour = int.Parse(c.fromFile_Colour);
            c.fs_Magnetic1 = int.Parse(c.fromFile_Magn1);
            c.fs_Magnetic2 = int.Parse(c.fromFile_Magn2);
            c.fs_Magnetic3 = int.Parse(c.fromFile_Magn3);
            /*
            ActualFileName = ActualImageFilename.Substring(stringLenght - 3, ActualImageFilename.Length - stringLenght - 1);
            
            if (imagePathsIndex < (imageFilePathsEdges.Length - 1))
                imagePathsIndex++;
            else
                imagePathsIndex = 0;
            */
            return c;
        }

        private void CreateCoinName()
        {
            //Curency_Denom_CoinID_Diameter_Magn1_Magn2_Magn3_Thickness_Colour_Material_Weight
            string CoinNamePre = textBoxCurrency.Text + "_" + textBoxValue.Text;
            string CoinNameVariant = "";

            if (listBoxCoinStatus.GetSelected(0)) CoinNameVariant = "1";
            else if (listBoxCoinStatus.GetSelected(1)) CoinNameVariant = "2";
            else CoinNameVariant = "3";

            if (listBoxNumberType.GetSelected(0)) CoinNameVariant = CoinNameVariant + "N";
            else CoinNameVariant = CoinNameVariant + "C";

            CoinNameVariant = CoinNameVariant + textBoxVariant.Text;

            if (listBoxCoinFace.GetSelected(0)) CoinNameVariant = CoinNameVariant + "O";
            else CoinNameVariant = CoinNameVariant + "R";

            CoinNameVariant = CoinNameVariant + textBoxSpare.Text;

            string CoinNameMidFlex = textBoxDiameter.Text + "_" + textBoxMag1.Text + "_" + textBoxMag2.Text + "_" + textBoxMag3.Text + "_" + textBoxThicknes.Text + "_" + textBoxColour.Text;
            string CoinNamePost = textBoxMaterial.Text + "_" + textBoxWeight.Text;

            string CoinName = CoinNamePre + "_" + CoinNameVariant + "_" + CoinNameMidFlex + "_" + CoinNamePost;

            labelName.Text = CoinName;
            _coinName = CoinName;
            labelName.ForeColor = Color.Blue;
            saveMode = true;

            var modelFileNames = Directory.GetFiles(MODELSDIRECTORY, CoinNamePre+"*.bmp", SearchOption.TopDirectoryOnly);
            listBoxNamesFound.Items.Clear();
            foreach (var modelFileName in modelFileNames)
            {
                StringBuilder builder = new StringBuilder(modelFileName);
                builder.Replace(MODELSDIRECTORY, "");                
                listBoxNamesFound.Items.Add(builder.ToString());
            }
            
        }

        private void textBoxCurrency_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void buttonNameCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonNameOK_Click(object sender, EventArgs e)
        {
            CreateCoinName();
            coinName = _coinName;
            this.Close();
        }

        private void textBoxValue_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void listBoxCoinStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void listBoxNumberType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxVariant_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void listBoxCoinFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxSpare_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxDiameter_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxMag1_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxMag2_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxMag3_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxThicknes_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxColour_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxWeight_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void textBoxMaterial_TextChanged(object sender, EventArgs e)
        {
            //Validation
            CreateCoinName();
        }

        private void checkBoxAutoFillUp_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoFillUp.Checked == true)
            {
                textBoxDiameter.Text = "0000";
                textBoxMag1.Text = "0000";
                textBoxMag2.Text = "0000";
                textBoxMag3.Text = "0000";
                textBoxThicknes.Text = "0000";
                textBoxColour.Text = "0000";

                textBoxDiameter.Enabled = false;
                textBoxMag1.Enabled = false;
                textBoxMag2.Enabled = false;
                textBoxMag3.Enabled = false;
                textBoxThicknes.Enabled = false;
                textBoxColour.Enabled = false;

                autoFillUp = true;
            }
            else
            {
                textBoxDiameter.Text = "1234";
                textBoxMag1.Text = "1111";
                textBoxMag2.Text = "2222";
                textBoxMag3.Text = "3333";
                textBoxThicknes.Text = "4444";
                textBoxColour.Text = "0100";

                textBoxDiameter.Enabled = true;
                textBoxMag1.Enabled = true;
                textBoxMag2.Enabled = true;
                textBoxMag3.Enabled = true;
                textBoxThicknes.Enabled = true;
                textBoxColour.Enabled = true;

                autoFillUp = false;
            }
            
            //Validation
            CreateCoinName();

        }
    }
}
