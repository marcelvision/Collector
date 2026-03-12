namespace Cointero
{
    partial class FormCollect
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param NameForm="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            textBoxCameraCounters = new TextBox();
            textBoxCoinName = new TextBox();
            buttonSetModels = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            textBoxFileName = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(4, 11);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(600, 540);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(610, 5);
            pictureBox2.Margin = new Padding(3, 2, 3, 2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(600, 540);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(248, 549);
            pictureBox3.Margin = new Padding(3, 2, 3, 2);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(720, 74);
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // textBoxCameraCounters
            // 
            textBoxCameraCounters.BackColor = SystemColors.Desktop;
            textBoxCameraCounters.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            textBoxCameraCounters.ForeColor = SystemColors.Info;
            textBoxCameraCounters.Location = new Point(486, 526);
            textBoxCameraCounters.Margin = new Padding(3, 2, 3, 2);
            textBoxCameraCounters.Name = "textBoxCameraCounters";
            textBoxCameraCounters.ReadOnly = true;
            textBoxCameraCounters.Size = new Size(244, 25);
            textBoxCameraCounters.TabIndex = 3;
            textBoxCameraCounters.Text = "---";
            textBoxCameraCounters.TextAlign = HorizontalAlignment.Center;
            // 
            // textBoxCoinName
            // 
            textBoxCoinName.BackColor = SystemColors.Desktop;
            textBoxCoinName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            textBoxCoinName.ForeColor = SystemColors.Info;
            textBoxCoinName.Location = new Point(558, 497);
            textBoxCoinName.Margin = new Padding(3, 2, 3, 2);
            textBoxCoinName.Name = "textBoxCoinName";
            textBoxCoinName.ReadOnly = true;
            textBoxCoinName.Size = new Size(97, 25);
            textBoxCoinName.TabIndex = 4;
            textBoxCoinName.Text = "---";
            textBoxCoinName.TextAlign = HorizontalAlignment.Center;
            // 
            // buttonSetModels
            // 
            buttonSetModels.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonSetModels.Location = new Point(17, 588);
            buttonSetModels.Margin = new Padding(3, 2, 3, 2);
            buttonSetModels.Name = "buttonSetModels";
            buttonSetModels.Size = new Size(52, 22);
            buttonSetModels.TabIndex = 5;
            buttonSetModels.Text = "COM";
            buttonSetModels.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button2.Location = new Point(101, 588);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(49, 22);
            button2.TabIndex = 6;
            button2.Text = "CAM";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button3.Location = new Point(185, 588);
            button3.Margin = new Padding(3, 2, 3, 2);
            button3.Name = "button3";
            button3.Size = new Size(49, 22);
            button3.TabIndex = 7;
            button3.Text = "IP STAT";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button4.Location = new Point(984, 588);
            button4.Margin = new Padding(3, 2, 3, 2);
            button4.Name = "button4";
            button4.Size = new Size(48, 22);
            button4.TabIndex = 10;
            button4.Text = "AUTO";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button5.Location = new Point(1067, 588);
            button5.Margin = new Padding(3, 2, 3, 2);
            button5.Name = "button5";
            button5.Size = new Size(49, 22);
            button5.TabIndex = 9;
            button5.Text = "NAME";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button6.Location = new Point(1154, 588);
            button6.Margin = new Padding(3, 2, 3, 2);
            button6.Name = "button6";
            button6.Size = new Size(46, 22);
            button6.TabIndex = 8;
            button6.Text = "STOP";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // textBoxFileName
            // 
            textBoxFileName.BackColor = SystemColors.Desktop;
            textBoxFileName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            textBoxFileName.ForeColor = SystemColors.Info;
            textBoxFileName.Location = new Point(248, 11);
            textBoxFileName.Margin = new Padding(3, 2, 3, 2);
            textBoxFileName.Name = "textBoxFileName";
            textBoxFileName.ReadOnly = true;
            textBoxFileName.Size = new Size(720, 25);
            textBoxFileName.TabIndex = 11;
            textBoxFileName.Text = "---";
            textBoxFileName.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.InfoText;
            textBox2.ForeColor = SystemColors.Info;
            textBox2.Location = new Point(504, 41);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 12;
            textBox2.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            textBox3.BackColor = SystemColors.InfoText;
            textBox3.ForeColor = SystemColors.Info;
            textBox3.Location = new Point(610, 41);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(100, 23);
            textBox3.TabIndex = 13;
            textBox3.TextAlign = HorizontalAlignment.Center;
            // 
            // FormCollect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(1216, 628);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBoxFileName);
            Controls.Add(button4);
            Controls.Add(button5);
            Controls.Add(button6);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(buttonSetModels);
            Controls.Add(textBoxCoinName);
            Controls.Add(textBoxCameraCounters);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "FormCollect";
            Text = "Collector";
            Load += Main_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private TextBox textBoxCameraCounters;
        private TextBox textBoxCoinName;
        private Button buttonSetModels;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
		private TextBox textBoxFileName;
        private TextBox textBox2;
        private TextBox textBox3;
    }
}
