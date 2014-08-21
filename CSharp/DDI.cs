// Copyright (c) 2012, SmiteWorks USA LLC

using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;

namespace CharacterConverter
{
    class DDI
    {
        public const String Format = @"D&D Insider (.dnd4e)";

        public const String Filter = @"D&D Insider (*.dnd4e)|*.dnd4e";

        public const String Xslt = @"\Dndi-4E.xslt";

        public const String CharPath = @"D20Character";
        public const String NamePath = @"CharacterSheet/Details/name";

        static public void CheckFile(XmlDocument doc, StringBuilder errorMessage)
        {
            if (doc.SelectSingleNode(CharPath) == null)
                errorMessage.Append("Invalid DNDI file.");
        }

        static String Normalize(String str)
        {
            return str.Trim();
        }

        static String AppendSpecial(String spe, String s)
        {
            var sb = new StringBuilder();
            sb.Append(spe);
            if (String.IsNullOrEmpty(s))
                return sb.ToString();
            if (!String.IsNullOrEmpty(spe))
                sb.Append(" ");
            sb.Append(s);
            sb.Append(";");
            return sb.ToString();
        }

        static int CalculateLargestModifier(CharConverter app, XmlNode dndi, String mod1, String mod2)
        {
            int iMod1 = CharConverter.ConvertToInt(app.ValueOf(dndi, "CharacterSheet/StatBlock/Stat[alias/@name='" + mod1 + " modifier']/@value"));
            int iMod2 = CharConverter.ConvertToInt(app.ValueOf(dndi, "CharacterSheet/StatBlock/Stat[alias/@name='" + mod2 + " modifier']/@value"));

            if (iMod1 > iMod2)
                return iMod1;
            return iMod2;
        }

        static void TallyRulesElements(CharConverter app, XmlNodeList nodeList, XmlNode targetNode, String type, String innerElement = "value")
        {
            XmlNode pathElement = targetNode;
            XmlDocument doc = targetNode.OwnerDocument;
            for (int i = 1; i < nodeList.Count; i++)
            {
                XmlNode rulesElement = nodeList[i];
                XmlElement tally = doc.CreateElement(CharConverter.CreateId(i));

                if (type.Equals("proficiencyarmor") || type.Equals("proficiencyweapon"))
                {
                    String name = app.ValueOf(rulesElement, "@name", "TallyRulesElements(proficiencyarmor||proficiencyweapon)");
                    try
                    {
                        if (name.Contains("(") && name.Contains(")"))
                        {
                            name = (name.Split('(')[1]).Split(')')[0];
                        }
                    }
                    catch (Exception)
                    {
                        name = "";
                    }
                    tally.AppendChild(CharConverter.CreateElement(doc, innerElement, "string", name));
                }
                else if (type.Equals("featlist") || type.Equals("specialabilitylist"))
                {
                    String value = app.ValueOf(rulesElement, "@name", "TallyRulesElements(featlist||specialabilitylist)");
                    tally.AppendChild(CharConverter.CreateElement(doc, innerElement, "string", value));
                    String error = "TallyRulesElements(" + value + ")";
                    value = app.ValueOf(rulesElement.LastChild, ".", error, false);
                    if (String.IsNullOrEmpty(value))
                        value = app.ValueOf(rulesElement, "specific[@name='Short Description']", error);
                    tally.AppendChild(CharConverter.CreateElement(doc, "description", "string", value));
                    value = app.ValueOf(rulesElement, "@type", error);
                    tally.AppendChild(CharConverter.CreateElement(doc, "source", "string", value));
                }
                else
                {
                    String value = app.ValueOf(rulesElement, "@name", "TallyRulesElements()");
                    tally.AppendChild(CharConverter.CreateElement(doc, innerElement, "string", value));
                    String error = "TallyRulesElements(" + value + ")";
                    value = app.ValueOf(rulesElement, "@type", error);
                    tally.AppendChild(CharConverter.CreateElement(doc, "source", "string", value));
                }

                /* DEBUG
                //shortcut
                XmlElement shortcut = CharConverter.CreateElement(doc, "shortcut", "windowreference");
                shortcut.AppendChild(doc.CreateElement("class"));
                shortcut.AppendChild(doc.CreateElement("recordname"));
                tally.AppendChild(shortcut);
                */

                pathElement.AppendChild(tally);
            }
        }

        static public void processClassBase(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value = app.ValueOf(inChar, "CharacterSheet/RulesElementTally/RulesElement[@type='Class']/@name", false);
            if (String.IsNullOrEmpty(value))
                value = app.ValueOf(inChar, "Level/RulesElement[@type='Level' and @name='1']/RulesElement[@type='Class']/@name", false);
            if (String.IsNullOrEmpty(value))
                value = app.ValueOf(inChar, "CharacterSheet/StatBlock/Stat[alias/@name='Default Class']/statadd/@String", false);
            targetNode.InnerText = value.Trim();
        }

        static public void processClassParagon(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value = app.ValueOf(inChar, "CharacterSheet/RulesElementTally/RulesElement[@type='Paragon Path']/@name", false);
            if (String.IsNullOrEmpty(value))
                value = app.ValueOf(inChar, "Level/RulesElement[@type='Level' and @name='11']/RulesElement[@type='Paragon Path']/@name", false);
            if (String.IsNullOrEmpty(value))
                value = app.ValueOf(inChar, "CharacterSheet/StatBlock/Stat[alias/@name='Default Paragon Path']/statadd/@String", false);
            targetNode.InnerText = value.Trim();
        }

        static public void processClassEpic(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value = app.ValueOf(inChar, "CharacterSheet/RulesElementTally/RulesElement[@type='Epic Destiny']/@name", false);
            if (String.IsNullOrEmpty(value))
                value = app.ValueOf(inChar, "Level/RulesElement[@type='Level' and @name='21']/RulesElement[@type='Epic Destiny']/@name", false);
            if (String.IsNullOrEmpty(value))
                value = app.ValueOf(inChar, "CharacterSheet/StatBlock/Stat[alias/@name='Default Epic Destiny']/statadd/@String", false);
            targetNode.InnerText = value.Trim();
        }

        static public void processEncumbranceHeavyarmor(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value = "0";
            foreach (XmlNode heavyarmor in inChar.SelectNodes("CharacterSheet/LootTally/loot[@equip-count='1']"))
            {
                if (!app.ValueOf(heavyarmor, "RulesElement/@type").Contains("Armor"))
                    continue;

                String type = app.ValueOf(heavyarmor, "RulesElement/specific[@name='Armor Type']").ToLower();
                if (type.Contains("heavy"))
                {
                    value = "1";
                    break;
                }
            }
            targetNode.InnerText = value;
        }

        static public void processFeatlist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlNodeList nodeList = inChar.SelectNodes("CharacterSheet/RulesElementTally/RulesElement[@type='Feat' and not(contains(@name, 'Proficiency'))]");
            TallyRulesElements(app, nodeList, targetNode, "featlist");
        }

        static public void processPowers(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;
/* DEBUG
            var categories = new List<string> 
            {
                "a1-atwill", 
                "a11-atwillspecial", 
                "a2-cantrip", 
                "b1-encounter", 
                "b11-encounterspecial", 
                "b2-channeldivinity",
                "c-daily",
                "d-utility",
                "e-itemdaily",
                "a0-situational"
            };

            foreach (String cate in categories)
            {
                XmlNode powerCate = targetNode.OwnerDocument.CreateElement(cate);
                powerCate.AppendChild(targetNode.OwnerDocument.CreateElement("power"));
                targetNode.AppendChild(powerCate);
            }

            XmlNode atwill = targetNode.SelectSingleNode(categories[0] + "/power");
            //XmlNode atwillspecial = targetNode.SelectSingleNode(categories[1] + "/power");
            XmlNode cantrip = targetNode.SelectSingleNode(categories[2] + "/power");
            XmlNode encounter = targetNode.SelectSingleNode(categories[3] + "/power");
            XmlNode encounterspecial = targetNode.SelectSingleNode(categories[4] + "/power");
            XmlNode channeldivinity = targetNode.SelectSingleNode(categories[5] + "/power");
            XmlNode daily = targetNode.SelectSingleNode(categories[6] + "/power");
            XmlNode utility = targetNode.SelectSingleNode(categories[7] + "/power");
            XmlNode itemdaily = targetNode.SelectSingleNode(categories[8] + "/power");
            XmlNode situational = targetNode.SelectSingleNode(categories[9] + "/power");
*/
            String value;
            foreach (XmlNode power in inChar.SelectNodes("CharacterSheet/PowerStats/*"))
            {
                if (power.HasChildNodes)
                {
                    /* DEBUG
                    XmlNode target;
                    */

                    //Figure out the powertype for sorting
                    String powerName = app.ValueOf(power, "@name");
                    if (RS_4E.CheckForExistingPower(app, outChar, powerName) != null)
                        continue;
                    String error = "AppendPowers(" + powerName + ")";

                    var sDescription = new StringBuilder();
                    XmlNodeList powerspecifics = power.ChildNodes;
                    for (int i = 0; i < powerspecifics.Count; i++)
                    {
                        XmlNode specific = powerspecifics[i];

                        //Check if the node has relevant information (only specific nodes do)
                        if (!specific.Name.Equals("specific"))
                            continue;

                        //Skip the irrelevant specific nodes
                        String name = app.ValueOf(specific, "@name", error);
                        if (name.StartsWith("_")
                        || name.Equals("Power Usage")
                        || name.Equals("Flavor")
                        || name.Equals("Action Type")
                        || name.Equals("Attack Type")
                        || name.Equals("Class")
                        || name.Equals("Level")
                        || name.Equals("Power Type")
                        || name.Equals("Display")
                        || name.Equals("Keywords"))
                            continue;

                        //Add the node's information to the stringbuilder
                        sDescription.Append(name);
                        sDescription.Append(":");
                        sDescription.Append(specific.InnerText);

                        if (i + 1 < power.ChildNodes.Count)
                            sDescription.Append("\\r");
                    }

                    /* DEBUG
                    String powerUsage = app.ValueOf(power, "specific[@name='Power Usage']", error);
                    String powerType = app.ValueOf(power, "specific[@name='Power Type']", error, false);

                    if (powerName.Contains("Channel Divinity"))
                        target = channeldivinity;
                    else if (powerUsage.Contains("Encounter (Special)"))
                        target = encounterspecial;
                    else if (powerType.Contains("Utility"))
                        target = utility;
                    else if (powerType.Contains("Cantrip"))
                        target = cantrip;
                    else if (powerUsage.Contains("Encounter"))
                        target = encounter;
                    else if (powerUsage.Contains("Daily"))
                        target = daily;
                    else if (powerUsage.Contains("At-Will"))
                        target = atwill;
                    else //Fallback in case of lack of info
                        target = daily;

                    XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(target));
                    */
                    /* DEBUG */
                    XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(targetNode));

                    value = Normalize(app.ValueOf(power, "specific[@name='Action Type']", error));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "action", "string", value));

                    value = Normalize(app.ValueOf(power, "specific[@name='Keywords']", error));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "keywords", "string", value));

                    value = Normalize(app.ValueOf(power, "@name", error));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "name", "string", value));

                    convertedPower.AppendChild(CharConverter.CreateElement(output, "prepared", "number", "1"));

                    value = Normalize(app.ValueOf(power, "specific[@name='Attack Type']", error));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "range", "string", value));

                    value = Normalize(app.ValueOf(power, "specific[@name='Power Usage']", error));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "recharge", "string", value));

                    value = Normalize(app.ValueOf(power, "specific[@name='Display']", error));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "source", "string", value));

                    value = Normalize(app.ValueOf(power, "specific[@name='Flavor']", error, false));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "flavor", "string", value));

                    convertedPower.AppendChild(CharConverter.CreateElement(output, "used", "number", "0"));

                    convertedPower.AppendChild(CharConverter.CreateElement(output, "parse", "number", "1"));

                    convertedPower.AppendChild(CharConverter.CreateElement(output, "shortdescription", "string", sDescription.ToString()));

                    XmlElement shortcut = CharConverter.CreateElement(output, "shortcut", "windowreference");
                    shortcut.AppendChild(output.CreateElement("class"));
                    shortcut.AppendChild(output.CreateElement("recordname"));
                    convertedPower.AppendChild(shortcut);

                    /* DEBUG
                    target.AppendChild(convertedPower);
                    */
                    /* DEBUG */
                    targetNode.AppendChild(convertedPower);
                }
            }

            bool cFeatures = app.outputOptions.CheckedItems.Contains(CharConverter.OutputOptDNDI[0]);
            bool feats = app.outputOptions.CheckedItems.Contains(CharConverter.OutputOptDNDI[1]);
            if (cFeatures || feats)
            {
                var sb = new StringBuilder();
                if (cFeatures)
                    sb.Append("@type='Class Feature'");
                if (feats)
                {
                    if (sb.Length > 0)
                        sb.Append(" or ");

                    sb.Append("@type='Feat'");
                }

                foreach (XmlNode classFeature in inChar.SelectNodes("CharacterSheet/RulesElementTally/RulesElement[" + sb + "]"))
                {
                    String name = Normalize(app.ValueOf(classFeature, "@name"));
                    //name = RemoveInvalidTokens(name);
                    if (RS_4E.CheckForExistingPower(app, outChar, name) != null)
                        continue;
                    String type = app.ValueOf(classFeature, "@type");

                    //value = ValueOf(inChar, "CharacterSheet/RulesElementTally/RulesElement[@type='Power' and @name='" + name + "']");
                    // if (!String.IsNullOrEmpty(value))
                    // continue;

                    /* DEBUG
                    XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(situational));
                    */
                    /* DEBUG */
                    XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(targetNode));

                    //action
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "action", "string"));

                    //keywords
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "keywords", "string"));

                    //name
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "name", "string", name));

                    //prepared
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "prepared", "number", "1"));

                    //range
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "range", "string"));

                    //recharge
                    /* DEBUG
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "recharge", "string", "Situational"));
                    */
                    /* DEBUG */
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "recharge", "string", ""));

                    //source
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "source", "string", type));

                    //flavor
                    value = Normalize(app.ValueOf(classFeature, "specific[@name='Short Description']", name));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "flavor", "string", value));

                    //used
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "used", "number", "0"));

                    //parse
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "parse", "number", "1"));

                    //shortdescription
                    value = app.ValueOf(classFeature.LastChild, ".");
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "shortdescription", "string", value));

                    /* DEBUG
                    situational.AppendChild(convertedPower);
                    */
                    /* DEBUG */
                    targetNode.AppendChild(convertedPower);
                }
            }

            bool appendItemProperties = app.outputOptions.CheckedItems.Contains(CharConverter.OutputOptDNDI[2]);
            foreach (XmlNode loot in inChar.SelectNodes("CharacterSheet/LootTally/loot[@count>0]"))
            {
                String lootName = app.ValueOf(loot, "RulesElement[last()]/@name");

                //Adds the item power to the powers page
                XmlNode power = loot.SelectSingleNode("RulesElement[last()]/specific[@name='Power']");
                if (power != null && !String.IsNullOrEmpty(power.InnerText))
                {
                    if (RS_4E.CheckForExistingPower(app, outChar, lootName) != null)
                        continue;

                    String powerDesc = power.InnerText;

                    Regex rx = new Regex(@"Power \* (\w+) \((\w+) Action\)\n(.*)");

                    Match match = rx.Match(powerDesc);
                    if (match.Success)
                    {
                        String recharge = match.Groups[1].Value;
                        String action = match.Groups[2].Value;
                        String desc = match.Groups[3].Value;

                        /* DEBUG
                        XmlNode target;
                        if (recharge == "Daily")
                            target = itemdaily;
                        else if (recharge == "Encounter")
                            target = encounter;
                        else
                            target = utility;

                        XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(target));
                        */
                        /* DEBUG */
                        XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(targetNode));

                        //action
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "action", "string", action));
                        //keywords
                        value = Normalize(app.ValueOf(loot, "RulesElement[last()]/specific[@name='Magic Item Type']", lootName));
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "keywords", "string", value));
                        //name
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "name", "string", lootName));
                        //prepared
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "prepared", "number", "1"));
                        //range
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "range", "string"));
                        //recharge
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "recharge", "string", recharge));
                        //source
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "source", "string", "Item"));
                        //flavor
                        value = Normalize(app.ValueOf(loot, "RulesElement[last()]/specific[@name='Flavor']", lootName));
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "flavor", "string", value));
                        //parse
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "parse", "number", "1"));
                        //shortdescription
                        convertedPower.AppendChild(CharConverter.CreateElement(output, "shortdescription", "string", desc));

                        /* DEBUG
                        target.AppendChild(convertedPower);
                        */
                        /* DEBUG */
                        targetNode.AppendChild(convertedPower);
                    }
                    else
                        app.AppendConsole(String.Format("Item Power Parsing Error. <Name:{0}>", lootName));


                    //XmlNode target;
                    //String recharge;
                    //if (powerDesc.ToLower().Contains("daily"))
                    //{
                    //    target = itemdaily;
                    //    recharge = "Daily";
                    //}
                    //else if (powerDesc.ToLower().Contains("encounter"))
                    //{
                    //    target = encounter;
                    //    recharge = "Encounter";
                    //}
                    //else
                    //{
                    //    target = utility;
                    //    recharge = "At-Will";
                    //}

                    //XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(target));


                    //String[] temp = powerDesc.Split(':')[1].Split('.')[0].Split(' ');
                    //StringBuilder sb = new StringBuilder();
                    //for (int i = 0; i < temp.Length; i++)
                    //{
                    //    String tempEntry = temp[i];

                    //    if (tempEntry.Equals("Action"))
                    //        continue;

                    //    sb.Append(tempEntry);

                    //    if (i != temp.Length - 1)
                    //        sb.Append(" ");
                    //}
                    //String action = sb.ToString();
                    //String shortDescription = Normalize(powerDesc.Split('.')[1]);

                    ////action
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "action", "string", action));

                    ////keywords
                    //value = Normalize(app.ValueOf(loot, "RulesElement[last()]/specific[@name='Magic Item Type']", lootName));
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "keywords", "string", value));

                    ////name
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "name", "string", lootName));

                    ////prepared
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "prepared", "number", "1"));

                    ////range
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "range", "string"));

                    ////recharge
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "recharge", "string", recharge));

                    ////source
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "source", "string", "Item"));

                    ////flavor
                    //value = Normalize(app.ValueOf(loot, "RulesElement[last()]/specific[@name='Flavor']", lootName));
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "flavor", "string", value));

                    ////used
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "used", "number", "0"));

                    ////parse
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "parse", "number", "1"));

                    ////shortdescription
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "shortdescription", "string", shortDescription));

                    ////shortcut
                    //XmlElement shortcutNode = CharConverter.CreateElement(output, "shortcut", "windowreference");
                    //shortcutNode.AppendChild(output.CreateElement("class"));
                    //shortcutNode.AppendChild(output.CreateElement("recordname"));
                    //convertedPower.AppendChild(shortcutNode);

                    //target.AppendChild(convertedPower);
                }

                String itemProperty = app.ValueOf(loot, "RulesElement[last()]/specific[@name='Property']", false);
                if (appendItemProperties && !String.IsNullOrEmpty(itemProperty))
                {
                    /* DEBUG
                    XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(situational));
                    */
                    /* DEBUG */
                    XmlElement convertedPower = output.CreateElement(CharConverter.CreateId(targetNode));

                    //action
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "action", "string"));
                    //keywords
                    value = Normalize(app.ValueOf(loot, "RulesElement[last()]/specific[@name='Magic Item Type']", lootName));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "keywords", "string", value));
                    //name
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "name", "string", lootName));
                    //prepared
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "prepared", "number", "1"));
                    //range
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "range", "string"));
                    //recharge
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "recharge", "string", "Situational"));
                    //source
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "source", "string", "Item"));
                    //flavor
                    value = Normalize(app.ValueOf(loot, "RulesElement[last()]/specific[@name='Flavor']", lootName));
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "flavor", "string", value));
                    //used
                    //convertedPower.AppendChild(CharConverter.CreateElement(output, "used", "number", "0"));
                    //parse
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "parse", "number", "1"));
                    //shortdescription
                    convertedPower.AppendChild(CharConverter.CreateElement(output, "shortdescription", "string", itemProperty));
                    //shortcut
                    //XmlElement shortcutNode = CharConverter.CreateElement(output, "shortcut", "windowreference");
                    //shortcutNode.AppendChild(output.CreateElement("class"));
                    //shortcutNode.AppendChild(output.CreateElement("recordname"));
                    //convertedPower.AppendChild(shortcutNode);

                    /* DEBUG
                    situational.AppendChild(convertedPower);
                    */
                    /* DEBUG */
                    targetNode.AppendChild(convertedPower);
                }
            }
        }

        static public void processProficiencyarmor(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlNodeList nodeList = inChar.SelectNodes("CharacterSheet/RulesElementTally/RulesElement[@type='Proficiency' and ( contains(@name, 'Armor') or contains(@name, 'Shield') )]");
            TallyRulesElements(app, nodeList, targetNode, "proficiencyarmor");
        }

        static public void processProficiencyweapon(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlNodeList nodeList = inChar.SelectNodes("CharacterSheet/RulesElementTally/RulesElement[@type='Proficiency' and not(contains(@name, 'Armor')) and not(contains(@name, 'Shield'))]");
            TallyRulesElements(app, nodeList, targetNode, "proficiencyweapon");
        }

        static public void processSpecial(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            //contains(alias/@name, 'Resist:') or contains(alias/@name, 'Vulnerable:') or contains(alias/@name, 'Immune:')
            String value;
            String specialstr = "";
            StringBuilder sb = new StringBuilder();
            XmlNodeList nodeList = inChar.SelectNodes("CharacterSheet/StatBlock/Stat[statadd/@type='resist']");
            if (nodeList.Count > 0)
                sb.Append("Resist ");
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode special = nodeList[i];
                try
                {
                    String name = app.ValueOf(special, "alias/@name");
                    name = name.Split(':')[1].ToLower();
                    value = app.ValueOf(special, "statadd/@value");

                    sb.Append(name);
                    sb.Append(" ");
                    sb.Append(value);

                    if (i + 2 < nodeList.Count)
                        sb.Append(", ");
                    else if (i + 1 < nodeList.Count)
                        sb.Append(" and ");
                }
                catch (Exception) { }
                i++;
            }
            specialstr = AppendSpecial(specialstr, sb.ToString());

            //Conditional Saving Throws
            foreach (XmlNode statadd in inChar.SelectNodes("CharacterSheet/StatBlock/stat[alias/@name='Saving Throws']/statadd"))
            {
                String conditional = app.ValueOf(statadd, "@conditional");

                if (String.IsNullOrEmpty(conditional))
                    continue;

                var consb = new StringBuilder();
                consb.Append("+");
                consb.Append(app.ValueOf(statadd, "@value"));
                consb.Append(" misc saving throw ");
                consb.Append(conditional);
                specialstr = AppendSpecial(specialstr, consb.ToString());
            }
            targetNode.InnerText = specialstr;
        }

        static public void processSenses(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            var sb = new StringBuilder();
            bool hasAppended = false;
            foreach (XmlNode vision in inChar.SelectNodes("CharacterSheet/RulesElementTally/RulesElement[@type='Vision']"))
            {
                String value = app.ValueOf(vision, "@name");

                if (value.Equals("Normal"))
                    continue;

                if (hasAppended)
                    sb.Append(",");

                sb.Append(value);

                hasAppended = true;
            }
            targetNode.InnerText = sb.ToString();
        }

        static public void processSpecialabilitylist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlNodeList nodeList = inChar.SelectNodes("CharacterSheet/RulesElementTally/RulesElement[@type='Racial Trait' or @type='Class Feature']");
            TallyRulesElements(app, nodeList, targetNode, "specialabilitylist");
        }

        static public void processSpeedArmor(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value = app.ValueOf(inChar, "CharacterSheet/number(translate(number(StatBlock/Stat[alias/@name='Speed']/statadd[@Level &gt; 0]/@value), 'Na', '0'))", false);
            int speedBase = CharConverter.ConvertToInt(Normalize(value));
            int speedArmor = 0;
            int speedMisc = 0;
            bool armor = false;
            foreach (XmlNode statadd in inChar.SelectNodes("CharacterSheet/StatBlock/Stat[alias/@name='Speed']/statadd"))
            {
                value = app.ValueOf(statadd, "@type", false);

                if (String.IsNullOrEmpty(value))
                    continue;

                if (app.ValueOf(statadd, "@type").Equals("Armor"))
                {
                    if (!armor)
                    {
                        speedArmor += CharConverter.ConvertToInt(app.ValueOf(statadd, "@value"));
                        armor = true;
                    }
                }
                else
                    speedMisc += CharConverter.ConvertToInt(app.ValueOf(statadd, "@value"));
            }
            targetNode.InnerText = Convert.ToString(speedArmor);
        }

        static public void processSpeedMisc(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value = app.ValueOf(inChar, "CharacterSheet/number(translate(number(StatBlock/Stat[alias/@name='Speed']/statadd[@Level &gt; 0]/@value), 'Na', '0'))", false);
            int speedBase = CharConverter.ConvertToInt(Normalize(value));
            int speedArmor = 0;
            int speedMisc = 0;
            bool armor = false;
            foreach (XmlNode statadd in inChar.SelectNodes("CharacterSheet/StatBlock/Stat[alias/@name='Speed']/statadd"))
            {
                value = app.ValueOf(statadd, "@type", false);

                if (String.IsNullOrEmpty(value))
                    continue;

                if (app.ValueOf(statadd, "@type").Equals("Armor"))
                {
                    if (!armor)
                    {
                        speedArmor += CharConverter.ConvertToInt(app.ValueOf(statadd, "@value"));
                        armor = true;
                    }
                }
                else
                    speedMisc += CharConverter.ConvertToInt(app.ValueOf(statadd, "@value"));
            }
            targetNode.InnerText = Convert.ToString(speedMisc);
        }

        static void AppendWeapons(CharConverter app, XmlNode inChar, XmlNodeList weapons, XmlNode targetNode, int level, String type)
        {
            XmlNode weaponList = targetNode;
            XmlDocument output = targetNode.OwnerDocument;
            int halfLevel = (int)Math.Floor((double)level / 2);
            String value;

            IEnumerator myEnum = weapons.GetEnumerator();
            while (myEnum.MoveNext())
            {
                XmlElement weapon = (XmlElement)(myEnum.Current);
                XmlElement convertedWeapon = output.CreateElement(CharConverter.CreateId(weaponList));
                String weaponName = app.ValueOf(weapon, "@name");

                //ammo
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "ammo", "number", "0"));

                //attackdef
                value = Normalize(app.ValueOf(weapon, "Defense", weaponName)).ToLower();
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "attackdef", "string", value));

                //attackstat
                value = Normalize(app.ValueOf(weapon, "AttackStat", weaponName)).ToLower();
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "attackstat", "string", value));

                //bonus
                int attackBonus = CharConverter.ConvertToInt(app.ValueOf(weapon, "AttackBonus", weaponName));
                int statBonus;
                try
                {
                    statBonus = CharConverter.ConvertToInt(app.ValueOf(weapon, "HitComponents", weaponName).Substring(2, 1));
                }
                catch (Exception)
                {
                    statBonus = 0;
                }
                attackBonus -= (halfLevel + statBonus);
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "bonus", "number", Convert.ToString(attackBonus)));

                String damage = app.ValueOf(weapon, "Damage");
                char damageSplit;
                if (damage.Contains("+"))
                    damageSplit = '+';
                else if (damage.Contains("-"))
                    damageSplit = '-';
                else
                    damageSplit = ' ';

                //damagebonus
                int damageBonus;
                try
                {
                    damageBonus = CharConverter.ConvertToInt(damage.Split(damageSplit)[1]);
                }
                catch (Exception)
                {
                    damageBonus = 0;
                }
                damageBonus -= statBonus;
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "damagebonus", "number", Convert.ToString(damageBonus)));

                //damagedice
                String diceValue;
                try
                {
                    value = Normalize(damage).Split('d')[0];
                    diceValue = Normalize(damage).Split('+')[0].Substring(value.Length);
                    if (level >= 21)
                        value = Convert.ToString(CharConverter.ConvertToInt(value) / 2);
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "damagedice", "dice", CharConverter.CreateDice(diceValue, value)));
                }
                catch (Exception)
                {
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "damagedice", "dice"));
                }

                //criticalbonus
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "criticalbonus", "number", "0"));

                String rangeincrement = null;
                String special = null;
                String properties = null;

                foreach (XmlNode node in weapon.SelectNodes("RulesElement"))
                {
                    String specificName = app.ValueOf(node, "@name");
                    //criticaldice
                    try
                    {
                        if (String.IsNullOrEmpty(rangeincrement))
                            rangeincrement = app.ValueOf(inChar, "CharacterSheet/LootTally/loot/RulesElement[@name='" + specificName + "']/specific[@name='Range']", false);

                        if (String.IsNullOrEmpty(special))
                            special = app.ValueOf(inChar, "CharacterSheet/LootTally/loot/RulesElement[@name='" + specificName + "']/specific[@name='Special']", false);

                        if (String.IsNullOrEmpty(properties))
                            properties = app.ValueOf(inChar, "CharacterSheet/LootTally/loot/RulesElement[@name='" + specificName + "']/specific[@name='Properties']", false);

                        value = app.ValueOf(inChar, "CharacterSheet/LootTally/loot/RulesElement[@name='" + specificName + "']/specific[@name='Critical']", false);
                        if (String.IsNullOrEmpty(value))
                            throw new Exception();
                        value = Normalize(value);

                        //Ensure this is crit dice
                        if (value.StartsWith("+"))
                        {
                            String[] crit = value.Split(' ');

                            //criticaldicetype
                            if (crit.Length > 2)
                            {
                                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "criticaldamagetype", "string", CharConverter.UppercaseFirst(crit[1])));
                            }
                            else
                            {
                                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "criticaldamagetype", "string"));
                            }

                            value = crit[0].Split('+')[1];
                            String diceNum = value.Substring(0, 1);
                            String diceVal = value.Substring(diceNum.Length);

                            convertedWeapon.AppendChild(CharConverter.CreateElement(output, "criticaldice", "dice", CharConverter.CreateDice(diceVal, diceNum)));
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                //rangeincrement
                if (type.Equals("1"))
                {
                    if (String.IsNullOrEmpty(rangeincrement))
                        convertedWeapon.AppendChild(CharConverter.CreateElement(output, "rangeincrement", "number", "0"));
                    else
                    {
                        try
                        {
                            value = Normalize(rangeincrement).Split('/')[0];
                            convertedWeapon.AppendChild(CharConverter.CreateElement(output, "rangeincrement", "number", value));
                        }
                        catch (Exception)
                        {
                            convertedWeapon.AppendChild(CharConverter.CreateElement(output, "rangeincrement", "number", "0"));
                        }
                    }
                }
                else
                {
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "rangeincrement", "number", "0"));
                }

                //special
                if (String.IsNullOrEmpty(special))
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "special", "string"));
                else
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "special", "string", Normalize(special)));

                //properties
                if (String.IsNullOrEmpty(properties))
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "properties", "string"));
                else
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "properties", "string", Normalize(properties)));

                //damagestat
                try
                {
                    value = Normalize(app.ValueOf(weapon, "DamageComponents")).Split(' ')[1].ToLower();
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "damagestat", "string", value));
                }
                catch (Exception)
                {
                    convertedWeapon.AppendChild(CharConverter.CreateElement(output, "damagestat", "string"));
                }

                //maxammo
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "maxammo", "number", "0"));

                //name
                value = app.ValueOf(weapon, "@name");
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "name", "string", value));

                //nodename
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "nodename", "string"));

                //nodetype
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "nodetype", "string"));

                //order
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "order", "number", Convert.ToString(weaponList.ChildNodes.Count + 1)));

                //shortcut
                XmlElement shortcut = CharConverter.CreateElement(output, "shortcut", "windowreference");
                shortcut.AppendChild(output.CreateElement("class"));
                shortcut.AppendChild(output.CreateElement("recordname"));
                convertedWeapon.AppendChild(shortcut);

                //type
                convertedWeapon.AppendChild(CharConverter.CreateElement(output, "type", "number", type));

                weaponList.AppendChild(convertedWeapon);
            }
        }

        static public void processWeaponlist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlNode weaponList = targetNode;
            XmlDocument output = outChar.OwnerDocument;
            int level = CharConverter.ConvertToInt(Normalize(app.ValueOf(inChar, "CharacterSheet/Details/Level")));
            XmlNodeList nodeList = inChar.SelectNodes("CharacterSheet/PowerStats/Power[@name='Melee Basic Attack']/Weapon[@name!='Unarmed']");
            AppendWeapons(app, inChar, nodeList, targetNode, level, "0");
            nodeList = inChar.SelectNodes("CharacterSheet/PowerStats/Power[@name='Ranged Basic Attack']/Weapon[@name!='Unarmed']");
            AppendWeapons(app, inChar, nodeList, targetNode, level, "1");

            foreach (XmlNode magicItem in inChar.SelectNodes("CharacterSheet/LootTally/loot[@count>0]/RulesElement[@type='Magic Item']"))
            {
                if (!app.ValueOf(magicItem, "specific[@name='Item Slot']", false).Contains("Off-Hand"))
                    continue;

                XmlElement convertedImplement = output.CreateElement(CharConverter.CreateId(weaponList));

                String magicItemType = Normalize(app.ValueOf(magicItem, "specific[@name='Magic Item Type']")).ToLower();

                try
                {
                    String critDice = Normalize(app.ValueOf(magicItem, "specific[@name='Critical']"));
                    critDice = critDice.Split(' ')[0];
                    critDice = critDice.Remove(0, 1);
                    critDice = CharConverter.CreateDice(critDice.Substring(critDice.IndexOf("d")), critDice.Split('d')[0]);
                    convertedImplement.AppendChild(CharConverter.CreateElement(output, "criticaldice", "dice", critDice));
                }
                catch (Exception) { }

                int attackBonus = 0;
                int damageBonus = 0;
                int critRangeBonus = 0;
                XmlNodeList stats = inChar.SelectNodes("CharacterSheet/StatBlock/Stat");
                IEnumerator statEnum = stats.GetEnumerator();
                while (statEnum.MoveNext())
                {
                    XmlNode stat = (XmlNode)statEnum.Current;
                    String alias = app.ValueOf(stat, "alias/@name");

                    //AttackBonus
                    if (alias.Contains(magicItemType)
                    && alias.Contains("implement")
                    && alias.Contains(":attack"))
                    {
                        attackBonus += CharConverter.ConvertToInt(app.ValueOf(stat, "@value"));
                    }

                    if (alias.Contains(magicItemType)
                    && alias.Contains("implement")
                    && alias.Contains(":crit-range"))
                    {
                        int temp = CharConverter.ConvertToInt(app.ValueOf(stat, "@value"));
                        if (temp > critRangeBonus)
                            critRangeBonus =
                        critRangeBonus += CharConverter.ConvertToInt(app.ValueOf(stat, "@value"));
                    }

                    if (alias.Contains(magicItemType)
                    && alias.Contains("implement")
                    && alias.Contains(":damage"))
                    {
                        damageBonus += CharConverter.ConvertToInt(app.ValueOf(stat, "@value"));
                    }
                }

                String enhancement = Normalize(app.ValueOf(magicItem, "specific[@name='Enhancement']"));
                if (enhancement.Contains("attack rolls")
                && enhancement.Contains("+")
                && enhancement.Contains("damage rolls"))
                {
                    enhancement = enhancement.Split(' ')[0];
                    enhancement = enhancement.Substring(1);

                    attackBonus += CharConverter.ConvertToInt(enhancement);
                    damageBonus += CharConverter.ConvertToInt(enhancement);
                }

                //name
                convertedImplement.AppendChild(CharConverter.CreateElement(output, "name", "string", app.ValueOf(magicItem, "@name")));

                //type
                convertedImplement.AppendChild(CharConverter.CreateElement(output, "type", "number", "1"));

                //order
                convertedImplement.AppendChild(CharConverter.CreateElement(output, "order", "number", Convert.ToString(weaponList.ChildNodes.Count + 1)));

                //bonus
                convertedImplement.AppendChild(CharConverter.CreateElement(output, "bonus", "number", Convert.ToString(attackBonus)));

                //properties
                convertedImplement.AppendChild(CharConverter.CreateElement(output, "properties", "string", "Crit Range " + Convert.ToString(20 - critRangeBonus)));

                //damagebonus
                convertedImplement.AppendChild(CharConverter.CreateElement(output, "damagebonus", "number", Convert.ToString(damageBonus)));

                weaponList.AppendChild(convertedImplement);
            }
        }

        static public void processInventorylist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;
            String value;
            XmlNodeList nodeList = inChar.SelectNodes("CharacterSheet/LootTally/loot[@count>0]");
            XmlNode inventorylist = targetNode;
            for (int index = 0; index < nodeList.Count; index++)
            {
                XmlNode loot = nodeList[index];
                XmlNode convertedLoot = output.CreateElement(CharConverter.CreateId(index));

                String type = app.ValueOf(loot, "RulesElement/@type");

                String lootName = app.ValueOf(loot, "RulesElement[last()]/@name");
                if (type.Equals("Weapon") && loot.SelectNodes("RulesElement").Count == 2)
                {
                    String weaponName = app.ValueOf(loot, "RulesElement/@name");
                    lootName = lootName.Replace("Weapon", weaponName);
                }

                //carried
                convertedLoot.AppendChild(CharConverter.CreateElement(output, "carried", "number", "1"));

                //count
                convertedLoot.AppendChild(CharConverter.CreateElement(output, "count", "number", app.ValueOf(loot, "@count")));

                //name
                convertedLoot.AppendChild(CharConverter.CreateElement(output, "name", "string", lootName));

                //nodename
                convertedLoot.AppendChild(CharConverter.CreateElement(output, "nodename", "string"));

                //nodetype
                convertedLoot.AppendChild(CharConverter.CreateElement(output, "nodetype", "string"));

                //shortcut
                XmlElement shortcut = CharConverter.CreateElement(output, "shortcut", "windowreference");
                shortcut.AppendChild(output.CreateElement("class"));
                shortcut.AppendChild(output.CreateElement("recordname"));
                convertedLoot.AppendChild(shortcut);

                //showonminisheet
                convertedLoot.AppendChild(CharConverter.CreateElement(output, "showonminisheet", "number", "0"));

                //weight
                value = Normalize(app.ValueOf(loot, "RulesElement[1]/specific[@name='Weight']", lootName, false));
                convertedLoot.AppendChild(CharConverter.CreateElement(output, "weight", "number", value));

                //location
                if (type.Equals("Ritual"))
                {
                    convertedLoot.AppendChild(CharConverter.CreateElement(output, "location", "string", "Ritual"));
                }
                else
                {
                    value = app.ValueOf(loot, "RulesElement[1]/specific[@name='Item Slot']", false);

                    if (String.IsNullOrEmpty(value))
                        convertedLoot.AppendChild(CharConverter.CreateElement(output, "location", "string"));
                    else
                        convertedLoot.AppendChild(CharConverter.CreateElement(output, "location", "string", Normalize(value)));
                }

                //subclass
                //convertedLoot.AppendChild(output.CreateElement("subclass"));

                inventorylist.AppendChild(convertedLoot);
            }
        }

        static public void processCoins(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;
            String coins = app.ValueOf(inChar, "textstring[@name='_PER_LEVEL_1_Carried Money']");
            String[] coinsSplit = coins.Split(' ');
            List<String> coinTypes = new List<string> { "gp", "pp", "cp", "sp" };
            for (int i = 0; i < coinsSplit.Length - 1; i++)
            {
                String amount = coinsSplit[i];
                String type = coinsSplit[i + 1];
                type = type.Replace("\n", "");
                int tempInt;
                if (int.TryParse(amount, out tempInt) && coinTypes.Contains(type))
                {
                    XmlNode slot = output.CreateElement("slot" + (targetNode.ChildNodes.Count));
                    slot.AppendChild(CharConverter.CreateElement(output, "amount", "number", amount));
                    slot.AppendChild(CharConverter.CreateElement(output, "name", "string", type));
                    targetNode.AppendChild(slot);
                }
            }
        }

        static void AppendDefense(CharConverter app, XmlNode inChar, XmlNode node, XmlNode targetNode, String ability, int level)
        {
            XmlDocument output = targetNode.OwnerDocument;
            ability = ability.ToLower();
            String error = "AppendDefense(" + ability + ")";

            int halfLevel = (int)Math.Floor((double)level / 2);
            int baseInt = 10 + halfLevel;
            int armor = CharConverter.ConvertToInt(app.ValueOf(node, "statadd[@type='Armor']/@value", false));
            XmlNodeList abilityNodes = node.SelectNodes("statadd[@type='Ability']");
            String mod1 = app.ValueOf(abilityNodes[0], "@statlink");
            String mod2 = app.ValueOf(abilityNodes[1], "@statlink");
            int abilityScore = CalculateLargestModifier(app, inChar, mod1, mod2);

            int nTotal = CharConverter.ConvertToInt(Normalize(app.ValueOf(node, "@value")));
            int nMisc = nTotal - (abilityScore + armor + baseInt);

            XmlNode stat = targetNode;

            stat.AppendChild(CharConverter.CreateElement(output, "base", "number", Convert.ToString(baseInt)));

            stat.AppendChild(CharConverter.CreateElement(output, "armor", "number", Convert.ToString(armor)));

            stat.AppendChild(CharConverter.CreateElement(output, "misc", "number", Convert.ToString(nMisc)));

            stat.AppendChild(CharConverter.CreateElement(output, "temporary", "number", "0"));

            //stat.AppendChild(CharConverter.CreateElement(output, "total", "number", Convert.ToString(nTotal)));
        }

        static public void processDefensesAc(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            int level = CharConverter.ConvertToInt(Normalize(app.ValueOf(inChar, "CharacterSheet/Details/Level")));
            XmlNode node = inChar.SelectSingleNode("CharacterSheet/StatBlock/Stat[alias/@name='AC']");
            AppendDefense(app, inChar, node, targetNode, "AC", level);
        }

        static public void processDefensesFortitude(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            int level = CharConverter.ConvertToInt(Normalize(app.ValueOf(inChar, "CharacterSheet/Details/Level")));
            XmlNode node = inChar.SelectSingleNode("CharacterSheet/StatBlock/Stat[alias/@name='Fortitude Defense']");
            AppendDefense(app, inChar, node, targetNode, "Fortitude", level);
        }

        static public void processDefensesReflex(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            int level = CharConverter.ConvertToInt(Normalize(app.ValueOf(inChar, "CharacterSheet/Details/Level")));
            XmlNode node = inChar.SelectSingleNode("CharacterSheet/StatBlock/Stat[alias/@name='Reflex Defense']");
            AppendDefense(app, inChar, node, targetNode, "Reflex", level);
        }

        static public void processDefensesWill(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            int level = CharConverter.ConvertToInt(Normalize(app.ValueOf(inChar, "CharacterSheet/Details/Level")));
            XmlNode node = inChar.SelectSingleNode("CharacterSheet/StatBlock/Stat[alias/@name='Will Defense']");
            AppendDefense(app, inChar, node, targetNode, "Will", level);
        }

    }
}
