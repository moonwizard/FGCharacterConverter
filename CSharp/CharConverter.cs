using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.Win32;

namespace CharacterConverter
{
    public partial class CharConverter : Form
    {
        //Program Details
        //Version = Newest Fantasy Grounds version that this program is compatible with.
        //Revision = Alphabetic. Keeps track of the amount of Converter releases for the FG2 version.
        const String CC_VERSION = "2.9.3";
        const String CC_REVISION = "A";
        const String SupportWebsite = "http://www.fantasygrounds.com/forums/forumdisplay.php?f=41";

        //Directories
        const String CampaignPath = @"\campaigns";
        const String CampaignDBPath = @"\db.xml";
        const String CampaignDBBackupPath = @"\db.import.xml";
        const String CampaignInfoPath = @"\campaign.xml";

        //Output Options: All
        const String ReplaceString = "Replace existing character.";
        const String UseLocalXslt = "Use local xslt for file conversion (if available).";
        private const String UseModuleManager = "Use modules from the module manager to lookup data.";
        static public readonly String[] OutputOptDNDI = new [] { 
            "Add class features as powers", 
            "Add feats as powers", 
            "Add item properties as powers" 
        };
        String SourceFormat = "";
        String InternalRuleset = "";
        public String DisplayRuleset = "";
        String LocalPath = "";
        String LocalBackupPath = "";
        String NamePath = "";
        String CharPath = "";
        String[] CampaignDirectories = new String[0];
        List<String> Settings = new List<String>();
        ModuleManager moduleManager;

        private delegate void ApiDel(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode);
        readonly Dictionary<String, Delegate> Api = new Dictionary<string, Delegate>();

        public static String CCSettingsPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Fantasy Grounds II\utilities\cc-settings.ini"; }
        }
        public static String CCBackupSettingsPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Fantasy Grounds II\utilities\cc-settings-backup.ini"; }
        }
        public static String CCDir
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Fantasy Grounds II\utilities"; }
        }
        public static String FGDataDir
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Fantasy Grounds II"; }
        }
        public static String ExePath
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                String[] exesplit = Application.ExecutablePath.Split('\\');
                for (int i = 0; i < exesplit.Length - 1; i++)
                {
                    sb.Append(exesplit[i]);
                    if (i + 1 < exesplit.Length - 1)
                        sb.Append("\\");
                }
                    
                return sb.ToString();
            }
        }
        public static String XsltPath
        {
            get { return ExePath + @"\Xslt"; }
        }
        public static String Fg2RegistryAppDataPath
        {
            get { return (String)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Fantasy Grounds\\2.0", "DataDir", null); }
        }

        public static readonly List<String> DefaultSettings = new List<string>
        {
            "Mod|isloaded=true|name=3.5E-basicrules.mod|ruleset=3.5E|type=Races;Classes;Skills;Weapons;Armor;Equipment;Feats;|path=" + Fg2RegistryAppDataPath + "\\modules\\3.5E-basicrules.mod",
            "Mod|isloaded=true|name=3.5E-magicitems.mod|ruleset=3.5E|type=Magic Items;|path=" + Fg2RegistryAppDataPath + "\\modules\\3.5E-magicitems.mod",
            "Mod|isloaded=true|name=3.5E-spells.mod|ruleset=3.5E|type=Spells;|path=" + Fg2RegistryAppDataPath + "\\modules\\3.5E-spells.mod",
            "Mod|isloaded=true|name=PF-SRD-Basic-Rules.mod|ruleset=PFRPG|type=Races;Classes;Skills;Weapons;Armor;Equipment;Feats;|path=" + Fg2RegistryAppDataPath + "\\modules\\PF-SRD-Basic-Rules.mod",
            "Mod|isloaded=true|name=PF-SRD-Spells.mod|ruleset=PFRPG|type=Spells;|path=" + Fg2RegistryAppDataPath + "\\modules\\PF-SRD-Spells.mod"
        };

        public CharConverter()
        {
            InitializeComponent();

            // Automatically update PCGen sheets if we find them
            PCGen.CreateSheets(true);

            //Checks for and creates directories that will be used
            if (!Directory.Exists(FGDataDir))
                Directory.CreateDirectory(FGDataDir);
            if (!Directory.Exists(CCDir))
                Directory.CreateDirectory(CCDir);

            //Loads settings and initializes the module manager
            if (File.Exists(CCSettingsPath))
                LoadSettings();
            else
                Settings = DefaultSettings;
            moduleManager = new ModuleManager(Settings);

            //Intializes the file I/O fields
            xmlOutputDialog.Title = "Select XML Output Location";
            xmlOutputDialog.Filter = "XML File (*.xml)|*.xml";
            xslDialog.Title = "Select Custom XSL File";
            xslDialog.Filter = "XSLT File (*.xslt)|*.xslt|XSL File (*.xsl)|*.xsl";
            saveToTxtFileDialog.Title = "Select Console Log Output Location";
            saveToTxtFileDialog.Filter = "Text File (*.txt)|*.txt";
            appDataDialog.Description = "Select Fantasy Grounds App Data Directory";

            //Creates the valid campaign list if the FG2 folder exists
            FGPath.Text = Fg2RegistryAppDataPath;
            if (FGPath.Text.Length == 0)
                MessageBox.Show("FG2 registry error. App Data path must be set manually. Reinstall Fantasy Grounds to fix.");
            else
                AppendCampaigns();

            //Sets up the UI
            VersionLabel.Text = CC_VERSION + "." + CC_REVISION;
            RulesetBox.Items.Add(RS_4E.Name);
            RulesetBox.Items.Add(RS_35E.Name);
            RulesetBox.Items.Add(RS_PF.Name);
            RulesetBox.SelectedIndex = 0;
            
            //Links the api calls with their corresponding functions
            Api.Add("CC.HL.SPELLSET", (ApiDel)HeroLab.processSpellset);
            Api.Add("CC.HL.LANGUAGELIST", (ApiDel)HeroLab.processLanguagelist);
            Api.Add("CC.HL.WEAPONLIST", (ApiDel)HeroLab.processWeaponlist);
            Api.Add("CC.HL.SPEED.SPECIAL", (ApiDel)HeroLab.processSpeedSpecial);
            Api.Add("CC.HL.SENSES", (ApiDel)HeroLab.processSenses);
            Api.Add("CC.HL.DEFENSES.DAMAGEREDUCTION", (ApiDel)HeroLab.processDefensesDamagereduction);
            Api.Add("CC.HL.DEFENSES.SR", (ApiDel)HeroLab.processDefensesSr);
            Api.Add("CC.HL.SKILLLIST", (ApiDel)HeroLab.processSkilllist);
            Api.Add("CC.HL.CLASSES", (ApiDel)HeroLab.processClasses);
            Api.Add("CC.HL.SPECIALABILITYLIST", (ApiDel)HeroLab.processSpecialabilitylist);
            Api.Add("CC.HL.FEATLIST", (ApiDel)HeroLab.processFeatlist);
            Api.Add("CC.HL.PROFICIENCYWEAPON", (ApiDel)HeroLab.processProficiencyweapon);
            Api.Add("CC.HL.PROFICIENCYARMOR", (ApiDel)HeroLab.processProficiencyarmor);
            Api.Add("CC.HL.ATTACKBONUS.GRAPPLE.SIZE", (ApiDel)HeroLab.processAttackbonusGrappleSize);
            Api.Add("CC.HL.ATTACKBONUS.GRAPPLE.TOTAL", (ApiDel)HeroLab.processAttackbonusGrappleTotal);
            Api.Add("CC.HL.ATTACKBONUS.GRAPPLE.MISC", (ApiDel)HeroLab.processAttackbonusGrappleMisc);
            Api.Add("CC.HL.ATTACKBONUS.MELEE.TOTAL", (ApiDel)HeroLab.processAttackbonusMeleeTotal);
            Api.Add("CC.HL.ATTACKBONUS.MELEE.MISC", (ApiDel)HeroLab.processAttackbonusMeleeMisc);
            Api.Add("CC.HL.ATTACKBONUS.RANGED.TOTAL", (ApiDel)HeroLab.processAttackbonusRangedTotal);
            Api.Add("CC.HL.ATTACKBONUS.RANGED.MISC", (ApiDel)HeroLab.processAttackbonusRangedMisc);
            Api.Add("CC.HL.INVENTORYLIST", (ApiDel)HeroLab.processInventorylist);
            Api.Add("CC.HL.SAVES.FORTITUDE.MISC", (ApiDel)HeroLab.processSavesFortitudeMisc);
            Api.Add("CC.HL.SAVES.WILL.MISC", (ApiDel)HeroLab.processSavesWillMisc);
            Api.Add("CC.HL.SAVES.REFLEX.MISC", (ApiDel)HeroLab.processSavesReflexMisc);

            Api.Add("CC.DNDI.CLASS.BASE", (ApiDel)DDI.processClassBase);
            Api.Add("CC.DNDI.CLASS.EPIC", (ApiDel)DDI.processClassEpic);
            Api.Add("CC.DNDI.CLASS.PARAGON", (ApiDel)DDI.processClassParagon);
            Api.Add("CC.DNDI.ENCUMBRANCE.HEAVYARMOR", (ApiDel)DDI.processEncumbranceHeavyarmor);
            Api.Add("CC.DNDI.FEATLIST", (ApiDel)DDI.processFeatlist);
            Api.Add("CC.DNDI.POWERS", (ApiDel)DDI.processPowers);
            Api.Add("CC.DNDI.PROFICIENCYARMOR", (ApiDel)DDI.processProficiencyarmor);
            Api.Add("CC.DNDI.PROFICIENCYWEAPON", (ApiDel)DDI.processProficiencyweapon);
            Api.Add("CC.DNDI.SENSES", (ApiDel)DDI.processSenses);
            Api.Add("CC.DNDI.SPECIAL", (ApiDel)DDI.processSpecial);
            Api.Add("CC.DNDI.SPECIALABILITYLIST", (ApiDel)DDI.processSpecialabilitylist);
            Api.Add("CC.DNDI.SPEED.ARMOR", (ApiDel)DDI.processSpeedArmor);
            Api.Add("CC.DNDI.SPEED.MISC", (ApiDel)DDI.processSpeedMisc);
            Api.Add("CC.DNDI.WEAPONLIST", (ApiDel)DDI.processWeaponlist);
            Api.Add("CC.DNDI.INVENTORYLIST", (ApiDel)DDI.processInventorylist);
            Api.Add("CC.DNDI.COINS", (ApiDel)DDI.processCoins);
            Api.Add("CC.DNDI.DEFENSES.AC", (ApiDel)DDI.processDefensesAc);
            Api.Add("CC.DNDI.DEFENSES.REFLEX", (ApiDel)DDI.processDefensesReflex);
            Api.Add("CC.DNDI.DEFENSES.WILL", (ApiDel)DDI.processDefensesWill);
            Api.Add("CC.DNDI.DEFENSES.FORTITUDE", (ApiDel)DDI.processDefensesFortitude);
            
            Api.Add("CC.PG.COINS.SLOT2", (ApiDel)PCGen.processCoinsSlot2);
            Api.Add("CC.PG.ATTACKBONUS.GRAPPLE.SIZE", (ApiDel)PCGen.processAttackbonusGrappleSize);
            Api.Add("CC.PG.PROFICIENCYARMOR", (ApiDel)PCGen.processProficiencyarmor);
            Api.Add("CC.PG.PROFICIENCYWEAPON", (ApiDel)PCGen.processProficiencyweapon);
            Api.Add("CC.PG.SKILLLIST", (ApiDel)PCGen.processSkilllist);
            Api.Add("CC.PG.SPECIALABILITYLIST", (ApiDel)PCGen.processSpecialabilitylist);
            Api.Add("CC.PG.SPEED.SPECIAL", (ApiDel)PCGen.processSpeedSpecial);
            Api.Add("CC.PG.SPELLSET", (ApiDel)PCGen.processSpellset);
        }

        #region Ui
        void UpdateFileMode()
        {
            //Updates the UI based on the current ruleset
            SourceFormat = (String)SourceBox.SelectedItem;
            
            FileButton.Visible = true;
            importLocation.Text = "";
            importLocation.Visible = true;
            LocalCheckBox.Visible = true;
            CampaignCheckBox.Visible = true;
            XmlCheckBox.Visible = true;

            outputOptions.Items.Clear();

            outputOptions.Items.Add(UseLocalXslt);
            outputOptions.Items.Add(UseModuleManager);
            outputOptions.SetItemChecked(outputOptions.Items.Count - 1, true);

            importDialog.Title = "Select the " + SourceFormat + " File to import";
            importDialog.Filter = "";

            if (DisplayRuleset.Equals(RS_4E.Name))
            {
                if (SourceFormat.Equals(DDI.Format))
                {
                    importDialog.Filter = DDI.Filter;
                    outputOptions.Items.AddRange(OutputOptDNDI);
                }
                else if (SourceFormat.Equals(FGXML.Format))
                    importDialog.Filter = FGXML.Filter;
            }
            else if (DisplayRuleset.Equals(RS_35E.Name))
            {
                if (SourceFormat.Equals(PCGen.Format))
                    importDialog.Filter = PCGen.Filter;
                else if (SourceFormat.Equals(HeroLab.Format))
                    importDialog.Filter = HeroLab.Filter;
                else if (SourceFormat.Equals(FGXML.Format))
                    importDialog.Filter = FGXML.Filter;
            }
            else if (DisplayRuleset.Equals(RS_PF.Name))
            {
                if (SourceFormat.Equals(PCGen.Format))
                    importDialog.Filter = PCGen.Filter;
                else if (SourceFormat.Equals(HeroLab.Format))
                    importDialog.Filter = HeroLab.Filter;
                else if (SourceFormat.Equals(FGXML.Format))
                    importDialog.Filter = FGXML.Filter;
            }

            if (SourceFormat.Equals(FGXML.Format))
                XmlCheckBox.Visible = false;

            AppendCampaigns();
        }

        void UpdateImportDisplay()
        {
            //Updates the import display based on the currently selected options
            if (LocalCheckBox.Checked == true || CampaignCheckBox.Checked == true)
            {
                AppDataButton.Visible = true;
                FGPath.Visible = true;
                if (!outputOptions.Items.Contains(ReplaceString))
                    outputOptions.Items.Insert(0, ReplaceString);
            }
            else
            {
                AppDataButton.Visible = false;
                FGPath.Visible = false;
                outputOptions.Items.Remove(ReplaceString);
            }

            if (CampaignCheckBox.Checked == true)
            {
                campaignList.Visible = true;
                RefreshCampaignsButton.Visible = true;
            }
            else
            {
                campaignList.Visible = false;
                RefreshCampaignsButton.Visible = false;
            }
        }

        private void manageModulesToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            ManageModules();
        }

        private void exportConversionSheetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateConversionXslt();
        }

        private void RulesetBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Modifies the sources box with respect to the new ruleset
            DisplayRuleset = (String)RulesetBox.SelectedItem;
            SourceBox.Items.Clear();
            if (DisplayRuleset.Equals(RS_4E.Name))
            {
                InternalRuleset = RS_4E.InternalName;
                LocalPath = RS_4E.LocalPath;
                LocalBackupPath = RS_4E.LocalBackupPath;

                SourceBox.Items.Add(DDI.Format);
                SourceBox.Items.Add(FGXML.Format);
            }
            else if (DisplayRuleset.Equals(RS_35E.Name))
            {
                InternalRuleset = RS_35E.InternalName;
                LocalPath = RS_35E.LocalPath;
                LocalBackupPath = RS_35E.LocalBackupPath;

                SourceBox.Items.Add(HeroLab.Format);
                SourceBox.Items.Add(PCGen.Format);
                SourceBox.Items.Add(FGXML.Format);
            }
            else if (DisplayRuleset.Equals(RS_PF.Name))
            {
                InternalRuleset = RS_PF.InternalName;
                LocalPath = RS_PF.LocalPath;
                LocalBackupPath = RS_PF.LocalBackupPath;

                SourceBox.Items.Add(HeroLab.Format);
                SourceBox.Items.Add(PCGen.Format);
                SourceBox.Items.Add(FGXML.Format);
            }
            SourceBox.SelectedIndex = 0;
        }

        private void ClipboardButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(console.Text))
                return;

            Clipboard.Clear();
            Clipboard.SetText(console.Text);
        }

        private void AppDataButton_Click(object sender, EventArgs e)
        {
            if (appDataDialog.ShowDialog() == DialogResult.OK)
                FGPath.Text = appDataDialog.SelectedPath;
        }

        private void SaveToTextButton_Click(object sender, EventArgs e)
        {
            if (saveToTxtFileDialog.ShowDialog() == DialogResult.OK)
            {
                String txtFile = saveToTxtFileDialog.FileName;
                TextWriter textWriter = new StreamWriter(txtFile);
                textWriter.Write(console.Text);
                textWriter.Close();
            }
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            if (importDialog.ShowDialog() == DialogResult.OK)
                importLocation.Text = importDialog.FileName;
        }

        private void XmlLocationButton_Click(object sender, EventArgs e)
        {
            if (xmlOutputDialog.ShowDialog() == DialogResult.OK)
                XmlOutputLocation.Text = xmlOutputDialog.FileName;
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            bool supportSite = false;
            bool overrideChar = outputOptions.CheckedItems.Contains(ReplaceString);

            //Creates a log of loading common problems
            String log;
            try { log = GetLog(); }
            catch (Exception) { return; }

            //Loads the input into memory
            XmlDocument input;
            try
            {
                input = new XmlDocument();
                input.Load(importLocation.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
            
            //Creates the start of the log
            AppendConsole("Conversion Start");
            AppendConsole("Version: " + CC_VERSION + "." + CC_REVISION);
            AppendConsole("Game System: " + DisplayRuleset);
            AppendConsole("Ruleset: " + InternalRuleset);
            AppendConsole("Source Format: " + SourceFormat);
            AppendConsole(".Path: " + importLocation.Text);
            AppendConsole(log, false);
            AppendConsole("----Conversion Results----");

            //Parses the input into individual character nodes
            XmlNode[] characterNodes = GetBatchCharacters(input, CharPath, NamePath);
            String[] characterNames = GetNames(characterNodes, NamePath);

            //Loads the output xml template into file
            var outputdoc = new XmlDocument();
            try
            {
                outputdoc.LoadXml(Properties.Resources.Output);
            }
            catch (Exception)
            {
                MessageBox.Show("Corrupted assembly. Please redownload program.");
                AppendConsole("Corrupted assembly. Please redownload program.");
                return;
            }

            //Sets the output version "ccversion" attribute to the current version
            Set(outputdoc, "root/@ccversion", CC_VERSION + "." + CC_REVISION);

            //Loops through the character nodes and applies the correct parsing
            Boolean bError = false;
            for (int i = 0; i < characterNodes.Length; i++)
            {
                XmlNode convertedNode = null;
                Byte[] xslt = null;
                String xsltPath = null;
                try
                {
                    if (SourceFormat.Equals(HeroLab.Format))
                    {
                        xslt = Properties.Resources.HeroLab_3_5E;
                        xsltPath = XsltPath + HeroLab.Xslt;
                    }
                    else if (SourceFormat.Equals(DDI.Format))
                    {
                        xslt = Properties.Resources.Dndi_4E;
                        xsltPath = XsltPath + DDI.Xslt;
                    }
                    else if(SourceFormat.Equals(PCGen.Format))
                    {
                        xslt = Properties.Resources.PCGen_3_5E;
                        xsltPath = XsltPath + PCGen.Xslt;
                    }

                    if (SourceFormat.Equals(FGXML.Format))
                        convertedNode = characterNodes[i];
                    else if (xslt != null)
                    {
                        //Applys xslt transformation and executes API commands
                        XmlNode outChar;
                        XmlNode inChar = characterNodes[i];
                        try
                        {
                            ApplyXslt(inChar, out outChar, xslt, xsltPath);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                            return;
                        }
                        var inCharDoc = new XmlDocument();
                        inCharDoc.LoadXml(inChar.OuterXml);
                        ExecuteApiFunctions(inChar, ref outChar);
                        convertedNode = outChar;
                    }
                    else
                    {
                        //Dev message to warn if a ruleset has not been supported yet.
                        //Should never happen as long as Ruleset/Souce box are not modified
                        //without proper support added.
                        MessageBox.Show("WAKEUP DEV: Display Ruleset unsupported");
                        AppendConsole("WAKEUP DEV: Display Ruleset unsupported");
                        return;
                    }

                    // Searches the selected modules for data if the user has selected the option
                    if(outputOptions.CheckedItems.Contains(UseModuleManager))
                        convertedNode = SearchModules(convertedNode);

                    // Updates the console to reflect the sucessful progress
                    AppendConsole(characterNames[i] + " successfully converted.");

                    // Outputs the converted xml to the selected location
                    if (XmlCheckBox.Checked)
                    {
                        XmlNode import = outputdoc.ImportNode(convertedNode, true);
                        XmlNode root = outputdoc.SelectSingleNode("root");
                        if ((import != null) && (root != null))
                            root.AppendChild(import);
                    }
                    
                    // Imports the converted xml into the FG local characters list
                    if (LocalCheckBox.Checked)
                    {
                        String output = characterNames[i] + " successfully imported to local character list";
                        try
                        {
                            var localCampaign = new XmlDocument();
                            if (File.Exists(FGPath.Text + LocalPath))
                            {
                                localCampaign.Load(FGPath.Text + LocalPath);
                                localCampaign.Save(FGPath.Text + LocalBackupPath);
                            }
                            else
                            {
                                localCampaign.AppendChild(localCampaign.CreateXmlDeclaration("1.0", "iso-8859-1", null));
                                localCampaign.AppendChild(localCampaign.CreateElement("root"));
                            }

                            XmlNode root = localCampaign.SelectSingleNode("root");
                            XmlNode newChar = localCampaign.ImportNode(convertedNode, true);

                            var overrideForm = new ReplaceCharacter(characterNames[i],
                                "Target: Local Character List",
                                GetNames(root.ChildNodes, "name"));

                            if (overrideChar)
                                overrideForm.ShowDialog();

                            if (overrideForm.ReplaceChar)
                                ReplaceCharacter(root, newChar, root.ChildNodes[overrideForm.Index]);
                            else
                                root.AppendChild(newChar);

                            localCampaign.Save(FGPath.Text + LocalPath);
                        }
                        catch (Exception exception)
                        {
                            output = "Could not import " + characterNames[i] + " to local character list. Error: " + exception.Message;
                        }
                        AppendConsole(output);
                    }

                    // Imports the converted xml into the selected FG campaigns
                    if (CampaignCheckBox.Checked)
                    {
                        IEnumerator index = campaignList.CheckedIndices.GetEnumerator();
                        while (index.MoveNext())
                        {
                            var name = (String)campaignList.Items[(int)index.Current];
                            String output = characterNames[i] + " successfully imported to campaign " + name;
                            try
                            {
                                String path = CampaignDirectories[(int)(index.Current)];
                                String backupPath = path + CampaignDBBackupPath;
                                path += CampaignDBPath;

                                var campaignDb = new XmlDocument();
                                campaignDb.Load(path);
                                campaignDb.Save(backupPath);
                                XmlNode characterSheet = campaignDb.SelectSingleNode("root/charsheet");
                                if (characterSheet == null)
                                {
                                    XmlNode root = campaignDb.SelectSingleNode("root");
                                    root.AppendChild(campaignDb.CreateElement("charsheet"));
                                    characterSheet = root.LastChild;
                                }

                                XmlNode newChar = campaignDb.CreateElement(CreateId(characterSheet));
                                IEnumerator myEnum = (convertedNode.ChildNodes).GetEnumerator();
                                while (myEnum.MoveNext())
                                    newChar.AppendChild(campaignDb.ImportNode((XmlNode)myEnum.Current, true));

                                var overrideForm = new ReplaceCharacter(characterNames[i],
                                    "Target Campaign: " + name,
                                    GetNames(characterSheet.ChildNodes, "name"));

                                if (overrideChar)
                                    overrideForm.ShowDialog();

                                if (overrideForm.ReplaceChar)
                                    ReplaceCharacter(characterSheet, newChar, characterSheet.ChildNodes[overrideForm.Index]);
                                else
                                    characterSheet.AppendChild(newChar);

                                campaignDb.Save(path);
                            }
                            catch (Exception exception)
                            {
                                output = characterNames[i] + " could not import into campaign " + name + ". Error: ";
                                if (exception.Message.Contains("Name cannot begin with"))
                                {
                                    supportSite = true;
                                    output += "Campaign db.xml is corrupted. Please remove all occurances of '<>' and '</>' or restore the campaign using the db.import.xml. Please also post your db.xml to the support forum.";
                                }
                                else
                                    output += exception.Message;

                            }
                            AppendConsole(output);
                        }
                    }
                }
                catch (Exception exception)
                {
                    AppendConsole("Could not convert " + characterNames[i] + ": " + exception.Message);
                    bError = true;
                }
            }

            // Save XML
            if (!bError && XmlCheckBox.Checked)
            {
                String output = "Characters successfully outputted to XML";
                try
                {
                    outputdoc.Save(XmlOutputLocation.Text);
                }
                catch (Exception exception)
                {
                    output = "Could not output characters to xml. Error: " + exception.Message;
                }
                AppendConsole(output);
            }

            AppendConsole("--------------------------------------------------------------");

            if (supportSite)
                AppendConsole("Support link: " + SupportWebsite);
        }

        private void XmlCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (XmlCheckBox.Checked)
            {
                XmlOutputLocation.Visible = true;
                XmlLocationButton.Visible = true;
            }
            else if (XmlCheckBox.Checked == false)
            {
                XmlOutputLocation.Visible = false;
                XmlLocationButton.Visible = false;
            }
        }

        private void LocalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateImportDisplay();
        }

        private void CampaignCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateImportDisplay();
        }

        private void RefreshCampaignsButton_Click(object sender, EventArgs e)
        {
            AppendCampaigns();
        }

        private void SourceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFileMode();
        }
        #endregion

        static void CreateConversionXslt()
        {
            //Exports the conversion xslt files to a directory
            if(!Directory.Exists(XsltPath))
                Directory.CreateDirectory(XsltPath);

            try
            {
                Utility.CreateFileFromBytes(Properties.Resources.HeroLab_3_5E, XsltPath + HeroLab.Xslt, true);
                Utility.CreateFileFromBytes(Properties.Resources.Dndi_4E, XsltPath + DDI.Xslt, true);
                Utility.CreateFileFromBytes(Properties.Resources.PCGen_3_5E, XsltPath + PCGen.Xslt, true);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
            MessageBox.Show("XSLT files exported to the folder 'Xslt' within the current application directory."
            + Environment.NewLine + "Select the use local xslt option in output options to use the files.");
        }

        void ManageModules()
        {
            moduleManager.ShowDialog();
            moduleManager.SaveSettings(ref Settings);
            SaveSettings();
        }

        void AppendCampaigns()
        {
            try
            {
                campaignList.Items.Clear();
                var campaignDirectories = new List<string>();
                String[] temp = Directory.GetDirectories(FGPath.Text + CampaignPath);
                for (int i = 0; i < temp.Length; i++)
                {
                    //Determine Ruleset
                    var campaign = new XmlDocument();
                    try
                    {
                        campaign.Load(temp[i] + CampaignInfoPath);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    String ruleset = ValueOf(campaign, "root/ruleset");
                    //Add directory if ruleset matches the internal ruleset
                    if (ruleset.Equals(InternalRuleset))
                    {
                        campaignDirectories.Add(temp[i]);
                        String[] split = temp[i].Split(Path.DirectorySeparatorChar);
                        campaignList.Items.Add(split[split.Length - 1]);
                    }
                }
                CampaignDirectories = campaignDirectories.ToArray();
            }
            catch (Exception e)
            {
                AppendConsole("AppendCampaigns fail. Info:" + e.Message);
            }

        }     

        public void LoadSettings()
        {
            //Parses the settings file from disk (if it exists) into
            //a list<String>
            Settings = new List<string>();
            String error = null;
            try
            {
                if (!File.Exists(CCSettingsPath))
                {
                    error = "Settings file does not exist at " + CCSettingsPath + ". Please report this to the developer with operating system information.";
                    throw new Exception();
                }
                using (TextReader textReader = new StreamReader(CCSettingsPath))
                {
                    String line;
                    while (!String.IsNullOrEmpty((line = textReader.ReadLine())))
                    {
                        Settings.Add(line);
                    }
                }
            }
            catch (Exception exception)
            {
                if (String.IsNullOrEmpty(error))
                    error = exception.Message;
                MessageBox.Show(error);
            }
        }

        public void SaveSettings()
        {
            //Saves the list<string> settings to disk into a line breaked format
            String error = null;
            try
            {
                if (File.Exists(CCSettingsPath))
                {
                    using(Stream settings = File.OpenRead(CCSettingsPath))
                    {
                        using(Stream backupsettings = File.OpenWrite(CCBackupSettingsPath))
                        {
                            Utility.CopyStream(settings, backupsettings);
                        }
                    }
                    File.Delete(CCSettingsPath);
                }
                
                using (var file = new StreamWriter(CCSettingsPath))
                {
                    foreach (string line in Settings)
                        file.WriteLine(line);
                }
            }
            catch (Exception exception)
            {
                if (String.IsNullOrEmpty(error))
                    error = exception.Message;
                MessageBox.Show(error);
            }
        }

        bool ReplaceCharacter(XmlNode parentNode, XmlNode newNode, XmlNode oldNode)
        {
            if (DisplayRuleset.Equals(RS_35E.Name))
                RS_35E.ReplaceCharacter(this, parentNode, newNode, oldNode);
            else if (DisplayRuleset.Equals(RS_PF.Name))
                RS_PF.ReplaceCharacter(this, parentNode, newNode, oldNode);
            else if (DisplayRuleset.Equals(RS_4E.Name))
                RS_4E.ReplaceCharacter(this, parentNode, newNode, oldNode);

            parentNode.ReplaceChild(newNode, oldNode);
            return true;
        }

        static public XmlNode ImportIdNode(XmlDocument newDoc, XmlNode parentNode, XmlNode oldNode)
        {
            //replicates oldNode with an FG idname valid for the parent node
            XmlNode newNode = newDoc.CreateElement(CreateId(parentNode));
            foreach (XmlNode node in oldNode.ChildNodes)
                newNode.AppendChild(newDoc.ImportNode(node, true));
            return newNode;
        }

        public bool ReplaceElement(XmlNode oldNode, XmlNode newNode, String path)
        {
            String value = ValueOf(oldNode, path, false);
            if (String.IsNullOrEmpty(value))
                return false;
            Set(newNode, path, value);
            return true;
        }

        XmlNode[] GetBatchCharacters(XmlDocument input, String CharPath, String NamePath)
        {
            XmlNode[] characternodes = null;
            int count = input.SelectNodes(CharPath).Count;
            if (count > 1)
            {
                //Selects the nodes based on the CharPath parameter
                XmlNodeList nodelist = input.SelectNodes(CharPath);
                var names = new String[count];
                for (int chari = 0; chari < nodelist.Count; chari++)
                    names[chari] = ValueOf(nodelist[chari], NamePath).Trim();

                //Opens up a dialog allowing the player to select which characters to convert
				var import = new BatchConvert(names);
				import.ShowDialog();

				int i = 0;
				characternodes = new XmlNode[import.Characters.CheckedIndices.Count];
				IEnumerator myEnum = import.Characters.CheckedIndices.GetEnumerator();
				while(myEnum.MoveNext())
				{
					characternodes[i] = nodelist[(int)myEnum.Current];
					i++;
				}
            }
            else
            {
                characternodes = new XmlNode[1];
                characternodes[0] = input.SelectSingleNode(CharPath);
            }
            return characternodes;
        }

        String[] GetNames(XmlNode[] nodes, String namePath)
        {
            var names = new String[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
                names[i] = ValueOf(nodes[i], namePath).Trim();
            return names;
        }

        String[] GetNames(XmlNodeList nodes, String namePath)
        {
            var names = new String[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
                names[i] = ValueOf(nodes[i], namePath).Trim();
            return names;
        }

        String GetLog()
        {
            var log = new StringBuilder();
            var errorMessage = new StringBuilder();
            String errorSnapshot;

            if (importLocation.Text.Length == 0)
                errorMessage.AppendLine("No file selected.");

            else if (!File.Exists(importLocation.Text))
                errorMessage.AppendLine("No file exists at selected location.");

            const String xmlLogHeader = "Output to Xml: ";
            if (XmlCheckBox.Checked)
            {
                if (XmlOutputLocation.Text.Length == 0)
                    errorMessage.AppendLine("No location selected for xml output.");
                else
                {
                    log.AppendLine(String.Format("{0}true", xmlLogHeader));
                    log.AppendLine(String.Format(".Path: {0}", importLocation.Text));
                }
            }
            else
                log.AppendLine(String.Format("{0}false", xmlLogHeader));

            const String localCharHeader = "Import to Local Character List: ";
            if (LocalCheckBox.Checked)
            {
                if (FGPath.Text.Length == 0)
                    errorMessage.AppendLine("No FG App Data directory selected.");
                //else if (!File.Exists(FGPath.Text + LocalPath))
                //    errorMessage.AppendLine("No FG local character list exists.");
                else
                {
                    log.AppendLine(String.Format("{0}true", localCharHeader));
                    log.AppendLine(String.Format(".Path: {0}{1}", FGPath.Text, LocalPath));
                }
            }
            else
                log.AppendLine(String.Format("{0}false", localCharHeader));

            const String campaignHeader = "Import to Campaign: ";
            if (CampaignCheckBox.Checked)
            {
                if (FGPath.Text.Length == 0)
                    errorMessage.AppendLine("No FG App Data directory selected.");
                else
                {
                    log.AppendLine(String.Format("{0}true", campaignHeader));

                    foreach(int ind in campaignList.CheckedIndices)
                    {
                        String path = CampaignDirectories[ind] + CampaignDBPath;
                        log.AppendLine(String.Format("{0}.Path: {1}", campaignList.Items[ind], path));
                    }

                    if (outputOptions.CheckedItems.Contains(ReplaceString))
                        log.AppendLine("Replace existing character: true");
                    else
                        log.AppendLine("Replace existing character: false");
                }
            }
            else
                log.AppendLine(String.Format("{0}false", campaignHeader));

            if(outputOptions.CheckedItems.Contains(UseModuleManager))
            {
                log.AppendLine("Using module manager: true");
                foreach(String moduleName in moduleManager.ModuleNames)
                    log.AppendLine(".Module: " + moduleName);
            }
            else
                log.AppendLine("Using module manager: false");

            if(outputOptions.CheckedItems.Contains(UseLocalXslt))
                log.AppendLine("Using local xslt: true");
            else
                log.AppendLine("Using local xslt: false");

            if(SourceFormat.Equals(DDI.Format))
            {
                foreach(String dndiOption in OutputOptDNDI)
                {
                    String sFlag = outputOptions.CheckedItems.Contains(dndiOption) ? "true" : "false";
                    log.AppendLine(String.Format("{0}: {1}", dndiOption, sFlag));
                }
            }

            errorSnapshot = errorMessage.ToString();
            XmlDocument input = null;
            try
            {
                input = new XmlDocument();
                input.Load(importLocation.Text);
            }
            catch (Exception e)
            {
                if (!errorMessage.ToString().Contains("No file selected"))
                    errorMessage.AppendLine(e.Message);
            }

            //If input has loaded correctly
            if (errorSnapshot.Equals(errorMessage.ToString()))
            {
                errorSnapshot = errorMessage.ToString();
                if (SourceFormat.Equals(DDI.Format))
                {
                    NamePath = DDI.NamePath;
                    CharPath = DDI.CharPath;
                    DDI.CheckFile(input, errorMessage);
                }
                else if (SourceFormat.Equals(HeroLab.Format))
                {
                    NamePath = HeroLab.NamePath;
                    CharPath = HeroLab.CharPath;
                    HeroLab.CheckFile(input, errorMessage);
                }
                else if (SourceFormat.Equals(FGXML.Format))
                {
                    NamePath = FGXML.NamePath;
                    CharPath = FGXML.CharPath;
                    FGXML.CheckFile(input, errorMessage);
                }
                else if (SourceFormat.Equals(PCGen.Format))
                {
                    NamePath = PCGen.NamePath;
                    CharPath = PCGen.CharPath;
                    PCGen.CheckFile(input, errorMessage);
                }

                if (input.SelectSingleNode(CharPath) == null && errorMessage.ToString().Equals(errorSnapshot))
                    errorMessage.AppendLine("No characters present in file.");
            }

            //Displays the error message if it exists and stops the conversion process
            if (!String.IsNullOrEmpty(errorMessage.ToString()))
            {
                MessageBox.Show("Cannot convert. Errors below." + Environment.NewLine + errorMessage.ToString());
                throw new Exception("");
            }
            
            return log.ToString();
        }

        //applies the transformation xslt
        void ApplyXslt(XmlNode inChar, out XmlNode outChar, byte[] xslt, String localXsltPath)
        {
            var sbxslt = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sbxslt))
            {
                XmlReader reader = new XmlNodeReader(inChar);
                var xsl = new XslCompiledTransform(true);
                if (outputOptions.CheckedItems.Contains(UseLocalXslt))
                {
                    xsl.Load(localXsltPath);
                }
                else
                {
                    XmlReader xsltread = new XmlTextReader(new MemoryStream(xslt));
                    xsl.Load(xsltread);
                }
                xsl.Transform(reader, writer);
            }
            string xml = sbxslt.ToString();
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            outChar = doc.DocumentElement;
            if (!outChar.Name.Equals("character"))
                outChar = outChar.ChildNodes[0];
        }

        String FormattedTextToString(XmlNode node, Boolean bSkipLinkList)
        {
            XmlAttribute att = node.Attributes["type"];
            if ((att != null) && (att.Value == "formattedtext"))
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    XmlNode x = node.ChildNodes[i];
                    XmlNode next = null;
                    if (i + 1 < node.ChildNodes.Count)
                        next = node.ChildNodes[i + 1];

                    if ((x.Name == "p") || (x.Name == "h") || (x.Name == "frame"))
                    {
                        sb.Append(x.InnerText);
                        if ((next != null) && ((next.Name != "linklist") || (!bSkipLinkList)))
                            sb.Append(@"\r\r");
                    }
                    else if ((x.Name == "list") || ((x.Name == "linklist") && (!bSkipLinkList)) || (x.Name == "table"))
                    {
                        for (int j = 0; j < x.ChildNodes.Count; j++)
                        {
                            XmlNode y = x.ChildNodes[j];
                            sb.Append(y.InnerText);
                            if (j + 1 < x.ChildNodes.Count)
                                sb.Append(@"\r");
                        }
                        if ((next != null) && ((next.Name != "linklist") || (!bSkipLinkList)))
                            sb.Append(@"\r\r");
                    }
                }
                return sb.ToString();
            }

            return node.InnerText;
        }

        XmlNode SearchModules(XmlNode characterNode)
        {
            for (int i = 0; i < ModuleManager.Types.Length; i++)
            {
                String type = ModuleManager.Types[i];

                //Checks if there are loaded modules for the ruleset and type
                //if there are no modules this type is skipped
                List<XmlDocument> documents = moduleManager.RetrieveModules(DisplayRuleset, type);
                if (documents.Count == 0)
                    continue;

                var playerNodeList = new List<XmlNode>();
                var playerNodeNameList = new List<String>();
                //Adds all of the nodes of one type for the 3.5E and Pathfinder rulesets
                //based on module database location assumptions
                if (DisplayRuleset.Equals(RS_35E.Name) || DisplayRuleset.Equals(RS_PF.Name))
                {
                    var paths = new List<string>();
                    if (type.Equals(ModuleManager.Spells))
                    {
                        XmlNode spellsetNode = characterNode.SelectSingleNode("spellset");
                        if(spellsetNode != null)
                        {
                            foreach (XmlNode spellset in spellsetNode.ChildNodes)
                            {
                                XmlNode levelsNode = spellset.SelectSingleNode("levels");
                                if (levelsNode == null)
                                    continue;

                                foreach (XmlNode level in levelsNode.ChildNodes)
                                {
                                    XmlNode spellsNode = level.SelectSingleNode("spells");
                                    if (spellsNode == null)
                                        continue;

                                    foreach (XmlNode spell in spellsNode.ChildNodes)
                                    {
                                        playerNodeList.Add(spell);
                                        playerNodeNameList.Add(ValueOf(spell, "name"));
                                    }
                                }
                            }
                        }
                    }
                    else if (type.Equals(ModuleManager.Weapons))
                    {
                        paths.Add("weaponlist/*");
                        paths.Add("inventorylist/*");
                    }
                    else if (type.Equals(ModuleManager.Skills))
                        paths.Add("skilllist/*");
                    else if (type.Equals(ModuleManager.Feats))
                        paths.Add("featlist/*");
                    else if (type.Equals(ModuleManager.MagicItems))
                        paths.Add("inventorylist/*");
                    else if (type.Equals(ModuleManager.Equipment))
                        paths.Add("inventorylist/*");

                    if (paths.Count > 0)
                    {
                        foreach (String path in paths)
                        {
                            foreach (XmlNode node in characterNode.SelectNodes(path))
                            {
                                playerNodeList.Add(node);
                                String name = ValueOf(node, "name", false);
                                if (String.IsNullOrEmpty(name))
                                    name = ValueOf(node, "value", false);
                                if (String.IsNullOrEmpty(name))
                                    name = ValueOf(node, "label", false);
                                playerNodeNameList.Add(name);
                            }
                        }
                    }
                }

                foreach (var document in documents)
                {
                    foreach (XmlNode dbNode in document.SelectSingleNode(ModuleManager.Paths[type]).ChildNodes)
                    {
                        String name = ValueOf(dbNode, "name");
                        int playerNodeIndex;
                        if ((playerNodeIndex = playerNodeNameList.IndexOf(name)) == -1)
                            continue;

                        XmlNode playerNode = playerNodeList[playerNodeIndex];
                        foreach (XmlNode playerChild in playerNode.ChildNodes)
                        {
                            XmlNode dbChild;
                            String playerChildName = playerChild.Name;
                            if (playerChildName.Equals("description") && !type.Equals(ModuleManager.Spells))
                                playerChildName = type.Equals(ModuleManager.Feats) ? "benefit" : "text";
                            else if (playerChildName.Equals("statname"))
                                playerChildName = "ability";

                            if ((dbChild = dbNode.SelectSingleNode(playerChildName)) == null)
                                continue;

                            playerChild.InnerText = FormattedTextToString(dbChild, type.Equals(ModuleManager.Spells));

                            if (type.Equals(ModuleManager.Spells))
                            {
                                XmlNode nodeSpell = playerChild.ParentNode;
                                if (nodeSpell != null)
                                {
                                    XmlNodeList listLinks = dbChild.SelectNodes("linklist/link");
                                    if (listLinks.Count > 0)
                                    {
                                        XmlDocument doc = nodeSpell.OwnerDocument;
                                        XmlNode nodeLinkedSpells = doc.CreateElement("linkedspells");
                                        nodeSpell.AppendChild(nodeLinkedSpells);

                                        foreach (XmlNode link in listLinks)
                                        {
                                            XmlNode nodeLinkItem = doc.CreateElement(CreateId(nodeLinkedSpells));
                                            nodeLinkedSpells.AppendChild(nodeLinkItem);

                                            XmlNode nodeLinkField = CreateElement(doc, "link", "windowreference");
                                            nodeLinkItem.AppendChild(nodeLinkField);

                                            XmlNode nodeLinkFieldClass = doc.CreateElement("class");
                                            XmlAttribute attClass = link.Attributes["class"];
                                            if (attClass != null)
                                                nodeLinkFieldClass.InnerText = attClass.Value;
                                            nodeLinkField.AppendChild(nodeLinkFieldClass);

                                            XmlNode nodeLinkFieldRecord = doc.CreateElement("recordname");
                                            XmlAttribute attRecord = link.Attributes["recordname"];
                                            if (attRecord != null)
                                                nodeLinkFieldRecord.InnerText = attRecord.Value;
                                            nodeLinkField.AppendChild(nodeLinkFieldRecord);

                                            XmlNode nodeLinkedNameField = CreateElement(doc, "linkedname", "string", link.InnerText);
                                            nodeLinkItem.AppendChild(nodeLinkedNameField);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return characterNode;
        }

        #region API
        public Dictionary<String, String> ParseApiAttributes(String call)
        {
            //retrieves api calls from attribute
            var apiAttributes = new Dictionary<String, String>();
            int start = call.IndexOf("(");
            int end = call.IndexOf(")");
            try
            {
                if (start != -1 && end != -1)
                {
                    String[] attributes = call.Substring(start, end - start).Split(';');
                    foreach (String attribute in attributes)
                    {
                        String[] atrsplit = attribute.Split('=');
                        apiAttributes.Add(atrsplit[0], atrsplit[1].Replace(".", "/"));
                    }
                }
                return apiAttributes;
            }
            catch (Exception){}
            return new Dictionary<string, string>();
        }

        void ExecuteApiFunctions(XmlNode inChar, ref XmlNode outChar)
        {
            ExecuteApiFunctions(inChar, ref outChar, ref outChar);
        }

        void ExecuteApiFunctions(XmlNode inChar, ref XmlNode outChar, ref XmlNode parentChar)
        {
            XmlDocument output = outChar.OwnerDocument;
            for (int i = 0; i < parentChar.ChildNodes.Count; i++)
            {
                XmlNode targetNode = parentChar.ChildNodes[i];
                if(targetNode.Name.Equals("#text"))
                    continue;
                ExecuteApiFunctions(inChar, ref outChar, ref targetNode);
                foreach (String key in Api.Keys)
                {
                    XmlNode textNode = targetNode.SelectSingleNode("text()");
                    XmlAttribute fgapi = targetNode.Attributes["fgapi"];
                    String textNodeText = ParseApiCall(textNode);
                    String fgapiText = ParseApiCall(fgapi);
                    String call = null;

                    if (textNodeText.Equals(key))
                    {
                        call = textNode.InnerText;
                        textNode.InnerText = "";
                    }
                    else if (fgapiText.Contains(key))
                    {
                        call = fgapi.InnerText;
                        targetNode.Attributes.Remove(fgapi);
                    }

                    if (call != null)
                    {
                        var del = (ApiDel)Api[key];
                        del(this, call, ref inChar, ref outChar, ref targetNode);
                    }
                }
            }
        }

        String ParseApiCall(XmlNode node)
        {
            if (node == null)
                return "";
            String innerText = node.InnerText;
            innerText = innerText.Replace("|", "");
            return innerText;
        }

        #region HelperFunctions

        public String ParseSpecial(XmlNode node, String name)
        {
            if (node == null)
                return "";

            return ParseSpecial(node.ChildNodes, name);
        }

        public String ParseSpecial(XmlNodeList nodeList, String name)
        {
            if (nodeList == null)
                return "";

            var sb = new StringBuilder();
            if (nodeList.Count > 0)
            {
                if (!String.IsNullOrEmpty(name))
                {
                    sb.Append(name);
                    sb.Append(" ");
                }
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode source = nodeList[i];
                    String sourcename = ValueOf(source, "@shortname", false);
                    if (String.IsNullOrEmpty(sourcename))
                        sourcename = ValueOf(source, "@name");

                    if (String.IsNullOrEmpty(sourcename)
                    || ValueOf(source, "@name").Contains("Spell Resistance"))
                        continue;
                    sb.Append(sourcename);
                    if (i + 1 < nodeList.Count)
                        sb.Append(", ");
                }
                sb.Append(".");
            }

            if (sb.ToString().Equals(name + " ."))
                return "";

            return sb.ToString();
        }

        bool CheckForDuplicateId(XmlNode node, String name, String path)
        {
            foreach (XmlNode source in node.ChildNodes)
            {
                if (ValueOf(source, path).Equals(name))
                    return true;
            }
            return false;
        }

        bool CheckForDuplicateId(XmlNode node, String name)
        {
            return CheckForDuplicateId(node, name, "name");
        }

        public bool CheckForDuplicateValue(XmlNode node, String name)
        {
            return CheckForDuplicateId(node, name, "value");
        }

        public bool CheckForDuplicateSpell_35(XmlNode spellset, String spellName, String spellLevel)
        {
            XmlNodeList spells = spellset.SelectSingleNode("levels/level" + spellLevel + "/spells").ChildNodes;
            foreach (XmlNode source in spells)
            {
                String name = ValueOf(source, "name");
                if (name.Equals(spellName))
                    return true;
            }
            return false;
        }

        #endregion
        #endregion

        static public XmlNode CopyChildNodes(XmlDocument doc, XmlNode source, XmlNode target)
        {
            foreach (XmlNode node in source.ChildNodes)
                target.AppendChild(doc.ImportNode(node, true));
            return target;
        }

        //Creates an FG id node name given a valid index
        static public String CreateId(int index)
        {
            return String.Format("id-{0:00000}", index);
        }

        //Creates an id by looping through the child nodes of a parent until a
        //valid FG id name has not been taken.
        static public String CreateId(XmlNode parentNode)
        {
            for (int i = 1; ; i++)
            {
                String id = CreateId(i);
                if (parentNode.SelectSingleNode(id) == null)
                    return id;
            }
        }
 
        static public XmlElement CreateElement(XmlDocument doc, String name, String type, String innerText = null)
        {
            //Creates an element with predefined parameters 
            XmlElement child = doc.CreateElement(name);
            child.SetAttributeNode(doc.CreateAttribute("type"));
            child.SetAttribute("type", type);
            if(!String.IsNullOrEmpty(innerText))
                child.InnerText = innerText;
            return child;
        }

        public String ValueOf(XmlNode node, String path, bool generateError = true)
        {
            if (node == null)
            {
                AppendConsole("Null Argument Node. <Path:" + path + ">");
                return "";
            }

            if (node.Name.Equals("#text") && !String.IsNullOrEmpty(path))
                generateError = false;

            XmlNode tempNode;
            try
            {
                tempNode = node.SelectSingleNode(path);
            }
            catch (Exception)
            {
                if (generateError)
                    AppendConsole("Invalid Path Error. <Name:" + node.Name + "; Path:" + path + ">");

                return "";
            }

            String value;
            if (tempNode == null)
            {
                if (generateError)
                    AppendConsole("Node Read Error. <Name:" + node.Name + "; Path:" + path + ">");

                value = "";
            }
            else
            {
                value = tempNode.InnerText;
            }

            return value;
        }

        public String ValueOf(XmlNode node, String path, String error, bool generateError = true)
        {
            if (node == null)
            {
                AppendConsole("Null Argument Node. <Path:" + path + "; Info:" + error + ">");
                return "";
            }

            if (node.Name.Equals("#text") && !String.IsNullOrEmpty(path))
                generateError = false;

            XmlNode tempNode;
            try
            {
                tempNode = node.SelectSingleNode(path);
            }
            catch (Exception)
            {
                AppendConsole("Invalid Path Error. <Name:" + node.Name + "; Path:" + path + "; Info:" + error + ">");
                return "";
            }

            String value;
            if (tempNode == null)
            {
                if (generateError)
                    AppendConsole("Node Read Error. <Name:" + node.Name + "; Path:" + path + "; Info:" + error + ">");

                value = "";
            }
            else
            {
                value = tempNode.InnerText;
            }

            return value;
        }

        static public int ConvertToInt(String str)
        {
            int value;
            try
            {
                value = Convert.ToInt32(str);
            }
            catch (Exception)
            {
                value = 0;
            }
            return value;
        }

        public void Set(XmlNode node, String path, String error, XmlNode destNode, String destPath, String destError, bool generateError = true)
        {
            String value = ValueOf(node, path, error, generateError);
            if (destNode == null)
            {
                AppendConsole("Null Destination Node. <Path:" + path + "; Info:" + error + ">");
                return;
            }

            XmlNode tempNode;
            try
            {
                tempNode = destNode.SelectSingleNode(destPath);
            }
            catch (Exception)
            {
                AppendConsole("Invalid Path Error. <Name:" + node.Name + "; Path:" + path + "; Info:" + error + ">");
                return;
            }

            if (tempNode == null)
            {
                if (generateError)
                    AppendConsole("Node Read Error. <Name:" + node.Name + "; Path:" + path + ">");

                destNode.InnerText = "";
            }
            else
            {
                destNode.InnerText = tempNode.InnerText;
            }
        }

        public void Set(XmlNode node, String path, XmlNode destNode, String destPath, bool generateError = true)
        {
            String value = ValueOf(node, path, generateError);
            if (destNode == null)
            {
                if (generateError)
                    AppendConsole("Null Destination Node. <Path:" + path + ">");

                return;
            }

            XmlNode tempDestNode;
            try
            {
                tempDestNode = destNode.SelectSingleNode(destPath);
            }
            catch (Exception)
            {
                if (generateError)
                    AppendConsole("Invalid Path Error. <Name:" + destNode.Name + "; Path:" + destPath + ">");

                return;
            }

            if (tempDestNode == null)
            {
                if (generateError)
                    AppendConsole("Node Read Error. <Name:" + destNode.Name + "; Path:" + destPath + ">");
                return;
            }
            else
            {
                tempDestNode.InnerText = value;
            }
        }

        public void Set(XmlNode node, String path, String value, bool generateError = true)
        {
            if (node == null)
            {
                if (generateError)
                    AppendConsole("Null Target Node for Set. <Path:" + path + ">");
                return;
            }

            if (String.IsNullOrEmpty(path))
            {
                if (generateError)
                    AppendConsole("Null path for set. <Node:" + node.Name + ">");
                return;
            }

            if (String.IsNullOrEmpty(value))
                return;

            try
            {
                XmlNode temp = node.SelectSingleNode(path);
                temp.InnerText = value;
            }
            catch (Exception)
            {
                if (generateError)
                    AppendConsole("Invalid path for set. <Node:" + node.Name + "; Path:" + path + ">");
            }

        }

        public void Set(XmlNode node, String value, bool generateError = true)
        {
            if (node == null)
            {
                if (generateError)
                    AppendConsole("Null Target Node for Set.");
                return;
            }

            if (String.IsNullOrEmpty(value))
                return;

            try
            {
                node.InnerText = value;
            }
            catch (Exception e)
            {
                if (generateError)
                    AppendConsole("InnerText Set Fail. <Exception" + e.ToString() + ">");
            }
        }

        public void AppendConsole(String message, bool newline = true)
        {
            console.AppendText(message);
            if(newline)
                console.AppendText(Environment.NewLine);
            console.SelectionStart = console.Text.Length;
            console.ScrollToCaret();
        }

        public static String CreateDice(String diceValue, String diceNumber)
        {
            int nDice = ConvertToInt(diceNumber);

            var sb = new StringBuilder();
            for (int i = 0; i < nDice; i++)
            {
                sb.Append(diceValue);
                if (i + 1 < nDice)
                    sb.Append(",");
            }

            return sb.ToString();
        }

        static public String UppercaseFirst(String s)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        static public String StripPlusSign(String s)
        {
            return s.TrimStart('+');
        }

        private void installToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCGen.CreateSheets();
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCGen.CopySheetToClipboard();
        }

        private void chooseLocationToInstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PCGen.SaveSheetToDirectory(folderBrowser.SelectedPath);
        }
    }
}
