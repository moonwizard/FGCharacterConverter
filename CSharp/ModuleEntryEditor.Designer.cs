// Copyright (c) 2012, SmiteWorks USA LLC

namespace CharacterConverter
{
    partial class ModuleEntryEditor
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
            this.OpenModuleDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveChangesButton = new System.Windows.Forms.Button();
            this.RejectChangesButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PathBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TypeBox = new System.Windows.Forms.CheckedListBox();
            this.SelectModuleButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RulesetCBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SaveChangesButton
            // 
            this.SaveChangesButton.Location = new System.Drawing.Point(7, 244);
            this.SaveChangesButton.Name = "SaveChangesButton";
            this.SaveChangesButton.Size = new System.Drawing.Size(156, 22);
            this.SaveChangesButton.TabIndex = 7;
            this.SaveChangesButton.Text = "Save Changes";
            this.SaveChangesButton.UseVisualStyleBackColor = true;
            this.SaveChangesButton.Click += new System.EventHandler(this.SaveChangesButton_Click);
            // 
            // RejectChangesButton
            // 
            this.RejectChangesButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.RejectChangesButton.Location = new System.Drawing.Point(178, 244);
            this.RejectChangesButton.Name = "RejectChangesButton";
            this.RejectChangesButton.Size = new System.Drawing.Size(156, 22);
            this.RejectChangesButton.TabIndex = 8;
            this.RejectChangesButton.Text = "Reject Changes";
            this.RejectChangesButton.UseVisualStyleBackColor = true;
            this.RejectChangesButton.Click += new System.EventHandler(this.RejectChangesButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.NavajoWhite;
            this.panel1.Controls.Add(this.PathBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.TypeBox);
            this.panel1.Controls.Add(this.SelectModuleButton);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.RulesetCBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.NameBox);
            this.panel1.Location = new System.Drawing.Point(-2, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(452, 239);
            this.panel1.TabIndex = 12;
            // 
            // PathBox
            // 
            this.PathBox.Location = new System.Drawing.Point(58, 31);
            this.PathBox.Name = "PathBox";
            this.PathBox.ReadOnly = true;
            this.PathBox.Size = new System.Drawing.Size(277, 20);
            this.PathBox.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Path";
            // 
            // TypeBox
            // 
            this.TypeBox.CheckOnClick = true;
            this.TypeBox.FormattingEnabled = true;
            this.TypeBox.Location = new System.Drawing.Point(58, 112);
            this.TypeBox.Name = "TypeBox";
            this.TypeBox.Size = new System.Drawing.Size(277, 109);
            this.TypeBox.TabIndex = 18;
            // 
            // SelectModuleButton
            // 
            this.SelectModuleButton.Location = new System.Drawing.Point(10, 3);
            this.SelectModuleButton.Name = "SelectModuleButton";
            this.SelectModuleButton.Size = new System.Drawing.Size(326, 22);
            this.SelectModuleButton.TabIndex = 17;
            this.SelectModuleButton.Text = "Select Module";
            this.SelectModuleButton.UseVisualStyleBackColor = true;
            this.SelectModuleButton.Click += new System.EventHandler(this.SelectModuleButton_Click_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Ruleset";
            // 
            // RulesetCBox
            // 
            this.RulesetCBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RulesetCBox.FormattingEnabled = true;
            this.RulesetCBox.Location = new System.Drawing.Point(58, 82);
            this.RulesetCBox.Name = "RulesetCBox";
            this.RulesetCBox.Size = new System.Drawing.Size(277, 21);
            this.RulesetCBox.TabIndex = 14;
            this.RulesetCBox.SelectedIndexChanged += new System.EventHandler(this.RulesetCBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Name";
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(58, 57);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(277, 20);
            this.NameBox.TabIndex = 12;
            // 
            // ModuleEntryEditor
            // 
            this.AcceptButton = this.SaveChangesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.BurlyWood;
            this.CancelButton = this.RejectChangesButton;
            this.ClientSize = new System.Drawing.Size(342, 274);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.RejectChangesButton);
            this.Controls.Add(this.SaveChangesButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModuleEntryEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Module Entry Editor";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog OpenModuleDialog;
        private System.Windows.Forms.Button SaveChangesButton;
        private System.Windows.Forms.Button RejectChangesButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox PathBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox TypeBox;
        private System.Windows.Forms.Button SelectModuleButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox RulesetCBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NameBox;
    }
}