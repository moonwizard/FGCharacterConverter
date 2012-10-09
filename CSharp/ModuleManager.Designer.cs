// Copyright (c) 2012, SmiteWorks USA LLC

namespace CharacterConverter
{
    partial class ModuleManager
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
            this.ModuleList = new System.Windows.Forms.CheckedListBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.SaveChangesButton = new System.Windows.Forms.Button();
            this.RejectChangesButton = new System.Windows.Forms.Button();
            this.RestoreDefaultsButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ModuleList
            // 
            this.ModuleList.FormattingEnabled = true;
            this.ModuleList.Location = new System.Drawing.Point(22, 8);
            this.ModuleList.Name = "ModuleList";
            this.ModuleList.Size = new System.Drawing.Size(213, 184);
            this.ModuleList.TabIndex = 0;
            this.ModuleList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ModuleList_ItemCheck);
            this.ModuleList.SelectedIndexChanged += new System.EventHandler(this.ModuleList_SelectedIndexChanged);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(244, 8);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(103, 22);
            this.AddButton.TabIndex = 1;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // EditButton
            // 
            this.EditButton.Enabled = false;
            this.EditButton.Location = new System.Drawing.Point(244, 65);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(103, 22);
            this.EditButton.TabIndex = 2;
            this.EditButton.Text = "Edit";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Enabled = false;
            this.RemoveButton.Location = new System.Drawing.Point(244, 37);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(103, 22);
            this.RemoveButton.TabIndex = 3;
            this.RemoveButton.Text = "Remove";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // SaveChangesButton
            // 
            this.SaveChangesButton.Location = new System.Drawing.Point(13, 223);
            this.SaveChangesButton.Name = "SaveChangesButton";
            this.SaveChangesButton.Size = new System.Drawing.Size(103, 22);
            this.SaveChangesButton.TabIndex = 4;
            this.SaveChangesButton.Text = "Save Changes";
            this.SaveChangesButton.UseVisualStyleBackColor = true;
            this.SaveChangesButton.Click += new System.EventHandler(this.SaveChangesButton_Click);
            // 
            // RejectChangesButton
            // 
            this.RejectChangesButton.Location = new System.Drawing.Point(127, 223);
            this.RejectChangesButton.Name = "RejectChangesButton";
            this.RejectChangesButton.Size = new System.Drawing.Size(103, 22);
            this.RejectChangesButton.TabIndex = 5;
            this.RejectChangesButton.Text = "Reject Changes";
            this.RejectChangesButton.UseVisualStyleBackColor = true;
            this.RejectChangesButton.Click += new System.EventHandler(this.RejectChangesButton_Click);
            // 
            // RestoreDefaultsButton
            // 
            this.RestoreDefaultsButton.Location = new System.Drawing.Point(241, 223);
            this.RestoreDefaultsButton.Name = "RestoreDefaultsButton";
            this.RestoreDefaultsButton.Size = new System.Drawing.Size(103, 22);
            this.RestoreDefaultsButton.TabIndex = 6;
            this.RestoreDefaultsButton.Text = "Restore Defaults";
            this.RestoreDefaultsButton.UseVisualStyleBackColor = true;
            this.RestoreDefaultsButton.Click += new System.EventHandler(this.RestoreDefaultsButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(244, 94);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(103, 22);
            this.ClearButton.TabIndex = 7;
            this.ClearButton.Text = "Clear All";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.NavajoWhite;
            this.panel1.Controls.Add(this.ModuleList);
            this.panel1.Controls.Add(this.ClearButton);
            this.panel1.Controls.Add(this.AddButton);
            this.panel1.Controls.Add(this.EditButton);
            this.panel1.Controls.Add(this.RemoveButton);
            this.panel1.Location = new System.Drawing.Point(-1, -3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(452, 213);
            this.panel1.TabIndex = 8;
            // 
            // ModuleManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.BurlyWood;
            this.ClientSize = new System.Drawing.Size(358, 258);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.RestoreDefaultsButton);
            this.Controls.Add(this.RejectChangesButton);
            this.Controls.Add(this.SaveChangesButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModuleManager";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Module Manager";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox ModuleList;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button EditButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button SaveChangesButton;
        private System.Windows.Forms.Button RejectChangesButton;
        private System.Windows.Forms.Button RestoreDefaultsButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Panel panel1;
    }
}