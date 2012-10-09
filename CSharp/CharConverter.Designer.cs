// Copyright (c) 2012, SmiteWorks USA LLC

namespace CharacterConverter
{
    partial class CharConverter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharConverter));
            this.PCGenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createLocalXslt = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.outputOptions = new System.Windows.Forms.CheckedListBox();
            this.consoleBox = new System.Windows.Forms.GroupBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.console = new System.Windows.Forms.RichTextBox();
            this.SaveToTextButton = new System.Windows.Forms.Button();
            this.clipboardButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RefreshCampaignsButton = new System.Windows.Forms.Button();
            this.campaignList = new System.Windows.Forms.CheckedListBox();
            this.CampaignCheckBox = new System.Windows.Forms.CheckBox();
            this.LocalCheckBox = new System.Windows.Forms.CheckBox();
            this.XmlCheckBox = new System.Windows.Forms.CheckBox();
            this.FGPath = new System.Windows.Forms.TextBox();
            this.AppDataButton = new System.Windows.Forms.Button();
            this.XmlLocationButton = new System.Windows.Forms.Button();
            this.XmlOutputLocation = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SourceBox = new System.Windows.Forms.ComboBox();
            this.RulesetBox = new System.Windows.Forms.ComboBox();
            this.FileButton = new System.Windows.Forms.Button();
            this.importLocation = new System.Windows.Forms.TextBox();
            this.ConvertButton = new System.Windows.Forms.Button();
            this.importDialog = new System.Windows.Forms.OpenFileDialog();
            this.xmlOutputDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.saveToTxtFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.appDataDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.xslDialog = new System.Windows.Forms.OpenFileDialog();
            this.pcgenOutputSheet = new System.Windows.Forms.SaveFileDialog();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.manageModulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.manageModulesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.installPCGenSheetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseLocationToInstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportConversionSheetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3.SuspendLayout();
            this.consoleBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PCGenMenuItem
            // 
            this.PCGenMenuItem.Name = "PCGenMenuItem";
            this.PCGenMenuItem.Size = new System.Drawing.Size(134, 20);
            this.PCGenMenuItem.Text = "Install PCGen Support";
            // 
            // createLocalXslt
            // 
            this.createLocalXslt.Name = "createLocalXslt";
            this.createLocalXslt.Size = new System.Drawing.Size(163, 20);
            this.createLocalXslt.Text = "Export Conversion XSLT Files";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.groupBox3.Controls.Add(this.outputOptions);
            this.groupBox3.Location = new System.Drawing.Point(16, 378);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(769, 148);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output Options";
            // 
            // outputOptions
            // 
            this.outputOptions.BackColor = System.Drawing.Color.White;
            this.outputOptions.CheckOnClick = true;
            this.outputOptions.Cursor = System.Windows.Forms.Cursors.Default;
            this.outputOptions.Location = new System.Drawing.Point(9, 23);
            this.outputOptions.Margin = new System.Windows.Forms.Padding(4);
            this.outputOptions.Name = "outputOptions";
            this.outputOptions.Size = new System.Drawing.Size(748, 106);
            this.outputOptions.TabIndex = 9;
            // 
            // consoleBox
            // 
            this.consoleBox.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.consoleBox.Controls.Add(this.ClearButton);
            this.consoleBox.Controls.Add(this.console);
            this.consoleBox.Controls.Add(this.SaveToTextButton);
            this.consoleBox.Controls.Add(this.clipboardButton);
            this.consoleBox.Location = new System.Drawing.Point(15, 577);
            this.consoleBox.Margin = new System.Windows.Forms.Padding(4);
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.Padding = new System.Windows.Forms.Padding(4);
            this.consoleBox.Size = new System.Drawing.Size(769, 225);
            this.consoleBox.TabIndex = 18;
            this.consoleBox.TabStop = false;
            this.consoleBox.Text = "Console";
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(563, 185);
            this.ClearButton.Margin = new System.Windows.Forms.Padding(4);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(189, 28);
            this.ClearButton.TabIndex = 13;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            // 
            // console
            // 
            this.console.Location = new System.Drawing.Point(11, 23);
            this.console.Margin = new System.Windows.Forms.Padding(4);
            this.console.Name = "console";
            this.console.ReadOnly = true;
            this.console.Size = new System.Drawing.Size(748, 154);
            this.console.TabIndex = 9;
            this.console.Text = "";
            // 
            // SaveToTextButton
            // 
            this.SaveToTextButton.Location = new System.Drawing.Point(284, 185);
            this.SaveToTextButton.Margin = new System.Windows.Forms.Padding(4);
            this.SaveToTextButton.Name = "SaveToTextButton";
            this.SaveToTextButton.Size = new System.Drawing.Size(189, 28);
            this.SaveToTextButton.TabIndex = 12;
            this.SaveToTextButton.Text = "Save to Text File";
            this.SaveToTextButton.UseVisualStyleBackColor = true;
            this.SaveToTextButton.Click += new System.EventHandler(this.SaveToTextButton_Click);
            // 
            // clipboardButton
            // 
            this.clipboardButton.Location = new System.Drawing.Point(11, 185);
            this.clipboardButton.Margin = new System.Windows.Forms.Padding(4);
            this.clipboardButton.Name = "clipboardButton";
            this.clipboardButton.Size = new System.Drawing.Size(189, 28);
            this.clipboardButton.TabIndex = 11;
            this.clipboardButton.Text = "Copy To Clipboard";
            this.clipboardButton.UseVisualStyleBackColor = true;
            this.clipboardButton.Click += new System.EventHandler(this.ClipboardButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.groupBox2.Controls.Add(this.RefreshCampaignsButton);
            this.groupBox2.Controls.Add(this.campaignList);
            this.groupBox2.Controls.Add(this.CampaignCheckBox);
            this.groupBox2.Controls.Add(this.LocalCheckBox);
            this.groupBox2.Controls.Add(this.XmlCheckBox);
            this.groupBox2.Controls.Add(this.FGPath);
            this.groupBox2.Controls.Add(this.AppDataButton);
            this.groupBox2.Controls.Add(this.XmlLocationButton);
            this.groupBox2.Controls.Add(this.XmlOutputLocation);
            this.groupBox2.Location = new System.Drawing.Point(16, 164);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(769, 207);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output";
            // 
            // RefreshCampaignsButton
            // 
            this.RefreshCampaignsButton.Location = new System.Drawing.Point(197, 118);
            this.RefreshCampaignsButton.Margin = new System.Windows.Forms.Padding(4);
            this.RefreshCampaignsButton.Name = "RefreshCampaignsButton";
            this.RefreshCampaignsButton.Size = new System.Drawing.Size(179, 26);
            this.RefreshCampaignsButton.TabIndex = 10;
            this.RefreshCampaignsButton.Text = "Refresh Campaigns";
            this.RefreshCampaignsButton.UseVisualStyleBackColor = true;
            this.RefreshCampaignsButton.Visible = false;
            this.RefreshCampaignsButton.Click += new System.EventHandler(this.RefreshCampaignsButton_Click);
            // 
            // campaignList
            // 
            this.campaignList.BackColor = System.Drawing.Color.White;
            this.campaignList.CheckOnClick = true;
            this.campaignList.Cursor = System.Windows.Forms.Cursors.Default;
            this.campaignList.FormattingEnabled = true;
            this.campaignList.Location = new System.Drawing.Point(384, 118);
            this.campaignList.Margin = new System.Windows.Forms.Padding(4);
            this.campaignList.Name = "campaignList";
            this.campaignList.Size = new System.Drawing.Size(373, 72);
            this.campaignList.TabIndex = 9;
            this.campaignList.Visible = false;
            // 
            // CampaignCheckBox
            // 
            this.CampaignCheckBox.AutoSize = true;
            this.CampaignCheckBox.Location = new System.Drawing.Point(11, 123);
            this.CampaignCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.CampaignCheckBox.Name = "CampaignCheckBox";
            this.CampaignCheckBox.Size = new System.Drawing.Size(136, 21);
            this.CampaignCheckBox.TabIndex = 8;
            this.CampaignCheckBox.Text = "Campaign Import";
            this.CampaignCheckBox.UseVisualStyleBackColor = true;
            this.CampaignCheckBox.Visible = false;
            this.CampaignCheckBox.CheckedChanged += new System.EventHandler(this.CampaignCheckBox_CheckedChanged);
            // 
            // LocalCheckBox
            // 
            this.LocalCheckBox.AutoSize = true;
            this.LocalCheckBox.Location = new System.Drawing.Point(11, 76);
            this.LocalCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.LocalCheckBox.Name = "LocalCheckBox";
            this.LocalCheckBox.Size = new System.Drawing.Size(107, 21);
            this.LocalCheckBox.TabIndex = 6;
            this.LocalCheckBox.Text = "Local Import";
            this.LocalCheckBox.UseVisualStyleBackColor = true;
            this.LocalCheckBox.Visible = false;
            this.LocalCheckBox.CheckedChanged += new System.EventHandler(this.LocalCheckBox_CheckedChanged);
            // 
            // XmlCheckBox
            // 
            this.XmlCheckBox.AutoSize = true;
            this.XmlCheckBox.Location = new System.Drawing.Point(11, 27);
            this.XmlCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.XmlCheckBox.Name = "XmlCheckBox";
            this.XmlCheckBox.Size = new System.Drawing.Size(58, 21);
            this.XmlCheckBox.TabIndex = 5;
            this.XmlCheckBox.Text = "XML";
            this.XmlCheckBox.UseVisualStyleBackColor = true;
            this.XmlCheckBox.Visible = false;
            this.XmlCheckBox.CheckedChanged += new System.EventHandler(this.XmlCheckBox_CheckedChanged);
            // 
            // FGPath
            // 
            this.FGPath.Location = new System.Drawing.Point(384, 74);
            this.FGPath.Margin = new System.Windows.Forms.Padding(4);
            this.FGPath.Name = "FGPath";
            this.FGPath.ReadOnly = true;
            this.FGPath.Size = new System.Drawing.Size(375, 22);
            this.FGPath.TabIndex = 5;
            this.FGPath.Visible = false;
            // 
            // AppDataButton
            // 
            this.AppDataButton.Location = new System.Drawing.Point(197, 73);
            this.AppDataButton.Margin = new System.Windows.Forms.Padding(4);
            this.AppDataButton.Name = "AppDataButton";
            this.AppDataButton.Size = new System.Drawing.Size(179, 26);
            this.AppDataButton.TabIndex = 6;
            this.AppDataButton.Text = "Select AppData Dir";
            this.AppDataButton.UseVisualStyleBackColor = true;
            this.AppDataButton.Visible = false;
            this.AppDataButton.Click += new System.EventHandler(this.AppDataButton_Click);
            // 
            // XmlLocationButton
            // 
            this.XmlLocationButton.Location = new System.Drawing.Point(197, 26);
            this.XmlLocationButton.Margin = new System.Windows.Forms.Padding(4);
            this.XmlLocationButton.Name = "XmlLocationButton";
            this.XmlLocationButton.Size = new System.Drawing.Size(179, 27);
            this.XmlLocationButton.TabIndex = 2;
            this.XmlLocationButton.Text = "Select XML Location";
            this.XmlLocationButton.UseVisualStyleBackColor = true;
            this.XmlLocationButton.Visible = false;
            this.XmlLocationButton.Click += new System.EventHandler(this.XmlLocationButton_Click);
            // 
            // XmlOutputLocation
            // 
            this.XmlOutputLocation.Location = new System.Drawing.Point(384, 28);
            this.XmlOutputLocation.Margin = new System.Windows.Forms.Padding(4);
            this.XmlOutputLocation.Name = "XmlOutputLocation";
            this.XmlOutputLocation.ReadOnly = true;
            this.XmlOutputLocation.Size = new System.Drawing.Size(375, 22);
            this.XmlOutputLocation.TabIndex = 4;
            this.XmlOutputLocation.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.groupBox1.Controls.Add(this.SourceBox);
            this.groupBox1.Controls.Add(this.RulesetBox);
            this.groupBox1.Controls.Add(this.FileButton);
            this.groupBox1.Controls.Add(this.importLocation);
            this.groupBox1.Location = new System.Drawing.Point(16, 44);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(769, 112);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input";
            // 
            // SourceBox
            // 
            this.SourceBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SourceBox.FormattingEnabled = true;
            this.SourceBox.Location = new System.Drawing.Point(288, 28);
            this.SourceBox.Margin = new System.Windows.Forms.Padding(4);
            this.SourceBox.Name = "SourceBox";
            this.SourceBox.Size = new System.Drawing.Size(375, 24);
            this.SourceBox.TabIndex = 14;
            this.SourceBox.SelectedIndexChanged += new System.EventHandler(this.SourceBox_SelectedIndexChanged);
            // 
            // RulesetBox
            // 
            this.RulesetBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RulesetBox.FormattingEnabled = true;
            this.RulesetBox.Location = new System.Drawing.Point(100, 28);
            this.RulesetBox.Margin = new System.Windows.Forms.Padding(4);
            this.RulesetBox.Name = "RulesetBox";
            this.RulesetBox.Size = new System.Drawing.Size(177, 24);
            this.RulesetBox.TabIndex = 13;
            this.RulesetBox.SelectedIndexChanged += new System.EventHandler(this.RulesetBox_SelectedIndexChanged);
            // 
            // FileButton
            // 
            this.FileButton.Location = new System.Drawing.Point(100, 65);
            this.FileButton.Margin = new System.Windows.Forms.Padding(4);
            this.FileButton.Name = "FileButton";
            this.FileButton.Size = new System.Drawing.Size(179, 27);
            this.FileButton.TabIndex = 0;
            this.FileButton.Text = "Select File";
            this.FileButton.UseVisualStyleBackColor = true;
            this.FileButton.Visible = false;
            this.FileButton.Click += new System.EventHandler(this.FileButton_Click);
            // 
            // importLocation
            // 
            this.importLocation.Location = new System.Drawing.Point(288, 66);
            this.importLocation.Margin = new System.Windows.Forms.Padding(4);
            this.importLocation.Name = "importLocation";
            this.importLocation.ReadOnly = true;
            this.importLocation.Size = new System.Drawing.Size(375, 22);
            this.importLocation.TabIndex = 1;
            this.importLocation.Visible = false;
            // 
            // ConvertButton
            // 
            this.ConvertButton.Location = new System.Drawing.Point(277, 534);
            this.ConvertButton.Margin = new System.Windows.Forms.Padding(4);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(240, 37);
            this.ConvertButton.TabIndex = 15;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.UseVisualStyleBackColor = true;
            this.ConvertButton.Click += new System.EventHandler(this.ConvertButton_Click);
            // 
            // xslDialog
            // 
            this.xslDialog.FileName = "openFileDialog1";
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.VersionLabel.Location = new System.Drawing.Point(740, 6);
            this.VersionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(53, 17);
            this.VersionLabel.TabIndex = 20;
            this.VersionLabel.Text = "2.8.0.B";
            // 
            // manageModulesToolStripMenuItem
            // 
            this.manageModulesToolStripMenuItem.Name = "manageModulesToolStripMenuItem";
            this.manageModulesToolStripMenuItem.Size = new System.Drawing.Size(111, 20);
            this.manageModulesToolStripMenuItem.Text = "Manage Modules";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageModulesToolStripMenuItem1,
            this.installPCGenSheetsToolStripMenuItem,
            this.exportConversionSheetsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(800, 36);
            this.menuStrip1.TabIndex = 21;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // manageModulesToolStripMenuItem1
            // 
            this.manageModulesToolStripMenuItem1.Name = "manageModulesToolStripMenuItem1";
            this.manageModulesToolStripMenuItem1.Size = new System.Drawing.Size(176, 32);
            this.manageModulesToolStripMenuItem1.Text = "Manage Modules";
            this.manageModulesToolStripMenuItem1.Click += new System.EventHandler(this.manageModulesToolStripMenuItem1_Click_1);
            // 
            // installPCGenSheetsToolStripMenuItem
            // 
            this.installPCGenSheetsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installToDefaultToolStripMenuItem,
            this.chooseLocationToInstallToolStripMenuItem,
            this.copyToClipboardToolStripMenuItem});
            this.installPCGenSheetsToolStripMenuItem.Name = "installPCGenSheetsToolStripMenuItem";
            this.installPCGenSheetsToolStripMenuItem.Size = new System.Drawing.Size(144, 32);
            this.installPCGenSheetsToolStripMenuItem.Text = "PCGen Sheets";
            // 
            // installToDefaultToolStripMenuItem
            // 
            this.installToDefaultToolStripMenuItem.Name = "installToDefaultToolStripMenuItem";
            this.installToDefaultToolStripMenuItem.Size = new System.Drawing.Size(309, 32);
            this.installToDefaultToolStripMenuItem.Text = "Install to Default Location";
            this.installToDefaultToolStripMenuItem.Click += new System.EventHandler(this.installToDefaultToolStripMenuItem_Click);
            // 
            // chooseLocationToInstallToolStripMenuItem
            // 
            this.chooseLocationToInstallToolStripMenuItem.Name = "chooseLocationToInstallToolStripMenuItem";
            this.chooseLocationToInstallToolStripMenuItem.Size = new System.Drawing.Size(309, 32);
            this.chooseLocationToInstallToolStripMenuItem.Text = "Choose Location to Install";
            this.chooseLocationToInstallToolStripMenuItem.Click += new System.EventHandler(this.chooseLocationToInstallToolStripMenuItem_Click);
            // 
            // copyToClipboardToolStripMenuItem
            // 
            this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            this.copyToClipboardToolStripMenuItem.Size = new System.Drawing.Size(309, 32);
            this.copyToClipboardToolStripMenuItem.Text = "Copy to Clipboard";
            this.copyToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyToClipboardToolStripMenuItem_Click);
            // 
            // exportConversionSheetsToolStripMenuItem
            // 
            this.exportConversionSheetsToolStripMenuItem.Name = "exportConversionSheetsToolStripMenuItem";
            this.exportConversionSheetsToolStripMenuItem.Size = new System.Drawing.Size(246, 32);
            this.exportConversionSheetsToolStripMenuItem.Text = "Export Conversion Sheets";
            this.exportConversionSheetsToolStripMenuItem.Click += new System.EventHandler(this.exportConversionSheetsToolStripMenuItem_Click);
            // 
            // CharConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.BurlyWood;
            this.ClientSize = new System.Drawing.Size(800, 815);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.consoleBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ConvertButton);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CharConverter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fantasy Grounds - Character Converter";
            this.groupBox3.ResumeLayout(false);
            this.consoleBox.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem PCGenMenuItem;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox consoleBox;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.RichTextBox console;
        private System.Windows.Forms.Button SaveToTextButton;
        private System.Windows.Forms.Button clipboardButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button RefreshCampaignsButton;
        private System.Windows.Forms.CheckedListBox campaignList;
        private System.Windows.Forms.CheckBox CampaignCheckBox;
        private System.Windows.Forms.CheckBox LocalCheckBox;
        private System.Windows.Forms.CheckBox XmlCheckBox;
        private System.Windows.Forms.TextBox FGPath;
        private System.Windows.Forms.Button AppDataButton;
        private System.Windows.Forms.Button XmlLocationButton;
        private System.Windows.Forms.TextBox XmlOutputLocation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox SourceBox;
        private System.Windows.Forms.ComboBox RulesetBox;
        private System.Windows.Forms.Button FileButton;
        private System.Windows.Forms.TextBox importLocation;
        private System.Windows.Forms.OpenFileDialog importDialog;
        private System.Windows.Forms.SaveFileDialog xmlOutputDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveToTxtFileDialog;
        private System.Windows.Forms.FolderBrowserDialog appDataDialog;
        private System.Windows.Forms.OpenFileDialog xslDialog;
        private System.Windows.Forms.SaveFileDialog pcgenOutputSheet;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Button ConvertButton;
        private System.Windows.Forms.ToolStripMenuItem createLocalXslt;
        private System.Windows.Forms.ToolStripMenuItem manageModulesToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem manageModulesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem installPCGenSheetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportConversionSheetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installToDefaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseLocationToInstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        public System.Windows.Forms.CheckedListBox outputOptions;
    }
}

