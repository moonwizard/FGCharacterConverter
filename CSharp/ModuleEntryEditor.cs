using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CharacterConverter
{
    public partial class ModuleEntryEditor : Form
    {
        public Dictionary<String, String> EditedModule;
        static readonly String[] Rulesets = new []
                                    {
                                        RS_PF.Name,
                                        RS_35E.Name,
                                        RS_SavageWorlds.Name
                                        //RS_4E.Name
                                    };
        public ModuleEntryEditor(Dictionary<String, String> mod)
        {
            InitializeComponent();
            EditedModule = mod;

            RulesetCBox.Items.AddRange(Rulesets);

            OpenModuleDialog.Filter = "FG Module File (.mod)|*.mod";
            OpenModuleDialog.Title = "Select a Fantasy Grounds Module File";
            OpenModuleDialog.CheckFileExists = true;
            OpenModuleDialog.CheckPathExists = true;
            if(Directory.Exists(CharConverter.Fg2RegistryAppDataPath + "\\modules"))
                OpenModuleDialog.InitialDirectory = CharConverter.Fg2RegistryAppDataPath + "\\modules";

            PathBox.Text = mod["path"];
            NameBox.Text = mod["name"];
            String modruleset = mod["ruleset"];
            for(int i=0; i < Rulesets.Length; i++)
            {
                String ruleset = Rulesets[i];
                if (modruleset.Contains(ruleset))
                    RulesetCBox.SelectedIndex = i;
            }
            if (RulesetCBox.SelectedIndex < 0) {
                RulesetCBox.SelectedIndex = 0;
            }
            String DisplayRuleset = Rulesets[RulesetCBox.SelectedIndex];
            TypeBox.Items.Clear();
            TypeBox.Items.AddRange(ModuleManager.Types[DisplayRuleset]);

            String modtype = mod["type"];
            for(int i = 0; i < TypeBox.Items.Count; i++)
            {
                var type = (String)TypeBox.Items[i];
                if(modtype.Contains(type))
                    TypeBox.SetItemChecked(i, true);
            }
        }

        private void RejectChangesButton_Click(object sender, EventArgs e)
        {
            EditedModule = null;
            Close();
        }

        private void SaveChangesButton_Click(object sender, EventArgs e)
        {
            String error = null;
            if (String.IsNullOrEmpty(PathBox.Text))
                error = "Please select a file";
            else if (String.IsNullOrEmpty(NameBox.Text))
                error = "Please enter a name.";
            else if (String.IsNullOrEmpty((String)RulesetCBox.SelectedItem))
                error = "Please select a ruleset.";   
            else if (TypeBox.CheckedItems.Count == 0)
                error = "Please select at least one type";
            if(!String.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return;
            }
            EditedModule["name"] = NameBox.Text;
            EditedModule["path"] = PathBox.Text;
            EditedModule["ruleset"] = (String)RulesetCBox.SelectedItem;
            var sb = new StringBuilder();
            foreach(String type in TypeBox.CheckedItems)
                sb.Append(type).Append(";");
            EditedModule["type"] = sb.ToString();
            Close();
        }

        private void SelectModuleButton_Click_1(object sender, EventArgs e)
        {
            if (OpenModuleDialog.ShowDialog() == DialogResult.OK)
            {
                PathBox.Text = OpenModuleDialog.FileName;
            }
        }

        private void RulesetCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            String DisplayRuleset = Rulesets[RulesetCBox.SelectedIndex];
            TypeBox.Items.Clear();
            TypeBox.Items.AddRange(ModuleManager.Types[DisplayRuleset]);
        }
    }
}
