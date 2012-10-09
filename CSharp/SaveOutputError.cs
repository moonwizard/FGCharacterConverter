using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace CharacterConverter
{
    public partial class SaveOutputError : Form
    {
        public SaveOutputError()
        {
            InitializeComponent();
        }

        public SaveOutputError(String displayMessage)
        {
            InitializeComponent();
            message.Text = displayMessage;
        }

        private void SaveOutputButton_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PCGen.SaveSheetToDirectory(folderBrowser.SelectedPath);
            Close();
        }

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
