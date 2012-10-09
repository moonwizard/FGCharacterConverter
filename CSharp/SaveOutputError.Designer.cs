// Copyright (c) 2012, SmiteWorks USA LLC

namespace CharacterConverter
{
    partial class SaveOutputError
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveOutputError));
            this.panel1 = new System.Windows.Forms.Panel();
            this.message = new System.Windows.Forms.RichTextBox();
            this.SaveOutputButton = new System.Windows.Forms.Button();
            this.ReturnButton = new System.Windows.Forms.Button();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.NavajoWhite;
            this.panel1.Controls.Add(this.message);
            this.panel1.Location = new System.Drawing.Point(-1, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(471, 214);
            this.panel1.TabIndex = 5;
            // 
            // message
            // 
            this.message.Location = new System.Drawing.Point(16, 14);
            this.message.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(432, 180);
            this.message.TabIndex = 1;
            this.message.Text = resources.GetString("message.Text");
            // 
            // SaveOutputButton
            // 
            this.SaveOutputButton.Location = new System.Drawing.Point(15, 222);
            this.SaveOutputButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SaveOutputButton.Name = "SaveOutputButton";
            this.SaveOutputButton.Size = new System.Drawing.Size(177, 28);
            this.SaveOutputButton.TabIndex = 4;
            this.SaveOutputButton.Text = "Save Output Sheets";
            this.SaveOutputButton.UseVisualStyleBackColor = true;
            this.SaveOutputButton.Click += new System.EventHandler(this.SaveOutputButton_Click);
            // 
            // ReturnButton
            // 
            this.ReturnButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ReturnButton.Location = new System.Drawing.Point(271, 222);
            this.ReturnButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ReturnButton.Name = "ReturnButton";
            this.ReturnButton.Size = new System.Drawing.Size(177, 28);
            this.ReturnButton.TabIndex = 3;
            this.ReturnButton.Text = "Return to Menu";
            this.ReturnButton.UseVisualStyleBackColor = true;
            this.ReturnButton.Click += new System.EventHandler(this.ReturnButton_Click);
            // 
            // SaveOutputError
            // 
            this.AcceptButton = this.SaveOutputButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.BurlyWood;
            this.CancelButton = this.ReturnButton;
            this.ClientSize = new System.Drawing.Size(464, 258);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SaveOutputButton);
            this.Controls.Add(this.ReturnButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveOutputError";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Message";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox message;
        private System.Windows.Forms.Button SaveOutputButton;
        private System.Windows.Forms.Button ReturnButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
    }
}