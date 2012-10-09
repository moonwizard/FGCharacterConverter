using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CharacterConverter
{
    public partial class ReplaceCharacter : Form
    {
        public bool ReplaceChar = false;
        public String CharacterName = "";
        public int Index = -1;

        public ReplaceCharacter(String charName, String target, String[] names)
        {
            InitializeComponent();
            description.Text = String.Format("Select a Character Slot for {0} to Override", charName) + Environment.NewLine + target;
			characterList.Items.AddRange(names);
			if(characterList.Items.Contains(charName))
				characterList.SelectedIndex = characterList.Items.IndexOf(charName);
        }

        private void ReplaceButton_Click(object sender, EventArgs e)
        {
             ReplaceChar = true;
			 Index = characterList.SelectedIndex;
			 if(Index == -1)
			 {
				 MessageBox.Show("Please select an existing character.");
				 return;
			 }
			 CharacterName = (String)characterList.SelectedItem;
			 Close();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            ReplaceChar = false;
            Index = -1;
            Close();
        }
    }
}
