namespace Cointero
{
    partial class FormName
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param Name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBoxCoinFace = new ListBox();
            textBoxVariant = new TextBox();
            listBoxNumberType = new ListBox();
            listBoxCoinStatus = new ListBox();
            textBoxValue = new TextBox();
            textBoxCurrency = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            textBoxDiameter = new TextBox();
            textBoxMag1 = new TextBox();
            textBoxMag2 = new TextBox();
            textBoxMag3 = new TextBox();
            textBoxMaterial = new TextBox();
            textBoxWeight = new TextBox();
            textBoxColour = new TextBox();
            textBoxThicknes = new TextBox();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            label16 = new Label();
            checkBoxAutoFillUp = new CheckBox();
            buttonNameOK = new Button();
            buttonNameCancel = new Button();
            label17 = new Label();
            labelName = new Label();
            textBoxSpare = new TextBox();
            label18 = new Label();
            listBoxNamesFound = new ListBox();
            SuspendLayout();
            // 
            // listBoxCoinFace
            // 
            listBoxCoinFace.FormattingEnabled = true;
            listBoxCoinFace.ItemHeight = 15;
            listBoxCoinFace.Location = new Point(537, 166);
            listBoxCoinFace.Name = "listBoxCoinFace";
            listBoxCoinFace.Size = new Size(70, 34);
            listBoxCoinFace.TabIndex = 25;
            listBoxCoinFace.SelectedIndexChanged += listBoxCoinFace_SelectedIndexChanged;
            // 
            // textBoxVariant
            // 
            textBoxVariant.Location = new Point(437, 166);
            textBoxVariant.MaxLength = 8;
            textBoxVariant.Name = "textBoxVariant";
            textBoxVariant.Size = new Size(71, 23);
            textBoxVariant.TabIndex = 24;
            textBoxVariant.Text = "00001234";
            textBoxVariant.TextAlign = HorizontalAlignment.Center;
            textBoxVariant.TextChanged += textBoxVariant_TextChanged;
            // 
            // listBoxNumberType
            // 
            listBoxNumberType.FormattingEnabled = true;
            listBoxNumberType.ItemHeight = 15;
            listBoxNumberType.Location = new Point(340, 166);
            listBoxNumberType.Name = "listBoxNumberType";
            listBoxNumberType.Size = new Size(68, 34);
            listBoxNumberType.TabIndex = 23;
            listBoxNumberType.SelectedIndexChanged += listBoxNumberType_SelectedIndexChanged;
            // 
            // listBoxCoinStatus
            // 
            listBoxCoinStatus.FormattingEnabled = true;
            listBoxCoinStatus.ItemHeight = 15;
            listBoxCoinStatus.Location = new Point(238, 166);
            listBoxCoinStatus.Name = "listBoxCoinStatus";
            listBoxCoinStatus.Size = new Size(73, 49);
            listBoxCoinStatus.TabIndex = 22;
            listBoxCoinStatus.SelectedIndexChanged += listBoxCoinStatus_SelectedIndexChanged;
            // 
            // textBoxValue
            // 
            textBoxValue.Location = new Point(138, 166);
            textBoxValue.MaxLength = 8;
            textBoxValue.Name = "textBoxValue";
            textBoxValue.Size = new Size(71, 23);
            textBoxValue.TabIndex = 21;
            textBoxValue.Text = "00001000";
            textBoxValue.TextAlign = HorizontalAlignment.Center;
            textBoxValue.TextChanged += textBoxValue_TextChanged;
            // 
            // textBoxCurrency
            // 
            textBoxCurrency.Location = new Point(63, 166);
            textBoxCurrency.MaxLength = 3;
            textBoxCurrency.Name = "textBoxCurrency";
            textBoxCurrency.Size = new Size(46, 23);
            textBoxCurrency.TabIndex = 20;
            textBoxCurrency.Text = "CUR";
            textBoxCurrency.TextAlign = HorizontalAlignment.Center;
            textBoxCurrency.TextChanged += textBoxCurrency_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(58, 148);
            label1.Name = "label1";
            label1.Size = new Size(51, 15);
            label1.TabIndex = 26;
            label1.Text = "Curency";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(135, 148);
            label2.Name = "label2";
            label2.Size = new Size(76, 15);
            label2.TabIndex = 27;
            label2.Text = "Value 8 digits";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(139, 200);
            label3.Name = "label3";
            label3.Size = new Size(68, 15);
            label3.TabIndex = 28;
            label3.Text = "head with 0";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(241, 148);
            label4.Name = "label4";
            label4.Size = new Size(66, 15);
            label4.TabIndex = 29;
            label4.Text = "Coin status";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(331, 148);
            label5.Name = "label5";
            label5.Size = new Size(87, 15);
            label5.TabIndex = 30;
            label5.Text = "Nimista / FCCE";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(428, 148);
            label6.Name = "label6";
            label6.Size = new Size(89, 15);
            label6.TabIndex = 31;
            label6.Text = "iD / Numista N.";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(530, 148);
            label7.Name = "label7";
            label7.Size = new Size(85, 15);
            label7.TabIndex = 32;
            label7.Text = "Train coin side ";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(62, 200);
            label8.Name = "label8";
            label8.Size = new Size(48, 15);
            label8.TabIndex = 33;
            label8.Text = "3 letters";
            // 
            // textBoxDiameter
            // 
            textBoxDiameter.Location = new Point(63, 270);
            textBoxDiameter.MaxLength = 4;
            textBoxDiameter.Name = "textBoxDiameter";
            textBoxDiameter.Size = new Size(46, 23);
            textBoxDiameter.TabIndex = 34;
            textBoxDiameter.Text = "1234";
            textBoxDiameter.TextAlign = HorizontalAlignment.Center;
            textBoxDiameter.TextChanged += textBoxDiameter_TextChanged;
            // 
            // textBoxMag1
            // 
            textBoxMag1.Location = new Point(144, 270);
            textBoxMag1.MaxLength = 4;
            textBoxMag1.Name = "textBoxMag1";
            textBoxMag1.Size = new Size(46, 23);
            textBoxMag1.TabIndex = 35;
            textBoxMag1.Text = "1111";
            textBoxMag1.TextAlign = HorizontalAlignment.Center;
            textBoxMag1.TextChanged += textBoxMag1_TextChanged;
            // 
            // textBoxMag2
            // 
            textBoxMag2.Location = new Point(225, 270);
            textBoxMag2.MaxLength = 4;
            textBoxMag2.Name = "textBoxMag2";
            textBoxMag2.Size = new Size(46, 23);
            textBoxMag2.TabIndex = 36;
            textBoxMag2.Text = "2222";
            textBoxMag2.TextAlign = HorizontalAlignment.Center;
            textBoxMag2.TextChanged += textBoxMag2_TextChanged;
            // 
            // textBoxMag3
            // 
            textBoxMag3.Location = new Point(306, 270);
            textBoxMag3.MaxLength = 4;
            textBoxMag3.Name = "textBoxMag3";
            textBoxMag3.Size = new Size(46, 23);
            textBoxMag3.TabIndex = 37;
            textBoxMag3.Text = "3333";
            textBoxMag3.TextAlign = HorizontalAlignment.Center;
            textBoxMag3.TextChanged += textBoxMag3_TextChanged;
            // 
            // textBoxMaterial
            // 
            textBoxMaterial.Location = new Point(549, 270);
            textBoxMaterial.MaxLength = 4;
            textBoxMaterial.Name = "textBoxMaterial";
            textBoxMaterial.Size = new Size(46, 23);
            textBoxMaterial.TabIndex = 41;
            textBoxMaterial.Text = "Gold";
            textBoxMaterial.TextAlign = HorizontalAlignment.Center;
            textBoxMaterial.TextChanged += textBoxMaterial_TextChanged;
            // 
            // textBoxWeight
            // 
            textBoxWeight.Location = new Point(630, 270);
            textBoxWeight.MaxLength = 4;
            textBoxWeight.Name = "textBoxWeight";
            textBoxWeight.Size = new Size(46, 23);
            textBoxWeight.TabIndex = 40;
            textBoxWeight.Text = "0555";
            textBoxWeight.TextAlign = HorizontalAlignment.Center;
            textBoxWeight.TextChanged += textBoxWeight_TextChanged;
            // 
            // textBoxColour
            // 
            textBoxColour.Location = new Point(468, 270);
            textBoxColour.MaxLength = 4;
            textBoxColour.Name = "textBoxColour";
            textBoxColour.Size = new Size(46, 23);
            textBoxColour.TabIndex = 39;
            textBoxColour.Text = "0100";
            textBoxColour.TextAlign = HorizontalAlignment.Center;
            textBoxColour.TextChanged += textBoxColour_TextChanged;
            // 
            // textBoxThicknes
            // 
            textBoxThicknes.Location = new Point(387, 270);
            textBoxThicknes.MaxLength = 4;
            textBoxThicknes.Name = "textBoxThicknes";
            textBoxThicknes.Size = new Size(46, 23);
            textBoxThicknes.TabIndex = 38;
            textBoxThicknes.Text = "4444";
            textBoxThicknes.TextAlign = HorizontalAlignment.Center;
            textBoxThicknes.TextChanged += textBoxThicknes_TextChanged;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new Point(59, 249);
            label9.Name = "label9";
            label9.Size = new Size(55, 15);
            label9.TabIndex = 42;
            label9.Text = "Diameter";
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Location = new Point(135, 249);
            label10.Name = "label10";
            label10.Size = new Size(66, 15);
            label10.TabIndex = 43;
            label10.Text = "Magnetic 1";
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Location = new Point(214, 249);
            label11.Name = "label11";
            label11.Size = new Size(66, 15);
            label11.TabIndex = 44;
            label11.Text = "Magnetic 2";
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Location = new Point(297, 249);
            label12.Name = "label12";
            label12.Size = new Size(66, 15);
            label12.TabIndex = 45;
            label12.Text = "Magnetic 3";
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label13.AutoSize = true;
            label13.Location = new Point(384, 249);
            label13.Name = "label13";
            label13.Size = new Size(53, 15);
            label13.TabIndex = 46;
            label13.Text = "Thicknes";
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label14.AutoSize = true;
            label14.Location = new Point(470, 249);
            label14.Name = "label14";
            label14.Size = new Size(43, 15);
            label14.TabIndex = 47;
            label14.Text = "Colour";
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label15.AutoSize = true;
            label15.Location = new Point(631, 249);
            label15.Name = "label15";
            label15.Size = new Size(45, 15);
            label15.TabIndex = 48;
            label15.Text = "Weight";
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label16.AutoSize = true;
            label16.Location = new Point(547, 249);
            label16.Name = "label16";
            label16.Size = new Size(50, 15);
            label16.TabIndex = 49;
            label16.Text = "Material";
            // 
            // checkBoxAutoFillUp
            // 
            checkBoxAutoFillUp.AutoSize = true;
            checkBoxAutoFillUp.Location = new Point(65, 329);
            checkBoxAutoFillUp.Name = "checkBoxAutoFillUp";
            checkBoxAutoFillUp.Size = new Size(158, 19);
            checkBoxAutoFillUp.TabIndex = 50;
            checkBoxAutoFillUp.Text = "Auto fill up sensor values";
            checkBoxAutoFillUp.UseVisualStyleBackColor = true;
            checkBoxAutoFillUp.CheckedChanged += checkBoxAutoFillUp_CheckedChanged;
            // 
            // buttonNameOK
            // 
            buttonNameOK.Location = new Point(601, 329);
            buttonNameOK.Name = "buttonNameOK";
            buttonNameOK.Size = new Size(75, 23);
            buttonNameOK.TabIndex = 51;
            buttonNameOK.Text = "OK";
            buttonNameOK.UseVisualStyleBackColor = true;
            buttonNameOK.Click += buttonNameOK_Click;
            // 
            // buttonNameCancel
            // 
            buttonNameCancel.Location = new Point(498, 329);
            buttonNameCancel.Name = "buttonNameCancel";
            buttonNameCancel.Size = new Size(75, 23);
            buttonNameCancel.TabIndex = 52;
            buttonNameCancel.Text = "Cancel";
            buttonNameCancel.UseVisualStyleBackColor = true;
            buttonNameCancel.Click += buttonNameCancel_Click;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label17.Location = new Point(105, 27);
            label17.Name = "label17";
            label17.Size = new Size(73, 15);
            label17.TabIndex = 53;
            label17.Text = "Coin Name: ";
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelName.Location = new Point(178, 27);
            labelName.Name = "labelName";
            labelName.Size = new Size(461, 15);
            labelName.TabIndex = 54;
            labelName.Text = "CUR_00001000_1N00000001O999_1234_1111_2222_3333_4444_0100_5555_Gold";
            // 
            // textBoxSpare
            // 
            textBoxSpare.Location = new Point(632, 166);
            textBoxSpare.MaxLength = 3;
            textBoxSpare.Name = "textBoxSpare";
            textBoxSpare.Size = new Size(46, 23);
            textBoxSpare.TabIndex = 55;
            textBoxSpare.Text = "999";
            textBoxSpare.TextAlign = HorizontalAlignment.Center;
            textBoxSpare.TextChanged += textBoxSpare_TextChanged;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(627, 148);
            label18.Name = "label18";
            label18.Size = new Size(57, 15);
            label18.TabIndex = 56;
            label18.Text = "unique id";
            // 
            // listBoxNamesFound
            // 
            listBoxNamesFound.FormattingEnabled = true;
            listBoxNamesFound.ItemHeight = 15;
            listBoxNamesFound.Location = new Point(109, 56);
            listBoxNamesFound.Name = "listBoxNamesFound";
            listBoxNamesFound.Size = new Size(530, 64);
            listBoxNamesFound.TabIndex = 57;
            // 
            // FormName
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(728, 387);
            Controls.Add(listBoxNamesFound);
            Controls.Add(label18);
            Controls.Add(textBoxSpare);
            Controls.Add(labelName);
            Controls.Add(label17);
            Controls.Add(buttonNameCancel);
            Controls.Add(buttonNameOK);
            Controls.Add(checkBoxAutoFillUp);
            Controls.Add(label16);
            Controls.Add(label15);
            Controls.Add(label14);
            Controls.Add(label13);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(textBoxMaterial);
            Controls.Add(textBoxWeight);
            Controls.Add(textBoxColour);
            Controls.Add(textBoxThicknes);
            Controls.Add(textBoxMag3);
            Controls.Add(textBoxMag2);
            Controls.Add(textBoxMag1);
            Controls.Add(textBoxDiameter);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(listBoxCoinFace);
            Controls.Add(textBoxVariant);
            Controls.Add(listBoxNumberType);
            Controls.Add(listBoxCoinStatus);
            Controls.Add(textBoxValue);
            Controls.Add(textBoxCurrency);
            Name = "FormName";
            Text = "FormName";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxCoinFace;
        private TextBox textBoxVariant;
        private ListBox listBoxNumberType;
        private ListBox listBoxCoinStatus;
        private TextBox textBoxValue;
        private TextBox textBoxCurrency;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private TextBox textBoxDiameter;
        private TextBox textBoxMag1;
        private TextBox textBoxMag2;
        private TextBox textBoxMag3;
        private TextBox textBoxMaterial;
        private TextBox textBoxWeight;
        private TextBox textBoxColour;
        private TextBox textBoxThicknes;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private CheckBox checkBoxAutoFillUp;
        private Button buttonNameOK;
        private Button buttonNameCancel;
        private Label label17;
        private Label labelName;
        private TextBox textBoxSpare;
        private Label label18;
        private ListBox listBoxNamesFound;
    }
}