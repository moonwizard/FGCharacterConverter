// Copyright (c) 2012, SmiteWorks USA LLC

namespace CharacterConverter
{
    partial class ReplaceCharacter
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
            this.CreateButton = new System.Windows.Forms.Button();
            this.ReplaceButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.description = new System.Windows.Forms.Label();
            this.characterList = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CreateButton
            // 
            this.CreateButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CreateButton.Location = new System.Drawing.Point(236, 114);
            this.CreateButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(127, 28);
            this.CreateButton.TabIndex = 6;
            this.CreateButton.Text = "Create New Slot";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // ReplaceButton
            // 
            this.ReplaceButton.Location = new System.Drawing.Point(9, 114);
            this.ReplaceButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ReplaceButton.Name = "ReplaceButton";
            this.ReplaceButton.Size = new System.Drawing.Size(127, 28);
            this.ReplaceButton.TabIndex = 5;
            this.ReplaceButton.Text = "Replace Slot";
            this.ReplaceButton.UseVisualStyleBackColor = true;
            this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.NavajoWhite;
            this.panel1.Controls.Add(this.description);
            this.panel1.Controls.Add(this.characterList);
            this.panel1.Location = new System.Drawing.Point(-9, -12);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(477, 116);
            this.panel1.TabIndex = 4;
            // 
            // description
            // 
            this.description.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.description.Location = new System.Drawing.Point(19, 21);
            this.description.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(357, 33);
            this.description.TabIndex = 1;
            this.description.Text = "Select Character Slot to Override PLACEHOLDERTEXT\r\nPLACEHOLDERTEXT PLACEHOLDERTEX" +
                "T";
            this.description.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // characterList
            // 
            this.characterList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.characterList.FormattingEnabled = true;
            this.characterList.Location = new System.Drawing.Point(19, 69);
            this.characterList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.characterList.Name = "characterList";
            this.characterList.Size = new System.Drawing.Size(352, 24);
            this.characterList.TabIndex = 0;
            // 
            // ReplaceCharacter
            // 
            this.AcceptButton = this.ReplaceButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.BurlyWood;
            this.CancelButton = this.CreateButton;
            this.ClientSize = new System.Drawing.Size(372, 149);
            this.ControlBox = false;
            this.Controls.Add(this.CreateButton);
            this.Controls.Add(this.ReplaceButton);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReplaceCharacter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Replace Existing Character";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Button ReplaceButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label description;
        private System.Windows.Forms.ComboBox characterList;
    }
}