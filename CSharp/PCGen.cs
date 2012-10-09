// Copyright (c) 2012, SmiteWorks USA LLC

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace CharacterConverter
{
    class PCGen
    {
        public const String Format = "PCGen (.xml)";

        public const String Filter = "PCGen Character (*.xml)|*.xml";

        public const String PCGDirectory = @"\PCGen\PCGen5164";
        public const String PCGOutputDirectory = @"\outputsheets";
        public const String PCGOutputSubDirectory = @"\d20\fantasy\htmlxml";

        public const String PCGOutputSheet = @"\csheet_FantasyGrounds2_pc_3.5.xml";
        public const String Xslt = @"\PCGen-3.5E.xslt";

        public const String CharPath = @"root/character";
        public const String NamePath = @"name";

        static public void CheckFile(XmlDocument doc, StringBuilder errorMessage)
        {
            if (doc.SelectSingleNode(@"root/@pcgenversion") == null)
                errorMessage.Append("Invalid PCGen file.");
        }

        static public void SaveSheetToDirectory(String sPath)
        {
            try
            {
                if (File.Exists(sPath + PCGOutputSheet))
                    File.Delete(sPath + PCGOutputSheet);
                using (Stream pcgenStream = new MemoryStream(Properties.Resources.csheet_FantasyGrounds2_pc_3_5))
                {
                    using (Stream fileStream = File.Create(sPath + PCGOutputSheet))
                        Utility.CopyStream(pcgenStream, fileStream);
                }
            }
            catch (ArgumentNullException e_null)
            {
                MessageBox.Show("Corrupted assembly. Please redownload program.\r\n\r\n" + e_null.Message);
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to copy file to folder specified.\r\n\r\n" + e.Message);
                return;
            }

            MessageBox.Show("Output sheets successfully saved.");
        }

        static public void CopySheetToClipboard()
        {
            try
            {
                if (!Directory.Exists(CharConverter.FGDataDir))
                    Directory.CreateDirectory(CharConverter.FGDataDir);
                if (!Directory.Exists(CharConverter.CCDir))
                    Directory.CreateDirectory(CharConverter.CCDir);

                SaveSheetToDirectory(CharConverter.CCDir);

                var paths = new StringCollection();
                paths.Add(CharConverter.CCDir + PCGOutputSheet);
                Clipboard.SetFileDropList(paths);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
       }

        static public void CreateSheets(bool ignore = false)
        {
            try
            {
                var sSheetPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles) + PCGDirectory + PCGOutputDirectory + PCGOutputSubDirectory;
                if (Directory.Exists(sSheetPath))
                {
                    if (File.Exists(sSheetPath + PCGOutputSheet))
                        File.Delete(sSheetPath + PCGOutputSheet);
                    using (Stream strSource = new MemoryStream(Properties.Resources.csheet_FantasyGrounds2_pc_3_5))
                    {
                        using (Stream strDest = File.OpenWrite(sSheetPath + PCGOutputSheet))
                            Utility.CopyStream(strSource, strDest);
                    }
                }
            }
            catch (IOException e)
            {
                //Catches any file write/read specific errors
                if (ignore)
                {
                    var permForm = new SaveOutputError(e.Message);
                    permForm.ShowDialog();
                }
                return;
            }
            catch (UnauthorizedAccessException)
            {
                //Catches the "lack of file permissions" error and displays the
                //correct steps to fix the problem.
                if (!ignore)
                {
                    var permForm = new SaveOutputError();
                    permForm.ShowDialog();
                }
                return;
            }

            if (!ignore)
            {
                MessageBox.Show(
                    "PCGen support installed successfully." + Environment.NewLine
                    + Environment.NewLine
                    + "For Pathfinder or 3.5E PCGen characters" + Environment.NewLine
                    + "1) Click File.Export....standard" + Environment.NewLine
                    + "2) Choose " + PCGOutputSubDirectory + PCGOutputSheet + Environment.NewLine
                    + "3) Convert the resulting xml file with this program using the respective import format."
                );
            }
        }

        static public void processCoinsSlot2(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            //coins/slot2/amount
            String value = targetNode.InnerText;
            if (!String.IsNullOrEmpty(value))
            {
                String[] gold = value.Split(' ');
                int goldamount = 0;
                for (int i = 0; i < gold.Length; i++)
                    goldamount += CharConverter.ConvertToInt(gold[i].Split('.')[0]);
                targetNode.InnerText = Convert.ToString(goldamount);
            }
        }

        static public void processAttackbonusGrappleSize(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            int size = CharConverter.ConvertToInt(targetNode.InnerText);
            size *= -1;
            targetNode.InnerText = Convert.ToString(size);
        }

        static public void processProficiencyarmor(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            var ApiAtributes = app.ParseApiAttributes(call);
            String featlist = ApiAtributes.ContainsKey("featlist") ? ApiAtributes["featlist"] : "featlist";

            XmlNode node = outChar.SelectSingleNode(featlist);
            XmlNodeList nodelist = node.ChildNodes;
            XmlNode armorpro = targetNode;
            for (int i = nodelist.Count - 1; i >= 0; i--)
            {
                XmlNode feat = nodelist[i];
                String value = app.ValueOf(feat, "value");
                XmlNode target = null;
                if (value.Contains("Armor Proficiency"))
                {
                    value = value.Split(',')[1].Substring(1);
                    target = armorpro;
                }
                else if (value.Contains("Shield Proficiency"))
                {
                    String[] valuesplit = value.Split(' ');
                    var shsb = new StringBuilder();
                    foreach (String shvalue in valuesplit)
                    {
                        if (shvalue.Equals("Proficiency"))
                            break;

                        shsb.Append(shvalue);
                        shsb.Append(" ");
                    }
                    shsb.Remove(shsb.Length - 1, 1);
                    value = shsb.ToString();
                    target = armorpro;

                    node.RemoveChild(node.ChildNodes[i]);
                }
                if (target != null)
                {
                    if (app.CheckForDuplicateValue(armorpro, value))
                        continue;

                    target.AppendChild(outChar.OwnerDocument.CreateElement(CharConverter.CreateId(target)));
                    target.LastChild.AppendChild(CharConverter.CreateElement(outChar.OwnerDocument, "value", "string", value));
                    value = app.ValueOf(feat, "description");
                    target.LastChild.AppendChild(CharConverter.CreateElement(outChar.OwnerDocument, "description", "string", value));
                }
            }
        }

        static public void processProficiencyweapon(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            var ApiAtributes = app.ParseApiAttributes(call);
            String featlist = ApiAtributes.ContainsKey("featlist") ? ApiAtributes["featlist"] : "featlist";

            XmlNode node = outChar.SelectSingleNode(featlist);
            XmlNodeList nodelist = node.ChildNodes;
            XmlNode weaponpro = targetNode;
            for (int i = nodelist.Count - 1; i >= 0; i--)
            {
                XmlNode feat = nodelist[i];
                String value = app.ValueOf(feat, "value");
                XmlNode target = null;
                if (value.Contains("Weapon Proficiency"))
                {
                    String[] valuesplit = value.Split(' ');
                    var wepsb = new StringBuilder();
                    foreach (String wepvalue in valuesplit)
                    {
                        if (wepvalue.Equals("Weapon"))
                            break;
                        wepsb.Append(wepvalue);
                        wepsb.Append(" ");
                    }
                    wepsb.Remove(wepsb.Length - 1, 1);
                    value = wepsb.ToString();
                    target = weaponpro;

                    node.RemoveChild(node.ChildNodes[i]);
                }
                if (target != null)
                {
                    if (app.CheckForDuplicateValue(weaponpro, value))
                        continue;

                    target.AppendChild(outChar.OwnerDocument.CreateElement(CharConverter.CreateId(target)));
                    target.LastChild.AppendChild(CharConverter.CreateElement(outChar.OwnerDocument, "value", "string", value));
                    value = app.ValueOf(feat, "description");
                    target.LastChild.AppendChild(CharConverter.CreateElement(outChar.OwnerDocument, "description", "string", value));
                }
            }
        }

        static public void processSkilllist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value;
            foreach (XmlNode skill in targetNode.ChildNodes)
            {
                String label = app.ValueOf(skill, "label");
                if (label.Contains("(")
                && label.Contains(")")
                && (label.Contains("Knowledge")
                || label.Contains("Perform")
                || label.Contains("Profession")
                || label.Contains("Craft")))
                {
                    app.Set(skill, "label", label.Split('(')[0]);
                    value = label.Split('(')[1].Split(')')[0];
                    skill.AppendChild(CharConverter.CreateElement(outChar.OwnerDocument, "sublabel", "string", value));
                }
            }
        }

        static String ParseClassName(String className)
        {
            if (className.Equals("Abjurer")
            || className.Equals("Conjurer")
            || className.Equals("Diviner")
            || className.Equals("Enchanter")
            || className.Equals("Evoker")
            || className.Equals("Illusionist")
            || className.Equals("Necromancer")
            || className.Equals("Transmuter"))
                className = "Wizard";

            return className;
        }

        static public void processSpecialabilitylist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value;
            var ApiAtributes = app.ParseApiAttributes(call);
            String characterClasses = "classes";
            String characterName = "name";

            if (ApiAtributes.ContainsKey("classes"))
                characterClasses = ApiAtributes["classes"];
            if (ApiAtributes.ContainsKey("name"))
                characterName = ApiAtributes["name"];

            foreach (XmlNode classnode in outChar.SelectSingleNode(characterClasses).ChildNodes)
            {
                value = app.ValueOf(classnode, "name");
                //Checks for speciality
                if (!value.Equals(ParseClassName(value)))
                {
                    app.Set(classnode, characterName, ParseClassName(value));
                    XmlNode spabilitylist = targetNode;
                    XmlNode specialty = outChar.OwnerDocument.CreateElement(CharConverter.CreateId(spabilitylist));
                    String specialtyname = value;

                    value = "Specialty: " + value;
                    specialty.AppendChild(CharConverter.CreateElement(outChar.OwnerDocument, "value", "string", value));

                    value = "The specialty for the " + ParseClassName(specialtyname) + "class is " + specialtyname + ".";
                    specialty.AppendChild(CharConverter.CreateElement(outChar.OwnerDocument, "description", "string", value));

                    if (spabilitylist.ChildNodes.Count > 0)
                        spabilitylist.InsertBefore(specialty, spabilitylist.ChildNodes[0]);
                    else
                        spabilitylist.AppendChild(specialty);
                }
            }
        }

        static public void processSpeedSpecial(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            //speed/special
            String value = targetNode.InnerText;
            var strbu = new StringBuilder();
            String[] speedspecial = value.Split(':');
            for (int i = 1; i < speedspecial.Length; i += 2)
            {
                strbu.Append(speedspecial[i]);
                strbu.Append(i + 2 >= speedspecial.Length ? "." : ", ");
            }
            targetNode.InnerText = strbu.ToString();
        }

        static public void processSpellset(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            foreach (XmlNode spellset in targetNode.ChildNodes)
            {
                if (spellset.SelectSingleNode("levels") == null)
                    continue;
                foreach (XmlNode level in spellset.SelectSingleNode("levels").ChildNodes)
                {
                    foreach (XmlNode spell in level.SelectSingleNode("spells").ChildNodes)
                    {
                        String value = app.ValueOf(spell, "school");
                        String[] school = value.Split(':');
                        var sb = new StringBuilder();
                        sb.Append(school[0]);
                        var innerbra = new StringBuilder();
                        for (int i = 1; i < school.Length; i++)
                        {
                            value = school[i];
                            if (!String.IsNullOrEmpty(value))
                                innerbra.Append(value);
                            if (i + 1 < school.Length
                            && !String.IsNullOrEmpty(school[i + 1]))
                                innerbra.Append(",");
                        }
                        if (!String.IsNullOrEmpty(innerbra.ToString()))
                            sb.Append(" [").Append(innerbra.ToString()).Append("]");
                        app.Set(spell, "school", sb.ToString());
                    }
                }
            }

            XmlNodeList nodelist = targetNode.ChildNodes;
            for (int i = nodelist.Count - 1; i >= 0; i--)
            {
                if (nodelist[i].ChildNodes.Count <= 1)
                    targetNode.RemoveChild(nodelist[i]);
            }
        }

    }
}
