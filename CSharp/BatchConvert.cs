using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CharacterConverter
{
    public partial class BatchConvert : Form
    {
        public BatchConvert(String[] names)
        {
            InitializeComponent();
            Characters.Items.AddRange(names);
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
