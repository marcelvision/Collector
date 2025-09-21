namespace Cointero
{
    partial class FormImageMan
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBoxSImageFiles = new ListBox();
            textBoxNoUniqCoins = new TextBox();
            labelUniqImg = new Label();
            buttonSpreadSelected = new Button();
            buttonSpreadAll = new Button();
            buttonListRef = new Button();
            listBoxModels = new ListBox();
            textBoxNoModels = new TextBox();
            labelModels = new Label();
            buttonModelsRef = new Button();
            buttonSetRunListAll = new Button();
            buttonSetRunList = new Button();
            textBoxAllFiles = new TextBox();
            textBoxSearchModels = new TextBox();
            SuspendLayout();
            // 
            // listBoxSImageFiles
            // 
            listBoxSImageFiles.FormattingEnabled = true;
            listBoxSImageFiles.ItemHeight = 15;
            listBoxSImageFiles.Location = new Point(31, 92);
            listBoxSImageFiles.Name = "listBoxSImageFiles";
            listBoxSImageFiles.Size = new Size(323, 394);
            listBoxSImageFiles.TabIndex = 0;
            listBoxSImageFiles.SelectedIndexChanged += listBoxSImageFiles_SelectedIndexChanged;
            listBoxSImageFiles.SelectedValueChanged += listBoxSImageFiles_SelectedValueChanged;
            // 
            // textBoxNoUniqCoins
            // 
            textBoxNoUniqCoins.Location = new Point(31, 63);
            textBoxNoUniqCoins.Name = "textBoxNoUniqCoins";
            textBoxNoUniqCoins.Size = new Size(56, 23);
            textBoxNoUniqCoins.TabIndex = 1;
            // 
            // labelUniqImg
            // 
            labelUniqImg.AutoSize = true;
            labelUniqImg.Location = new Point(93, 66);
            labelUniqImg.Name = "labelUniqImg";
            labelUniqImg.Size = new Size(108, 15);
            labelUniqImg.TabIndex = 2;
            labelUniqImg.Text = "unique coin names";
            labelUniqImg.Click += label1_Click;
            // 
            // buttonSpreadSelected
            // 
            buttonSpreadSelected.Location = new Point(246, 62);
            buttonSpreadSelected.Name = "buttonSpreadSelected";
            buttonSpreadSelected.Size = new Size(108, 23);
            buttonSpreadSelected.TabIndex = 3;
            buttonSpreadSelected.Text = "Process selected";
            buttonSpreadSelected.UseVisualStyleBackColor = true;
            buttonSpreadSelected.Click += buttonSpreadSelected_Click;
            // 
            // buttonSpreadAll
            // 
            buttonSpreadAll.Location = new Point(246, 33);
            buttonSpreadAll.Name = "buttonSpreadAll";
            buttonSpreadAll.Size = new Size(108, 23);
            buttonSpreadAll.TabIndex = 4;
            buttonSpreadAll.Text = "Process all";
            buttonSpreadAll.UseVisualStyleBackColor = true;
            buttonSpreadAll.Click += buttonSpreadAll_Click;
            // 
            // buttonListRef
            // 
            buttonListRef.Location = new Point(31, 33);
            buttonListRef.Name = "buttonListRef";
            buttonListRef.Size = new Size(108, 23);
            buttonListRef.TabIndex = 5;
            buttonListRef.Text = "Refresh List";
            buttonListRef.UseVisualStyleBackColor = true;
            buttonListRef.Click += buttonListRef_Click;
            // 
            // listBoxModels
            // 
            listBoxModels.FormattingEnabled = true;
            listBoxModels.ItemHeight = 15;
            listBoxModels.Location = new Point(445, 92);
            listBoxModels.Name = "listBoxModels";
            listBoxModels.Size = new Size(320, 394);
            listBoxModels.TabIndex = 6;
            listBoxModels.SelectedIndexChanged += listBoxModels_SelectedIndexChanged;
            // 
            // textBoxNoModels
            // 
            textBoxNoModels.Location = new Point(445, 62);
            textBoxNoModels.Name = "textBoxNoModels";
            textBoxNoModels.Size = new Size(56, 23);
            textBoxNoModels.TabIndex = 7;
            // 
            // labelModels
            // 
            labelModels.AutoSize = true;
            labelModels.Location = new Point(507, 66);
            labelModels.Name = "labelModels";
            labelModels.Size = new Size(46, 15);
            labelModels.TabIndex = 8;
            labelModels.Text = "models";
            // 
            // buttonModelsRef
            // 
            buttonModelsRef.Location = new Point(445, 33);
            buttonModelsRef.Name = "buttonModelsRef";
            buttonModelsRef.Size = new Size(108, 23);
            buttonModelsRef.TabIndex = 9;
            buttonModelsRef.Text = "Refresh List";
            buttonModelsRef.UseVisualStyleBackColor = true;
            buttonModelsRef.Click += buttonModelsRef_Click;
            // 
            // buttonSetRunListAll
            // 
            buttonSetRunListAll.Location = new Point(657, 33);
            buttonSetRunListAll.Name = "buttonSetRunListAll";
            buttonSetRunListAll.Size = new Size(108, 23);
            buttonSetRunListAll.TabIndex = 11;
            buttonSetRunListAll.Text = "Set run all";
            buttonSetRunListAll.UseVisualStyleBackColor = true;
            buttonSetRunListAll.Click += buttonSetRunListAll_Click;
            // 
            // buttonSetRunList
            // 
            buttonSetRunList.Location = new Point(657, 62);
            buttonSetRunList.Name = "buttonSetRunList";
            buttonSetRunList.Size = new Size(108, 23);
            buttonSetRunList.TabIndex = 10;
            buttonSetRunList.Text = "Set run selected";
            buttonSetRunList.UseVisualStyleBackColor = true;
            buttonSetRunList.Click += buttonSetRunList_Click;
            // 
            // textBoxAllFiles
            // 
            textBoxAllFiles.Location = new Point(577, 62);
            textBoxAllFiles.Name = "textBoxAllFiles";
            textBoxAllFiles.Size = new Size(56, 23);
            textBoxAllFiles.TabIndex = 12;
            // 
            // textBoxSearchModels
            // 
            textBoxSearchModels.Location = new Point(559, 34);
            textBoxSearchModels.Name = "textBoxSearchModels";
            textBoxSearchModels.Size = new Size(92, 23);
            textBoxSearchModels.TabIndex = 13;
            textBoxSearchModels.TextChanged += textBoxSearchModels_TextChanged;
            // 
            // FormImageMan
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 511);
            Controls.Add(textBoxSearchModels);
            Controls.Add(textBoxAllFiles);
            Controls.Add(buttonSetRunListAll);
            Controls.Add(buttonSetRunList);
            Controls.Add(buttonModelsRef);
            Controls.Add(labelModels);
            Controls.Add(textBoxNoModels);
            Controls.Add(listBoxModels);
            Controls.Add(buttonListRef);
            Controls.Add(buttonSpreadAll);
            Controls.Add(buttonSpreadSelected);
            Controls.Add(labelUniqImg);
            Controls.Add(textBoxNoUniqCoins);
            Controls.Add(listBoxSImageFiles);
            Name = "FormImageMan";
            Text = "FormImageMan";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxSImageFiles;
        private TextBox textBoxNoUniqCoins;
        private Label labelUniqImg;
        private Button buttonSpreadSelected;
        private Button buttonSpreadAll;
        private Button buttonListRef;
        private ListBox listBoxModels;
        private TextBox textBoxNoModels;
        private Label labelModels;
        private Button buttonModelsRef;
        private Button buttonSetRunListAll;
        private Button buttonSetRunList;
        private TextBox textBoxAllFiles;
        private TextBox textBoxSearchModels;
    }
}