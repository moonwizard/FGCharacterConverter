using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Ionic.Zip;

namespace CharacterConverter
{
    public partial class ModuleManager : Form
    {
        //Assumed fields
        private const String RefPath = "root/reference/";
        //public const String Races = "Races";
        //public const String Classes = "Classes";
        public const String Skills = "Skills";
        public const String Weapons = "Weapons";
        public const String Armor = "Armor";
        public const String Equipment = "Equipment";
        public const String Feats = "Feats";
        public const String Spells = "Spells";
        public const String MagicItems = "Magic Items";
        public const String Hindrances = "Hindrances";
        public const String Edges = "Edges";
        public const String Powers = "Powers";
        public const String SWWeapons = "SW Weapons";
        public const String SWArmor = "SW Armor";
        public const String SWSkills = "SW Skills";
        public const String Gear = "Gear";



        public static readonly Dictionary<String, String> Paths = new Dictionary<string, string>()
            {
                //{Races, RefPath + "races"},
                //{Classes, RefPath + "classes"},
                {Skills, RefPath + "skills"},
                {Weapons, RefPath + "weapon"},
                {Armor, RefPath + "armor"},
                {Equipment, RefPath + "equipment"},
                {Feats, RefPath + "feats"},
                {MagicItems, RefPath + "magicitems"},
                {Spells, "root/spelldesc"},
                {SWSkills, RefPath + "skills"},
                {SWWeapons, RefPath + "weapons"},
                {SWArmor, RefPath + "armor"},
                {Gear, RefPath + "mundaneitems"},
                {Hindrances, RefPath + "hindrances"},
                {Edges, RefPath + "edges"},
                {Powers, "root/powerdesc"}
            };

        public static readonly Dictionary<String, String[]> Types = new Dictionary<string,string[]>()
            {
                { RS_PF.Name, new [] {
                Skills,
                Weapons,
                Armor,
                Equipment,
                Feats,
                Spells,
                MagicItems
                    }
                },
                { RS_35E.Name, new [] {
                Skills,
                Weapons,
                Armor,
                Equipment,
                Feats,
                Spells,
                MagicItems
                    }
                },
                { RS_SavageWorlds3.Name, new [] {
                SWSkills,
                SWWeapons,
                SWArmor,
                Gear,
                Hindrances,
                Edges,
                Powers
                    }
                }
            };
 /*       public static readonly String[] Types = new []
            {
                //Races,
                //Classes,
                Skills,
                Weapons,
                Armor,
                Equipment,
                Feats,
                Spells,
                MagicItems,
                SWSkills,
                SWWeapons,
                SWArmor,
                Gear,
                Hindrances,
                Edges,
                Powers
            };
 */
        public static List<Dictionary<String, String>> DefaultEntries
        {
            get
            {
                return LoadSettings(CharConverter.DefaultSettings);
            }
        }
        public static Dictionary<String, String> EmptyModuleEntry
        {
            get
            {
                return new Dictionary<string, string>()
                           {
                               {"isloaded", ""},
                               {"name", ""},
                               {"ruleset", ""},
                               {"type", ""},
                               {"path", ""}
                           };
            }
        }
            
        //Settings interface
        public const String ModKey = "Mod";
        public const String SplittingCharacter = "|";
        public static String ModSettingsEntryFormat
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(ModKey).Append(SplittingCharacter);
                sb.Append("isloaded=ISLOADED").Append(SplittingCharacter);
                sb.Append("name=NAME").Append(SplittingCharacter);
                sb.Append("ruleset=RULESET").Append(SplittingCharacter);
                sb.Append("type=TYPE").Append(SplittingCharacter);
                sb.Append("path=PATH");
                return sb.ToString();
            }
        }

        private List<Dictionary<String, String>> tempModuleEntries;
        public List<Dictionary<String, String>> ModuleEntries;
        public List<String> ModuleNames
        {
            get
            {
                var modnames = new List<String>();
                foreach (var moddict in tempModuleEntries)
                {
                    modnames.Add(moddict["name"]);
                }
                return modnames;
            }
        }

        public bool Initalizing = false;

        public ModuleManager(List<String> settings)
        {
            Initalizing = true;
            InitializeComponent();
            ModuleEntries = LoadSettings(settings);
            tempModuleEntries = LoadSettings(settings);
            RefreshUi(true);
            UpdateCheckedStatus();
            Initalizing = false;
        }

        public void RefreshUi(bool initial = false)
        {
            //If initializing do not update the checked status to file
            if(!initial)
                UpdateCheckedStatus();

            //Loops through the settings file to check the box next to a module
            //if "isloaded" is true
            int modindex = ModuleList.SelectedIndex;
            ModuleList.Items.Clear();
            foreach (var moddict in tempModuleEntries)
            {
                ModuleList.Items.Add(moddict["name"]);
                if (moddict["isloaded"].Equals("true"))
                    ModuleList.SetItemChecked(ModuleList.Items.Count - 1, true);
            }
            if (modindex < tempModuleEntries.Count)
                ModuleList.SelectedIndex = modindex;

            //Refreshs the buttons.
            RefreshButtons();
            ClearButton.Enabled = ModuleList.Items.Count != 0;
        }

        public void UpdateCheckedStatus()
        {
            //Writes the "isloaded" status of the modules to file
            for (int i = 0; i < tempModuleEntries.Count; i++)
                tempModuleEntries[i]["isloaded"] = ModuleList.GetItemChecked(i) ? "true" : "false";
        }

        public static List<Dictionary<String, String>> LoadSettings(List<String> settings)
        {
            var modEntires = new List<Dictionary<String, String>>();
            //Loops through each line of the settings file for a module entry
            foreach (String line in settings)
            {
                //If a module entry is found parse and load the entry in to a dictionary
                if (line.StartsWith(ModKey))
                {
                    String[] modentry = line.Split(SplittingCharacter.ToCharArray());
                    var moddict = new Dictionary<string, string>
                    {
                        {"isloaded", modentry[1].Replace("isloaded=", "")},
                        {"name", modentry[2].Replace("name=", "")}, 
                        {"ruleset", modentry[3].Replace("ruleset=", "")}, 
                        {"type", modentry[4].Replace("type=", "")},
                        {"path", modentry[5].Replace("path=", "")}
                    };
                    modEntires.Add(moddict);
                }
            }
            return modEntires;
        }

        public void SaveSettings(ref List<String> settings)
        {
            var tempModule = new List<Dictionary<string, string>>(tempModuleEntries);

            //Remove all mod entires
            var tempSettings = new List<string>();
            for(int i=settings.Count-1; i >= 0; i--)
            {
                String line = settings[i];
                if (!line.StartsWith(ModKey))
                    tempSettings.Add(line);
            }

            //Add remaining module entires to settings
            foreach(var dict in tempModule)
                tempSettings.Add(ConvertDictionaryToEntry(dict));

            settings = tempSettings;
        }

        public static String ConvertDictionaryToEntry(Dictionary<String, String> moddict)
        {
            //Converts a dictionary into an entry using the default module entry string
            String entry = ModSettingsEntryFormat;
            entry = entry.Replace("ISLOADED", moddict["isloaded"]);
            entry = entry.Replace("NAME", moddict["name"]);
            entry = entry.Replace("RULESET", moddict["ruleset"]);
            entry = entry.Replace("TYPE", moddict["type"]);
            entry = entry.Replace("PATH", moddict["path"]);
            return entry;
        }

        public List<XmlDocument> RetrieveModules(String ruleset, String type)
        {
            //Loads a list of XmlDocuments using the "path" from the module entries
            List<Dictionary<String, String>> dicts = FilterDictionaries("true", null, ruleset, type, null);
            var xmlDocuments = new List<XmlDocument>();
            foreach(var dict in dicts)
            {
                String name = dict["name"];
                try
                {
                    xmlDocuments.Add(RetrieveXmlFromModule(dict["path"]));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load module " + name + Environment.NewLine + ex.Message);
                    continue;
                }
                
            }
            return xmlDocuments;
        }
        
        public XmlDocument RetrieveXmlFromModule(String path)
        {
            //Retrieves the xml database from a module using the three valid names for
            //said database
            var temp = new XmlDocument();
            using (var mod = new ZipFile(path))
            {
                if (Directory.Exists(CharConverter.CCDir + "\\temp"))
                    Directory.Delete(CharConverter.CCDir + "\\temp", true);

                Directory.CreateDirectory(CharConverter.CCDir + "\\temp");
                mod.ExtractAll(CharConverter.CCDir + "\\temp");
                if (File.Exists(CharConverter.CCDir + "\\temp\\client.xml"))
                    temp.Load(CharConverter.CCDir + "\\temp\\client.xml");
                if (File.Exists(CharConverter.CCDir + "\\temp\\common.xml"))
                    temp.Load(CharConverter.CCDir + "\\temp\\common.xml");
                else if (File.Exists(CharConverter.CCDir + "\\temp\\db.xml"))
                    temp.Load(CharConverter.CCDir + "\\temp\\db.xml");
                else
                    throw new Exception("No xml database found");

                Directory.Delete(CharConverter.CCDir + "\\temp", true);
            }
            return temp;
        }

        public List<Dictionary<String, String>> FilterDictionaries(String isloaded, String name, String ruleset, String type, String path)
        {
            //Filters the module entries for certain paramaters 
            bool isloadedcheck = !String.IsNullOrEmpty(isloaded);
            bool namecheck = !String.IsNullOrEmpty(name);
            bool rulesetcheck = !String.IsNullOrEmpty(ruleset);
            bool typecheck = !String.IsNullOrEmpty(type);
            bool pathcheck = !String.IsNullOrEmpty(path);

            var filterdict = new List<Dictionary<String, String>>();
            foreach(var moddict in tempModuleEntries)
            {
                if (isloadedcheck && !isloaded.Equals(moddict["isloaded"]))
                    continue;
                if (rulesetcheck && !ruleset.Equals(moddict["ruleset"]))
                    continue;
                if (typecheck && !moddict["type"].Contains(type))
                    continue;
                if (pathcheck && !path.Equals(moddict["path"]))
                    continue;
                if (namecheck && !name.Equals(moddict["name"]))
                    continue;

                filterdict.Add(moddict);
            }
            return filterdict;
        }

        private void RestoreDefaultsButton_Click(object sender, EventArgs e)
        {
            tempModuleEntries = DefaultEntries;
            RefreshUi(true);
            UpdateCheckedStatus();
        }

        private void SaveChangesButton_Click(object sender, EventArgs e)
        {
            UpdateCheckedStatus();   
            ModuleEntries = tempModuleEntries;
            Close();
        }

        private void RejectChangesButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            tempModuleEntries.Clear();
            ModuleList.Items.Clear();
            RefreshUi();
        }

        private void RefreshButtons()
        {
            EditButton.Enabled = ModuleList.SelectedIndex < tempModuleEntries.Count && ModuleList.SelectedIndex >= 0;
            RemoveButton.Enabled = EditButton.Enabled;
        }

        private void ModuleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            using(var modEditor = new ModuleEntryEditor(EmptyModuleEntry))
            {
                modEditor.ShowDialog();
                if(modEditor.EditedModule != null)
                {
                    tempModuleEntries.Add(modEditor.EditedModule);
                    ModuleList.Items.Add(modEditor.EditedModule["name"]);
                }  
            }
            RefreshUi();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            using (var modEditor = new ModuleEntryEditor(tempModuleEntries[ModuleList.SelectedIndex]))
            {
                modEditor.ShowDialog();
                if (modEditor.EditedModule != null)
                    tempModuleEntries[ModuleList.SelectedIndex] = modEditor.EditedModule;
            }
            RefreshUi();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            UpdateCheckedStatus();
            tempModuleEntries.RemoveAt(ModuleList.SelectedIndex);
            RefreshUi();
        }

        private void ModuleList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //validates the module entry when the user attempts to set it as loaded
            if(e.NewValue == CheckState.Checked)
            {
                try
                {
                    String path = tempModuleEntries[e.Index]["path"];
                    XmlDocument temp = RetrieveXmlFromModule(path);
                }
                catch (Exception exception)
                {
                    e.NewValue = CheckState.Unchecked;
                    if(!Initalizing)
                        MessageBox.Show(exception.Message);
                }
            }
        }
    }
}
