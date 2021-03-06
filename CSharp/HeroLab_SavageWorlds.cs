﻿// Copyright (c) 2012, SmiteWorks USA LLC

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using System.Xml;

namespace CharacterConverter
{
    class HeroLab_SavageWorlds
    {
        public const String Format = "Hero Lab (.xml)";

        public const String Filter = "Hero Lab (*.xml)|*.xml";

        public const String Xslt = @"\HeroLab-SavageWorlds.xslt";

        public const String CharPath = @"document/public/character";
        public const String NamePath = @"@name";


        static Hashtable edgeHashtable = new Hashtable();
        static Hashtable hindranceHashtable = new Hashtable();
        static Hashtable skillHashtable = new Hashtable();
        static Hashtable powerHashtable = new Hashtable();

        private static string storedArcaneType = "";

        static HeroLab_SavageWorlds()
        {
            /*
            string[] readLines = File.ReadAllLines(@"\SavageWorlds-Edges.txt");
            foreach (string line in readLines)
            {
                string key = line.Split(',')[0];
                string val = line.Split(',')[1];
                if (!edgeHashtable.ContainsKey(key))
                {
                    edgeHashtable.Add(key, val);
                }
            }
            readLines = File.ReadAllLines(@"\SavageWorlds-Hindrances.txt");
            foreach (string line in readLines)
            {
                string key = line.Split(',')[0];
                string val = line.Split(',')[1];
                if (!hindranceHashtable.ContainsKey(key))
                {
                    hindranceHashtable.Add(key, val);
                }
            }
            */
            edgeHashtable.Add("Brawny", "reference.edges.brawny@SW Deluxe Player Guide");
            hindranceHashtable.Add("Big Mouth", "reference.hindrances.bigmouth@SW Deluxe Player Guide");
            skillHashtable.Add("Boating", "reference.skills.baoting@SW Deluxe Player Guide");
            powerHashtable.Add("Armor", "powerdesc.armor@SW Deluxe Player Guide");
            powerHashtable.Add("Attack, Melee", "powerdesc.attackmelee@SW Super Powers");
            powerHashtable.Add("Chameleon", "powerdesc.chameleon@SW Super Powers");
            powerHashtable.Add("Invisibility", "powerdesc.invisibility@SW Super Powers");
            powerHashtable.Add("Parry", "powerdesc.parry@SW Super Powers");
        }
        static String convertName(String name)
        {
            return name.ToLower().Replace(" ", "").Replace("-", "");
        }
        static public String getEdgeReference(String edgeName)
        {
            if (edgeName.Contains("Arcane Background: "))
            {
                storedArcaneType = edgeName.Substring(edgeName.IndexOf(':') + 2);
            } 
            if (edgeName.Contains(":"))
            {
                edgeName = edgeName.Substring(0, edgeName.IndexOf(':'));
            }
            if (edgeHashtable.ContainsKey(edgeName))
            {
                return ((String)edgeHashtable[edgeName]);
            }
            else
            {
                return ("reference.edges." + convertName(edgeName) + "@SW Deluxe Player Guide");
            }
        }
        static public String getHindranceReference(String hindranceName)
        {
            if (hindranceName.Contains(":"))
            {
                hindranceName = hindranceName.Substring(0, hindranceName.IndexOf(':'));
            }
            if (hindranceHashtable.ContainsKey(hindranceName))
            {
                return ((String)hindranceHashtable[hindranceName]);
            }
            else
            {
                return ("reference.hindrances." + convertName(hindranceName) + "@SW Deluxe Player Guide");
            }
        }
        static public String getSkillReference(String skillName)
        {
            if (skillName.Contains(" ("))
            {
                skillName = skillName.Substring(0, skillName.IndexOf('(')-1);
            }
            if (skillName.Contains(":"))
            {
                skillName = skillName.Substring(0, skillName.IndexOf(':'));
            }
            if (skillHashtable.ContainsKey(skillName))
            {
                return ((String)skillHashtable[skillName]);
            }
            else
            {
                return ("reference.skills." + convertName(skillName) + "@SW Deluxe Player Guide");
            }
        }
        static public String getPowerReference(String powerName)
        {
            if (powerName.Contains(" ("))
            {
                powerName = powerName.Substring(0, powerName.IndexOf('(')-1);
            }
            if (powerName.Contains(":"))
            {
                powerName = powerName.Substring(0, powerName.IndexOf(':'));
            }
            if (powerHashtable.ContainsKey(powerName))
            {
                return ((String)powerHashtable[powerName]);
            }
            else
            {
                return ("powerdesc." + convertName(powerName) + "@SW Deluxe Player Guide");
            }
        }
        static public void CheckFile(XmlDocument doc, StringBuilder errorMessage)
        {
            if (doc.SelectSingleNode(CharPath) == null)
                errorMessage.Append("Invalid Hero Lab character file.");
        }

        static String parseClassName(String className)
        {
            var sb = new StringBuilder();
            String[] classNameSplit = className.Split(' ');
            for (int i = 0; i < classNameSplit.Length; i++)
            {
                sb.Append(classNameSplit[i]);

                if (i + 1 >= classNameSplit.Length
                || classNameSplit[i + 1].Contains("("))
                    break;

                sb.Append(" ");
            }
            return sb.ToString();
        }

        static String parseCastType(String type)
        {
            // "" = Prepared, "spontaneous" = Spontaneous, "points" = Points
            if (type.Equals("Spontaneous"))
                return "spontaneous";
            if (type.Equals("Points") || String.IsNullOrEmpty(type))
                return "points";

            return "";
        }

        static public void processArmorList(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;

            XmlNode node = targetNode;
            XmlNodeList nodeList = null;
            nodeList = inChar.SelectNodes("defenses/defense");

            foreach (XmlNode source in nodeList)
            {
                XmlNode converted = output.CreateElement(CharConverter.CreateId(node));
                node.AppendChild(converted);

                XmlNode areaprotected = CharConverter.CreateElement(output, "areaprotected", "string", "");
                converted.AppendChild(areaprotected);
                XmlNode armornotes = CharConverter.CreateElement(output, "armornotes", "string", "");
                converted.AppendChild(armornotes);

                string nameVal = app.ValueOf(source, "@name");
                string defense = app.ValueOf(source, "@defense");
                string equipped = app.ValueOf(source, "@equipped");
                string quantity = app.ValueOf(source, "@quantity");
                XmlNode weightNode = source.SelectSingleNode("weight");
                string weightVal = app.ValueOf(weightNode, "@value");

                XmlNode costNode = source.SelectSingleNode("cost");
                string costVal = app.ValueOf(costNode, "@value");

                string description = app.ValueOf(source, "description");

                XmlNode arweight = CharConverter.CreateElement(output, "arweight", "number", weightVal);
                converted.AppendChild(arweight);
                string carriedVal = "0";

                if (equipped.Equals("yes"))
                {
                    carriedVal = "1";
                }
                XmlNode carried = CharConverter.CreateElement(output, "carried", "number", carriedVal);
                converted.AppendChild(carried);
                XmlNode catname = CharConverter.CreateElement(output, "catname", "string", "Armor");
                converted.AppendChild(catname);
                XmlNode cost = CharConverter.CreateElement(output, "cost", "string", costVal);
                converted.AppendChild(cost);
                XmlNode count = CharConverter.CreateElement(output, "count", "number", quantity);
                converted.AppendChild(count);
                XmlNode group = CharConverter.CreateElement(output, "group", "string", "");
                converted.AppendChild(group);
                XmlNode isidentified = CharConverter.CreateElement(output, "isidentified", "string", "1");
                converted.AppendChild(isidentified);
                XmlNode name = CharConverter.CreateElement(output, "name", "string", nameVal);
                converted.AppendChild(name);
                XmlNode notes = CharConverter.CreateElement(output, "notes", "string", "");
                converted.AppendChild(notes);
                XmlNode protection = CharConverter.CreateElement(output, "protection", "string", defense.Replace("+", ""));
                converted.AppendChild(protection);
                XmlNode weight = CharConverter.CreateElement(output, "weight", "number", weightVal);
                converted.AppendChild(weight);

            }

        }




        static public void processPowerList(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;

            XmlNode node = targetNode;
            XmlNodeList nodeList = null;
            string[] powersLists = new string[2] { "arcanepowers/arcanepower", "superpowers/superpower" };

            foreach (string powerList in powersLists)
            {
                nodeList = inChar.SelectNodes(powerList);

                foreach (XmlNode source in nodeList)
                {
                    XmlNode converted = output.CreateElement(CharConverter.CreateId(node));
                    node.AppendChild(converted);

                    string nameVal = app.ValueOf(source, "@name");
                    string nameextVal = app.ValueOf(source, "@nameext");
                    string rangeVal = app.ValueOf(source, "@range");
                    string durationVal = app.ValueOf(source, "@duration");
                    string maintenanceVal = app.ValueOf(source, "@maintenance");
                    string trappingsVal = app.ValueOf(source, "@trappings");
                    string description = app.ValueOf(source, "description");

                    string levelVal = app.ValueOf(source, "@level");
                    string basepointsVal = app.ValueOf(source, "@basepoints");
                    string totalpointsVal = app.ValueOf(source, "@totalpoints");
                    string damageVal = app.ValueOf(source, "@damage");



                    XmlNode bonusList = output.CreateElement("bonuslist");
                    converted.AppendChild(bonusList);
                    XmlNode damagebonus = CharConverter.CreateElement(output, "damagebonus", "number", "0");
                    converted.AppendChild(damagebonus);
                    XmlNode damagedice = CharConverter.CreateElement(output, "damagedice", "dice", "");
                    converted.AppendChild(damagedice);
                    XmlNode descriptionNode = CharConverter.CreateElement(output, "description", "formattedtext");
                    descriptionNode.AppendChild(output.CreateElement("p"));
                    descriptionNode.LastChild.InnerText = description;
                    converted.AppendChild(descriptionNode);
                    XmlNode duration = CharConverter.CreateElement(output, "duration", "string", durationVal);
                    converted.AppendChild(duration);
                    XmlNode favored = CharConverter.CreateElement(output, "favored", "number", "0");
                    converted.AppendChild(favored);
                    XmlNode fumble = CharConverter.CreateElement(output, "fumble", "number", "0");
                    converted.AppendChild(fumble);
                    XmlNode nodeLinkField = CharConverter.CreateElement(output, "link", "windowreference");
                    converted.AppendChild(nodeLinkField);

                    XmlNode nodeLinkFieldClass = output.CreateElement("class");
                    nodeLinkFieldClass.InnerText = "powerdesc";
                    nodeLinkField.AppendChild(nodeLinkFieldClass);

                    XmlNode nodeLinkFieldRecord = output.CreateElement("recordname");
                    nodeLinkFieldRecord.InnerText = getPowerReference(nameVal);
                    nodeLinkField.AppendChild(nodeLinkFieldRecord);
                    XmlNode name = CharConverter.CreateElement(output, "name", "string", nameVal);
                    converted.AppendChild(name);

                    XmlNode powerNode = output.CreateElement("power");
                    powerNode.AppendChild(CharConverter.CreateElement(output, "current", "number", basepointsVal));
                    powerNode.AppendChild(CharConverter.CreateElement(output, "max", "number", totalpointsVal));
                    converted.AppendChild(powerNode);
                    XmlNode powerpoints = CharConverter.CreateElement(output, "powerpoints", "string", totalpointsVal);
                    converted.AppendChild(powerpoints);
                    XmlNode range = CharConverter.CreateElement(output, "range", "string", rangeVal);
                    converted.AppendChild(range);
                    XmlNode rank = CharConverter.CreateElement(output, "rank", "string", "");
                    converted.AppendChild(rank);
                    XmlNode shortdescription = CharConverter.CreateElement(output, "shortdescription", "string", description);
                    converted.AppendChild(shortdescription);
                    XmlNode trappings = CharConverter.CreateElement(output, "trappings", "string", trappingsVal);
                    converted.AppendChild(trappings);
                    XmlNode type = CharConverter.CreateElement(output, "type", "number", "3");
                    converted.AppendChild(type);

                }
            }
        }

        static public void processArcaneType(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;

            XmlNode node = targetNode;
            node.InnerText = storedArcaneType;
        }
        static public void processEdgeList(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;

            XmlNode node = targetNode;
            XmlNodeList nodeList = null;
            nodeList = inChar.SelectNodes("edges/edge");

            foreach (XmlNode source in nodeList)
            {
                // Create Edge node
                XmlNode converted = output.CreateElement(CharConverter.CreateId(node));
                node.AppendChild(converted);

                XmlNode nodeLinkField = CharConverter.CreateElement(output, "link", "windowreference");
                converted.AppendChild(nodeLinkField);

                XmlNode nodeLinkFieldClass = output.CreateElement("class");
                nodeLinkFieldClass.InnerText = "sw_referencefeat";
                nodeLinkField.AppendChild(nodeLinkFieldClass);

                // Name                
                String name = app.ValueOf(source, "@name");

                XmlNode nodeLinkFieldRecord = output.CreateElement("recordname");
                nodeLinkFieldRecord.InnerText = getEdgeReference(name);
                nodeLinkField.AppendChild(nodeLinkFieldRecord);

                converted.AppendChild(CharConverter.CreateElement(output, "name", "string", name));
            }

        }
        static public void processHindraceList(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;

            XmlNode node = targetNode;
            XmlNodeList nodeList = null;
            nodeList = inChar.SelectNodes("hindrances/hindrance");

            foreach (XmlNode source in nodeList)
            {
                // Create Edge node
                XmlNode converted = output.CreateElement(CharConverter.CreateId(node));
                node.AppendChild(converted);

                XmlNode nodeLinkField = CharConverter.CreateElement(output, "link", "windowreference");
                converted.AppendChild(nodeLinkField);

                XmlNode nodeLinkFieldClass = output.CreateElement("class");
                nodeLinkFieldClass.InnerText = "sw_referencefeat";
                nodeLinkField.AppendChild(nodeLinkFieldClass);

                // Name                
                String name = app.ValueOf(source, "@name");

                XmlNode nodeLinkFieldRecord = output.CreateElement("recordname");
                nodeLinkFieldRecord.InnerText = getHindranceReference(name);
                nodeLinkField.AppendChild(nodeLinkFieldRecord);

                converted.AppendChild(CharConverter.CreateElement(output, "name", "string", name));
            }
        }
        static public void processInventoryList(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;

            XmlNode node = targetNode;
            XmlNodeList nodeList = null;
            nodeList = inChar.SelectNodes("gear/item");

            foreach (XmlNode source in nodeList)
            {
                XmlNode converted = output.CreateElement(CharConverter.CreateId(node));
                node.AppendChild(converted);

                string nameVal = app.ValueOf(source, "@name");
                string quantity = app.ValueOf(source, "@quantity");
                XmlNode weightNode = source.SelectSingleNode("weight");
                string weightVal = app.ValueOf(weightNode, "@value");

                XmlNode costNode = source.SelectSingleNode("cost");
                string costVal = app.ValueOf(costNode, "@value");

                string description = app.ValueOf(source, "description");

                XmlNode arweight = CharConverter.CreateElement(output, "arweight", "number", weightVal);
                converted.AppendChild(arweight);

                XmlNode carried = CharConverter.CreateElement(output, "carried", "number", "1");
                converted.AppendChild(carried);
                XmlNode count = CharConverter.CreateElement(output, "count", "number", quantity);
                converted.AppendChild(count);
                XmlNode invweight = CharConverter.CreateElement(output, "invweight", "number", weightVal);
                converted.AppendChild(invweight);
                XmlNode name = CharConverter.CreateElement(output, "name", "string", nameVal);
                converted.AppendChild(name);
                XmlNode notes = CharConverter.CreateElement(output, "notes", "string", "");
                converted.AppendChild(notes);
                 XmlNode weight = CharConverter.CreateElement(output, "weight", "number", weightVal);
                converted.AppendChild(weight);
                XmlNode cost = CharConverter.CreateElement(output, "cost", "string", costVal);
                converted.AppendChild(cost);
                XmlNode descriptionNode = CharConverter.CreateElement(output, "text", "formattedtext");
                descriptionNode.AppendChild(output.CreateElement("p"));
                descriptionNode.LastChild.InnerText = description;
                converted.AppendChild(descriptionNode);

            }
        }
        static public void processSkillList(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;

            XmlNode node = targetNode;
            XmlNodeList nodeList = null;
            nodeList = inChar.SelectNodes("skills/skill");


            foreach (XmlNode source in nodeList)
            {
                XmlNode converted = output.CreateElement(CharConverter.CreateId(node));
                node.AppendChild(converted);

                string nameVal = app.ValueOf(source, "@name");
                string valueVal = app.ValueOf(source, "@value");
                string skillVal = "0";
                string skillMod = "0";
                int ind = -1;
                if (((ind =valueVal.IndexOf('+'))>0) || ((ind =valueVal.IndexOf('+'))>0)) {
                	skillMod = valueVal.Substring(ind);
                	skillVal = valueVal.Substring(0, ind);
                } else {
                    skillVal = valueVal;
                	skillMod = "0";
                }

                string description = app.ValueOf(source, "description");
                XmlNode bonusList = output.CreateElement("bonuslist");
                converted.AppendChild(bonusList);
                XmlNode favored = CharConverter.CreateElement(output, "favored", "number", "0");
                converted.AppendChild(favored);
                XmlNode nodeLinkField = CharConverter.CreateElement(output, "link", "windowreference");
                converted.AppendChild(nodeLinkField);

                XmlNode nodeLinkFieldClass = output.CreateElement("class");
                nodeLinkFieldClass.InnerText = "sw_referenceskill";
                nodeLinkField.AppendChild(nodeLinkFieldClass);

                XmlNode nodeLinkFieldRecord = output.CreateElement("recordname");
                nodeLinkFieldRecord.InnerText = getSkillReference(nameVal);
                nodeLinkField.AppendChild(nodeLinkFieldRecord);

                converted.AppendChild(CharConverter.CreateElement(output, "name", "string", nameVal));
                converted.AppendChild(CharConverter.CreateElement(output, "skill", "dice", skillVal));
                converted.AppendChild(CharConverter.CreateElement(output, "skillmod", "number", skillMod));

            }
        }

        static public void processWeaponList(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {

            XmlDocument output = outChar.OwnerDocument;

            XmlNode node = targetNode;
            XmlNodeList nodeList = null;
            string strengthDie = "d4";
            string strengthMod = "0";

            // get strength trait.
            nodeList = inChar.SelectNodes("attributes/attribute");
            foreach (XmlNode source in nodeList)
            {
                string nameVal = app.ValueOf(source, "@name");
                string valueVal = app.ValueOf(source, "@value");
                if (nameVal.Equals("Strength"))
                {
                    strengthDie = getDamageDice(valueVal, "", "0");
                    strengthMod = getDamageBonus(valueVal, "", "0");
                }
            }
            

            nodeList = inChar.SelectNodes("attacks/attack");

            foreach (XmlNode source in nodeList)
            {
                XmlNode converted = output.CreateElement(CharConverter.CreateId(node));
                node.AppendChild(converted);

                string nameVal = app.ValueOf(source, "@name");
                string attackVal = app.ValueOf(source, "@attack");
                string damageVal = app.ValueOf(source, "@damage");
                string equippedVal = app.ValueOf(source, "@equipped");
                string rangeVal = app.ValueOf(source, "@range");
                string quantityVal = app.ValueOf(source, "@quantity");
                XmlNode weightNode = source.SelectSingleNode("weight");
                string weightVal = app.ValueOf(weightNode, "@value");

                XmlNode costNode = source.SelectSingleNode("cost");
                string costVal = app.ValueOf(costNode, "@value");

                string description = app.ValueOf(source, "description");

                XmlNode ammoNode = output.CreateElement("ammo");
                converted.AppendChild(ammoNode);
                XmlNode ammoMaxNode = CharConverter.CreateElement(output, "max", "number", "0");
                ammoNode.AppendChild(ammoMaxNode);
                XmlNode ammoUsedNode = CharConverter.CreateElement(output, "used", "number", "0");
                ammoNode.AppendChild(ammoUsedNode);
                XmlNode armorpiercing = CharConverter.CreateElement(output, "armorpiercing", "number", "0");
                converted.AppendChild(armorpiercing);
                XmlNode bonusList = output.CreateElement("bonuslist");
                converted.AppendChild(bonusList);
                string carriedVal = "0";
                if (equippedVal.Equals("yes"))
                {
                    carriedVal = "1";
                }
                XmlNode carried = CharConverter.CreateElement(output, "carried", "number", carriedVal);
                converted.AppendChild(carried);

                string catname = "";
                if (rangeVal != null && !rangeVal.Equals("")) {
                    catname = "Ranged Weapons";
                } else {
                    catname =  "Melee Weapons";
                }

                XmlNode catnameNode = CharConverter.CreateElement(output, "catname", "string", catname);
                converted.AppendChild(catnameNode);

                XmlNode cost = CharConverter.CreateElement(output, "cost", "string", costVal);
                converted.AppendChild(cost);
                XmlNode count = CharConverter.CreateElement(output, "count", "number", quantityVal);
                converted.AppendChild(count);
                XmlNode damageNode = CharConverter.CreateElement(output, "damage", "string", damageVal);
                converted.AppendChild(damageNode);

                string damagebonusVal = getDamageBonus(damageVal, strengthDie, strengthMod);
                string damagediceVal = getDamageDice(damageVal, strengthDie, strengthMod);
                XmlNode damagebonusNode = CharConverter.CreateElement(output, "damagebonus", "number", damagebonusVal);
                converted.AppendChild(damagebonusNode);
                XmlNode damagediceNode = CharConverter.CreateElement(output, "damagedice", "dice", damagediceVal);
                converted.AppendChild(damagediceNode);

                XmlNode favored = CharConverter.CreateElement(output, "favored", "number", "0");
                converted.AppendChild(favored);
                XmlNode fumble = CharConverter.CreateElement(output, "fumble", "number", "0");
                converted.AppendChild(fumble);
                XmlNode group = CharConverter.CreateElement(output, "group", "string", catname);
                converted.AppendChild(group);
                XmlNode isequipment = CharConverter.CreateElement(output, "isequipment", "number", "1");
                converted.AppendChild(isequipment);
                XmlNode isidentified = CharConverter.CreateElement(output, "isidentified", "number", "1");
                converted.AppendChild(isidentified);
                XmlNode nameNode = CharConverter.CreateElement(output, "name", "string", nameVal);
                converted.AppendChild(nameNode);
                XmlNode notes = CharConverter.CreateElement(output, "notes", "string", description);
                converted.AppendChild(notes);
                XmlNode range = CharConverter.CreateElement(output, "range", "string", rangeVal);
                converted.AppendChild(range);
                XmlNode typeVal = CharConverter.CreateElement(output, "type", "number", "0");
                converted.AppendChild(typeVal);
                XmlNode weaponnotes = CharConverter.CreateElement(output, "weaponnotes", "string", description);
                converted.AppendChild(weaponnotes);
                XmlNode weight = CharConverter.CreateElement(output, "weight", "number", weightVal);
                converted.AppendChild(weight);
                XmlNode wpweight = CharConverter.CreateElement(output, "wpweight", "number", weightVal);
                converted.AppendChild(wpweight);

            }
        }
        /**
        static public void processArmorList(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
        }
        **/


        private static string lastDamageMod = "-1";
        private static string lastDamageDice = "-1";

        static public string getDamageDice(string gearDamage, string strDice, string strMod)
        {
            if (!lastDamageDice.Equals("-1"))
            {
                string tmp = lastDamageDice;
                lastDamageDice = "-1";
                lastDamageMod = "-1";
                return tmp;
            }
            else
            {
                parseDamage(gearDamage, strDice, strMod);
                return lastDamageDice;
            }
        }

        static public string getDamageBonus(string gearDamage, string strDice, string strMod)
        {
            if (!lastDamageMod.Equals("-1"))
            {
                string tmp = lastDamageMod;
                lastDamageDice = "-1";
                lastDamageMod = "-1";
                return tmp;
            }
            else
            {
                getDamageDice(gearDamage, strDice, strMod);
                return lastDamageMod;
            }
        }

        static private void parseDamage(string gearDamage, string strDice, string strMod)
        {
            string tmpDamage = gearDamage;
            string gearDamageBonus = "0";
            string gearDice = "";
            string gearDicePart = "";


            if (gearDamage.StartsWith("Str"))
            { //Melee or Thrown
                gearDice = strDice;
                tmpDamage = tmpDamage.Substring(3);
                if (tmpDamage.Length > 0)
                {
                    if (tmpDamage.StartsWith("+"))
                    {
                        tmpDamage = tmpDamage.Substring(1);
                        if (tmpDamage.StartsWith("d"))
                        {
                            gearDice = gearDice + "," + tmpDamage;
                        }
                        else
                        {
                            int tmpBonus = (Convert.ToInt32(tmpDamage) + Convert.ToInt32(strMod));
                            gearDamageBonus = tmpBonus.ToString();
                        }
                    }
                    else
                    {
                        //??
                    }
                }
            }
            else if (gearDamage.StartsWith("1-3d6"))
            { //Shotgun (special)
                gearDice = "d6,d6,d6";
                tmpDamage = tmpDamage.Substring(5);
            }
            else
            {

                int ind = -1;
                if (((ind = gearDamage.IndexOf('+')) > 0) || ((ind = gearDamage.IndexOf('-')) > 0))
                {
                    gearDamageBonus = gearDamage.Substring(ind);
                    gearDicePart = gearDamage.Substring(0, ind);
                }
                else
                {
                    gearDamageBonus = "0";
                    gearDicePart = gearDamage;
                }
                if ((ind = gearDicePart.IndexOf('d')) > 0)
                {
                    String numDice = gearDicePart.Substring(0, ind);
                    String dType = gearDicePart.Substring(ind + 1);
                    int numDiceInt = 0;
                    try
                    {
                        gearDice = "d" + dType;

                        if (numDice == null || (numDiceInt = Convert.ToInt32(numDice)) == 0)
                        {

                        }
                        else if (numDiceInt > 1)
                        {
                            for (int j = 1; j < numDiceInt; j++)
                            {
                                gearDice = gearDice + ",d" + dType;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        gearDice = "";
                    }
                }
                else
                {
                    gearDice = gearDicePart;
                }
            }

            lastDamageDice = gearDice;
            lastDamageMod = gearDamageBonus;
            return ;
        }

        static public void processSpellset(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;
            foreach (XmlNode classSource in inChar.SelectNodes("classes/class"))
            {
                String className = app.ValueOf(classSource, "@name");
                String castType = app.ValueOf(classSource, "@spells");

                //Spell-Like Abilities
                //at-will = level0
                //other = level1 and set avaiable to (NUM/day)

                String value;
                XmlNode node = targetNode;
                XmlNode converted = output.CreateElement(CharConverter.CreateId(node));

                //Load the template from assembly
                var spellset = new XmlDocument();
                try
                {
                    spellset.LoadXml(Properties.Resources.SpellsetPF);
                }
                catch (Exception)
                {
                    //Theortically the code should never reach this point.
                    app.AppendConsole("Spell failure because corrupted assembly. Please redownload program.");
                    return;
                }

                converted = CharConverter.CopyChildNodes(output, spellset.SelectSingleNode("spellset"), converted);

                String sCasterType = parseCastType(castType);
                if (!String.IsNullOrEmpty(sCasterType))
                    converted.AppendChild(CharConverter.CreateElement(output, "castertype", "string", sCasterType));

                app.Set(converted, "label", className);

                value = app.ValueOf(inChar, "classes/class[@name='" + className + "']/@casterlevel");
                app.Set(converted, "cl", value);

                //"arcanespellfailure"??
                /*
                value = ValueOf(inChar, "classes/class[@name='" + className + "']/arcanespellfailure/@value", false);
                if (String.IsNullOrEmpty(value))
                    value = "0";
                Set(converted, "sp", value);
                 * */

                value = parseClassName(className).ToLower();
                if (app.DisplayRuleset.Equals(RS_PF.Name))
                    app.Set(converted, "dc/ability", RS_PF.getSpellDCAbility(value));
                else if (app.DisplayRuleset.Equals(RS_35E.Name))
                    app.Set(converted, "dc/ability", RS_35E.getSpellDCAbility(value));

                var castInfo = inChar.SelectSingleNode("spellclasses/spellclass[@name='" + className + "']");
                if (castInfo != null)
                {
                    foreach (XmlNode source in castInfo.ChildNodes)
                    {
                        String casts = app.ValueOf(source, "@maxcasts", false);
                        if (String.IsNullOrEmpty(casts))
                            casts = "1";
                        app.Set(converted, "availablelevel" + app.ValueOf(source, "@level"), casts);
                    }
                }

                var nodeLists = new XmlNodeList[2];
                nodeLists[0] = inChar.SelectNodes("spellsknown/spell");
                nodeLists[1] = inChar.SelectNodes("spellbook/spell");
                //nodeLists[2] = inChar.SelectNodes("spellsmemorized/spell");
                foreach (XmlNodeList nodelist in nodeLists)
                {
                    foreach (XmlNode source in nodelist)
                    {
                        String sourceClass = app.ValueOf(source, "@class");
                        String level = app.ValueOf(source, "@level");
                        String name = app.ValueOf(source, "@name");
                        if (sourceClass.Equals(className) && !app.CheckForDuplicateSpell_35(converted, name, level))
                        {
                            XmlNode target = converted.SelectSingleNode("levels/level" + level + "/spells");
                            XmlNode spell = output.CreateElement(CharConverter.CreateId(target));

                            String casttime = "";
                            String componenttext = "";
                            String description = "";
                            XmlNode descriptionnode = null;
                            String shortdescription = "";
                            String duration = "";
                            String effect = "";
                            String range = "";
                            String save = "";
                            String school = "";
                            String resist = "";

                            casttime = app.ValueOf(source, "@casttime");
                            componenttext = app.ValueOf(source, "@componenttext");
                            description = app.ValueOf(source, "description");
                            shortdescription = description;
                            duration = app.ValueOf(source, "@duration");
                            var sb = new StringBuilder();
                            String[] effects = new String[3];
                            effects[0] = app.ValueOf(source, "@effect", false);
                            effects[1] = app.ValueOf(source, "@target", false);
                            effects[2] = app.ValueOf(source, "@area", false);
                            for (int i = 0; i < effects.Length; i++)
                            {
                                String effecttemp = effects[i];
                                if (!String.IsNullOrEmpty(effecttemp))
                                {
                                    if (!String.IsNullOrEmpty(sb.ToString()))
                                        sb.Append(";");
                                    sb.Append(effecttemp);
                                }
                            }
                            effect = sb.ToString();
                            range = app.ValueOf(source, "@range");
                            save = app.ValueOf(source, "@save");
                            sb = new StringBuilder();
                            sb.Append(app.ValueOf(source, "@schooltext"));
                            value = app.ValueOf(source, "@descriptortext", false);
                            if (String.IsNullOrEmpty(value))
                                value = app.ValueOf(source, "@subschooltext");
                            if (!String.IsNullOrEmpty(value) && !value.Equals("None"))
                            {
                                sb.Append(" [");
                                sb.Append(value);
                                sb.Append("]");
                            }
                            school = sb.ToString();
                            resist = app.ValueOf(source, "@resist");


                            spell.AppendChild(CharConverter.CreateElement(output, "level", "string", level));
                            spell.AppendChild(CharConverter.CreateElement(output, "cast", "number", "0"));
                            spell.AppendChild(CharConverter.CreateElement(output, "castingtime", "string", casttime));
                            spell.AppendChild(CharConverter.CreateElement(output, "components", "string", componenttext));
                            spell.AppendChild(descriptionnode != null
                                                  ? output.ImportNode(descriptionnode, true)
                                                  : CharConverter.CreateElement(output, "description", "string", description));
                            spell.AppendChild(CharConverter.CreateElement(output, "shortdescription", "string", shortdescription));
                            spell.AppendChild(CharConverter.CreateElement(output, "duration", "string", duration));
                            spell.AppendChild(CharConverter.CreateElement(output, "name", "string", name));
                            spell.AppendChild(CharConverter.CreateElement(output, "effect", "string", effect));
                            spell.AppendChild(CharConverter.CreateElement(output, "range", "string", range));
                            spell.AppendChild(CharConverter.CreateElement(output, "save", "string", save));
                            spell.AppendChild(CharConverter.CreateElement(output, "school", "string", school));
                            spell.AppendChild(CharConverter.CreateElement(output, "sr", "string", resist));
                            spell.AppendChild(CharConverter.CreateElement(output, "parse", "number", "1"));

                            target.AppendChild(spell);
                        }

                        //ignore dc? 10 + casters primary spellcasting stat + spell level
                    }
                }

                XmlNodeList nodesMemorized = inChar.SelectNodes("spellsmemorized/spell");
                foreach (XmlNode source in nodesMemorized)
                {
                    String sourceClass = app.ValueOf(source, "@class");
                    String level = app.ValueOf(source, "@level");
                    String name = app.ValueOf(source, "@name");
                    if (sourceClass.Equals(className) || sourceClass.Equals(""))
                    {
                        XmlNodeList spells = converted.SelectSingleNode("levels/level" + level + "/spells").ChildNodes;
                        foreach (XmlNode nodeExistingSpell in spells)
                        {
                            String sExistingName = app.ValueOf(nodeExistingSpell, "name");
                            if (sExistingName.Equals(name))
                            {
                                XmlNode nodePrepared = nodeExistingSpell.SelectSingleNode("prepared");
                                if (nodePrepared != null)
                                {
                                    int nPrepared = CharConverter.ConvertToInt(nodePrepared.InnerText);
                                    nPrepared = nPrepared + 1;
                                    nodePrepared.InnerText = nPrepared.ToString();
                                }
                                else
                                    nodeExistingSpell.AppendChild(CharConverter.CreateElement(output, "prepared", "number", "1"));
                            }
                        }
                    }
                }

                node.AppendChild(converted);
            }
        }

        static public void processLanguagelist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;
            XmlNode languagelist = targetNode;
            foreach (XmlNode language in inChar.SelectNodes("languages/language"))
            {
                XmlNode convertedLanguage = output.CreateElement(CharConverter.CreateId(languagelist));
                convertedLanguage.AppendChild(CharConverter.CreateElement(output, "name", "string", app.ValueOf(language, "@name")));
                languagelist.AppendChild(convertedLanguage);
            }
        }

        static public void processWeaponlist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;
            var types = new List<string>() { "0", "1" };
            foreach (String type in types)
            {
                //attackbonus and damagebonus independent of the abilityscore
                String value;
                XmlNode node = targetNode;
                XmlNodeList nodeList = null;
                if (type.Equals("0"))
                    nodeList = inChar.SelectNodes("melee/weapon");
                else if (type.Equals("1"))
                    nodeList = inChar.SelectNodes("ranged/weapon");

                foreach (XmlNode source in nodeList)
                {
                    // Create weapon node
                    XmlNode converted = output.CreateElement(CharConverter.CreateId(node));

                    // Name
                    String name = app.ValueOf(source, "@name");
                    String nameLower = name.ToLower();
                    converted.AppendChild(CharConverter.CreateElement(output, "name", "string", name));

                    // Type (0 = melee, 1 = ranged)
                    converted.AppendChild(CharConverter.CreateElement(output, "type", "number", type));

                    // Attacks
                    String[] attacks = app.ValueOf(source, "@attack").Split('/');
                    for (int i = 0; i < attacks.Length; i++)
                    {
                        if (attacks[i].StartsWith("+"))
                            attacks[i] = attacks[i].Substring(1);
                        converted.AppendChild(CharConverter.CreateElement(output,
                                                            "attack" + Convert.ToString(i + 1), "number", attacks[i]));
                    }
                    converted.AppendChild(CharConverter.CreateElement(output, "attacks", "number", Convert.ToString(attacks.Length)));

                    //critatkrange
                    String crit = app.ValueOf(source, "@crit");
                    if (crit.Contains("-"))
                        crit = crit.Split('-')[0];
                    else
                        crit = "20";
                    converted.AppendChild(CharConverter.CreateElement(output, "critatkrange", "number", crit));

                    // description
                    //String description = ValueOf(source, "description");
                    //converted.AppendChild(CreateElement(output, "description", "string", description));

                    // JPG 3.0.5 - New Stuff
                    // Handle range and special ranged weapon types
                    String sWeaponType = "";
                    if (type.Equals("1"))
                    {
                        value = app.ValueOf(source, "rangedattack/@rangeincvalue");
                        converted.AppendChild(CharConverter.CreateElement(output, "rangeincrement", "number", value));

                        if (name.StartsWith("Longbow") || name.StartsWith("Shortbow"))
                            sWeaponType = "bow";
                        else if (name.StartsWith("Sling"))
                            sWeaponType = "sling";
                    }
                    else
                        sWeaponType = "melee";

                    // Determine base damage type and magic bonus
                    List<String> aBaseDamageTypes = new List<String>();
                    String sDamageBonus = "";
                    XmlNodeList wepTypes = source.SelectNodes("weptype");
                    for (int i = 0; i < wepTypes.Count; i++)
                        aBaseDamageTypes.Add(wepTypes[i].InnerText.ToLower());

                    Regex rxMagic = new Regex(@"^\+(\d+)");
                    Match mMagic = rxMagic.Match(name);
                    if (mMagic.Success)
                    {
                        sDamageBonus = mMagic.Groups[1].Value;
                        if (Regex.IsMatch(sDamageBonus, @"^[1-5]$"))
                            aBaseDamageTypes.Add(@"magic");
                        else
                            aBaseDamageTypes.Add(@"epic");
                    }
                    if (Regex.IsMatch(nameLower, @"adamantine"))
                        aBaseDamageTypes.Add(@"adamantine");
                    if (Regex.IsMatch(nameLower, @"cold iron"))
                        aBaseDamageTypes.Add(@"cold iron");
                    if (Regex.IsMatch(nameLower, @"silver"))
                        aBaseDamageTypes.Add(@"silver");

                    String sBaseDamageType = String.Join(",", aBaseDamageTypes.ToArray());

                    // Determine base damage multiple
                    String sBaseDamageCrit = app.ValueOf(source, "@crit");
                    sBaseDamageCrit = sBaseDamageCrit.Contains("x") ? sBaseDamageCrit.Substring(sBaseDamageCrit.IndexOf("x") + 1) : "2";

                    // Build damage list
                    XmlNode nodeDamageList = output.CreateElement("damagelist");

                    // Determine damage clauses, and build damage list entries
                    String[] aDamages = app.ValueOf(source, "@damage").Split(new string[] { " plus " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < aDamages.Length; ++i)
                    {
                        List<String> aDamageDice = new List<String>();
                        List<String> aDamageType = new List<String>();

                        String[] aWords = aDamages[i].Split(null);
                        if (aWords.Length > 0)
                        {
                            if (Regex.IsMatch(aWords[0], @"^[\d+-d]+$"))
                            {
                                Regex rx = new Regex(@"(\d+)(d\d+)");
                                MatchCollection mc = rx.Matches(aWords[0]);
                                foreach (Match m in mc)
                                {
                                    int nDice = CharConverter.ConvertToInt(m.Groups[1].ToString());
                                    String sDie = m.Groups[2].ToString();
                                    for (int j = 0; j < nDice; ++j)
                                        aDamageDice.Add(sDie);
                                }
                            }

                            if (i == 0)
                                aDamageType.Add(sBaseDamageType);
                            if (aWords.Length > 1)
                                aDamageType.Add(String.Join(" ", aWords, 1, aWords.Length - 1));
                        }

                        if (aDamageDice.Count > 0)
                        {
                            XmlNode nodeDamageClause = output.CreateElement(CharConverter.CreateId(nodeDamageList));

                            nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "dice", "dice", String.Join(",", aDamageDice.ToArray())));
                            nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "type", "string", String.Join(",", aDamageType.ToArray())));

                            if (i > 0)
                                nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "critmult", "number", "2"));
                            else
                            {
                                nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "bonus", "number", sDamageBonus));
                                nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "critmult", "number", sBaseDamageCrit));

                                if (sWeaponType == "bow")
                                {
                                    Regex rx = new Regex(@"\(Str \+(\d+)\)");
                                    Match m = rx.Match(name);
                                    if (m.Success)
                                    {
                                        nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "stat", "string", "strength"));
                                        nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "statmax", "number", m.Groups[1].Value));
                                        nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "statmult", "number", "1"));
                                    }
                                }
                                else if (sWeaponType == "sling")
                                {
                                    nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "stat", "string", "strength"));
                                    nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "statmult", "number", "1"));
                                }
                                else if (sWeaponType == "melee")
                                {
                                    nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "stat", "string", "strength"));

                                    String sEquip = app.ValueOf(source, "@equipped", false);
                                    if (String.Equals(sEquip, "offhand"))
                                        nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "statmult", "number", "0.5"));
                                    else if (String.Equals(sEquip, "bothhands"))
                                        nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "statmult", "number", "1.5"));
                                    else
                                        nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "statmult", "number", "1"));
                                }
                            }

                            nodeDamageList.AppendChild(nodeDamageClause);
                        }
                    }

                    // Add the final damage list to the new weapon record
                    converted.AppendChild(nodeDamageList);

                    // JPG 3.0.5 - Old Stuff
                    //// Damage Bonus (Unable to calculate, since only total provided, and too many permutations)
                    //String damageBonus = "0";

                    //// Damage Dice
                    //String damagedice = "";
                    //value = app.ValueOf(source, "@damage");
                    //if (!String.IsNullOrEmpty(value))
                    //{
                    //    var sbDamageDice = new StringBuilder();
                    //    Boolean bAddComma = false;

                    //    Regex rx = new Regex(@"(\d+)(d\d+)");
                    //    MatchCollection mc = rx.Matches(value);
                    //    foreach(Match m in mc)
                    //    {
                    //        int nDice = CharConverter.ConvertToInt(m.Groups[1].ToString());
                    //        String sDie = m.Groups[2].ToString();

                    //        for (int j = 0; j < nDice; ++j)
                    //        {
                    //            if (bAddComma)
                    //                sbDamageDice.Append(",");
                    //            bAddComma = true;

                    //            sbDamageDice.Append(sDie);
                    //        }
                    //    }

                    //    damagedice = sbDamageDice.ToString();
                    //}
                    //converted.AppendChild(CharConverter.CreateElement(output, "damagedice", "dice", damagedice));

                    //critdmgmult
                    //String critdmgmult = app.ValueOf(source, "@crit");
                    //critdmgmult = critdmgmult.Contains("x") ? critdmgmult.Substring(critdmgmult.IndexOf("x") + 1) : "0";
                    //converted.AppendChild(CharConverter.CreateElement(output, "critdmgmult", "number", critdmgmult));

                    //damagetype
                    //var sb = new StringBuilder();
                    //XmlNodeList wepTypes = source.SelectNodes("weptype");
                    //for (int i = 0; i < wepTypes.Count; i++)
                    //{
                    //    XmlNode wepType = wepTypes[i];
                    //    sb.Append(wepType.InnerText.ToLower());
                    //    if (i + 1 < wepTypes.Count)
                    //        sb.Append(", ");
                    //}
                    //String damagetype = sb.ToString();
                    //converted.AppendChild(CharConverter.CreateElement(output, "damagetype", "string", damagetype));

                    //if (type.Equals("1"))
                    //{
                    //    value = app.ValueOf(source, "rangedattack/@rangeincvalue");
                    //    converted.AppendChild(CharConverter.CreateElement(output, "rangeincrement", "number", value));

                    //    String sRangedStatAdj = "";
                    //    if (name.StartsWith("Longbow") || name.StartsWith("Shortbow"))
                    //    {
                    //        sRangedStatAdj = "bow";

                    //        Regex rx = new Regex(@"\(Str \+(\d+)\)");
                    //        Match m = rx.Match(name);
                    //        if (m.Success)
                    //            converted.AppendChild(CharConverter.CreateElement(output, "damagemaxstat", "number", m.Groups[1].Value));
                    //    }
                    //    else if (name.StartsWith("Sling"))
                    //        sRangedStatAdj = "sling";
                    //    converted.AppendChild(CharConverter.CreateElement(output, "damagerangedstatadj", "string", sRangedStatAdj));
                    //}

                    node.AppendChild(converted);

                    if (source.SelectSingleNode("rangedattack") != null && type.Equals("0"))
                    {
                        converted = output.CreateElement(CharConverter.CreateId(node));

                        converted.AppendChild(CharConverter.CreateElement(output, "name", "string", name));
                        converted.AppendChild(CharConverter.CreateElement(output, "type", "number", "1"));

                        attacks = app.ValueOf(source, "rangedattack/@attack").Split('/');
                        for (int i = 0; i < attacks.Length; i++)
                        {
                            if (attacks[i].StartsWith("+"))
                                attacks[i] = attacks[i].Substring(1);
                            converted.AppendChild(CharConverter.CreateElement(output,
                                                                "attack" + Convert.ToString(i + 1), "number", attacks[i]));
                        }

                        converted.AppendChild(CharConverter.CreateElement(output, "critatkrange", "number", crit));
                        String rangeincrement = app.ValueOf(source, "rangedattack/@rangeincvalue");
                        converted.AppendChild(CharConverter.CreateElement(output, "rangeincrement", "number", rangeincrement));

                        // Build damage list
                        XmlNode nodeDamageList2 = output.CreateElement("damagelist");

                        // Determine damage clauses, and build damage list entries
                        String[] aDamages2 = app.ValueOf(source, "@damage").Split(new string[] { " plus " }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < aDamages2.Length; ++i)
                        {
                            List<String> aDamageDice = new List<String>();
                            List<String> aDamageType = new List<String>();

                            String[] aWords = aDamages2[i].Split(null);
                            if (aWords.Length > 0)
                            {
                                if (Regex.IsMatch(aWords[0], @"^[\d+-d]+$"))
                                {
                                    Regex rx = new Regex(@"(\d+)(d\d+)");
                                    MatchCollection mc = rx.Matches(aWords[0]);
                                    foreach (Match m in mc)
                                    {
                                        int nDice = CharConverter.ConvertToInt(m.Groups[1].ToString());
                                        String sDie = m.Groups[2].ToString();
                                        for (int j = 0; j < nDice; ++j)
                                            aDamageDice.Add(sDie);
                                    }
                                }

                                if (i == 0)
                                    aDamageType.Add(sBaseDamageType);
                                if (aWords.Length > 1)
                                    aDamageType.Add(String.Join(" ", aWords, 1, aWords.Length - 1));
                            }

                            if (aDamageDice.Count > 0)
                            {
                                XmlNode nodeDamageClause = output.CreateElement(CharConverter.CreateId(nodeDamageList2));

                                nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "dice", "dice", String.Join(",", aDamageDice.ToArray())));
                                nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "type", "string", String.Join(",", aDamageType.ToArray())));

                                if (i > 0)
                                    nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "critmult", "number", "2"));
                                else
                                {
                                    nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "bonus", "number", sDamageBonus));
                                    nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "critmult", "number", sBaseDamageCrit));

                                    // Assume "thrown" weapon type
                                    nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "stat", "string", "strength"));
                                    String sEquip = app.ValueOf(source, "@equipped", false);
                                    if (String.Equals(sEquip, "offhand"))
                                        nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "statmult", "number", "0.5"));
                                    else
                                        nodeDamageClause.AppendChild(CharConverter.CreateElement(output, "statmult", "number", "1"));
                                }

                                nodeDamageList2.AppendChild(nodeDamageClause);
                            }
                        }

                        // Add the final damage list to the new weapon record
                        converted.AppendChild(nodeDamageList2);

                        node.AppendChild(converted);
                    }
                }
            }
        }

        static public void processSpeedSpecial(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            targetNode.InnerText = app.ParseSpecial(inChar.SelectNodes("movement/special"), null);
        }

        static public void processSenses(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            targetNode.InnerText = app.ParseSpecial(inChar.SelectSingleNode("senses"), null);
        }

        static public void processDefensesDamagereduction(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            var sb = new StringBuilder();
            sb.Append(app.ParseSpecial(inChar.SelectSingleNode("resistances"), "Resist"));
            sb.Append(app.ParseSpecial(inChar.SelectSingleNode("immunities"), "Immune "));
            targetNode.InnerText = sb.ToString();
        }

        static public void processDefensesSr(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;
            foreach (XmlNode source in inChar.SelectNodes("resistances/special"))
            {
                if (app.ValueOf(source, "@name").Contains("Spell Resistance"))
                {
                    String sr = app.ValueOf(source, "@shortname").Split(' ')[1];
                    targetNode.AppendChild(CharConverter.CreateElement(output, "base", "number", sr));
                }
            }
        }

        static public void processSkilllist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlNode node = targetNode;
            String value;

            int nArmorCheckPenalty = CharConverter.ConvertToInt(app.ValueOf(inChar, "penalties/penalty[@name='Armor Check Penalty']/@value"));

            XmlDocument output = outChar.OwnerDocument;
            foreach (XmlNode skill in inChar.SelectNodes("skills/skill"))
            {
                XmlNode convertedSkill = output.CreateElement(CharConverter.CreateId(node));
                Double fSkillBonus = 0;

                value = app.ValueOf(skill, "@armorcheck", false);
                if (value.Equals("yes"))
                {
                    value = "1";
                    fSkillBonus += nArmorCheckPenalty;
                }
                else
                    value = "0";
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "armorcheckmultiplier", "number", value));

                //sublabel
                value = app.ValueOf(skill, "@name");
                if (app.DisplayRuleset.Equals(RS_35E.Name))
                {
                    try
                    {
                        if (value.Contains(":"))
                        {
                            convertedSkill.AppendChild(CharConverter.CreateElement(output, "sublabel", "string", value.Split(':')[1].Substring(1)));
                            value = value.Split(':')[0];
                        }
                    }
                    catch (Exception)
                    { }
                }
                else if (app.DisplayRuleset.Equals(RS_PF.Name))
                {
                    try
                    {
                        int nStartParen = value.IndexOf('(');
                        if (nStartParen >= 0)
                        {
                            String sublabel;
                            int nEndParen = value.IndexOf(')', nStartParen);
                            if (nEndParen >= 0)
                                sublabel = value.Substring(nStartParen + 1, nEndParen - nStartParen - 1).Trim();
                            else
                                sublabel = value.Substring(nStartParen + 1).Trim();

                            convertedSkill.AppendChild(CharConverter.CreateElement(output, "sublabel", "string", sublabel));

                            value = value.Substring(0, nStartParen).Trim();
                        }
                    }
                    catch (Exception)
                    { }
                }
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "label", "string", value));

                //state 1==yes and 0==no
                Boolean bClassSkill = false;
                value = app.ValueOf(skill, "@classskill", false);
                if (value.Equals("yes"))
                {
                    value = "1";
                    bClassSkill = true;
                }
                else
                    value = "0";
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "state", "number", value));

                value = app.ValueOf(skill, "@trainedonly", false);
                if (value.Equals("yes"))
                    value = "1";
                else
                    value = "0";
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "trainedonly", "number", value));

                value = app.ValueOf(skill, "@ranks", false);
                Double fRanks = 0;
                try { fRanks = Convert.ToDouble(value); }
                catch (Exception) { };
                if (fRanks > 0)
                {
                    fSkillBonus += fRanks;
                    if (bClassSkill && app.DisplayRuleset.Equals(RS_PF.Name))
                        fSkillBonus += 3;
                }
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "ranks", "number", value));

                convertedSkill.AppendChild(CharConverter.CreateElement(output, "showonminisheet", "number", "1"));

                value = app.ValueOf(skill, "@attrbonus", false);
                fSkillBonus += CharConverter.ConvertToInt(value);
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "stat", "number", value));

                value = app.ValueOf(skill, "@attrname", false).ToLower();
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "statname", "string", value));

                value = app.ValueOf(skill, "@value");
                Double fTotal = 0;
                try { fTotal = Convert.ToDouble(value); }
                catch (Exception) { };
                Double fMisc = fTotal - fSkillBonus;
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "misc", "number", fMisc.ToString()));
                //convertedSkill.AppendChild(CharConverter.CreateElement(output, "total", "number", value));

                value = app.ValueOf(skill, "description");
                convertedSkill.AppendChild(CharConverter.CreateElement(output, "description", "string", value));

                node.AppendChild(convertedSkill);
            }
        }

        static public void processClasses(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlNode classes = targetNode;
            String value;
            XmlDocument output = outChar.OwnerDocument;
            foreach (XmlNode charClass in inChar.SelectNodes("classes/class"))
            {
                XmlNode convertedClass = output.CreateElement(CharConverter.CreateId(classes));

                //classes/id-XXXXX/level
                value = app.ValueOf(charClass, "@name");
                convertedClass.AppendChild(CharConverter.CreateElement(output, "name", "string", parseClassName(value)));

                //Add archetype as specialability
                if (value.Contains("("))
                {
                    XmlNode specialabilitylist = outChar.SelectSingleNode("specialabilitylist");
                    XmlNode archetype = output.CreateElement(CharConverter.CreateId(specialabilitylist));

                    String archetypename = value.Split('(')[1].Split(')')[0];
                    archetype.AppendChild(CharConverter.CreateElement(output, "value", "string", "Archetype: " + archetypename));

                    archetype.AppendChild(CharConverter.CreateElement(output, "source", "string", parseClassName(value)));

                    value = "The selected archetype of the " + parseClassName(value) + " class is " + archetypename;
                    archetype.AppendChild(CharConverter.CreateElement(output, "description", "string", value));

                    specialabilitylist.AppendChild(archetype);
                }

                value = app.ValueOf(charClass, "@level");
                convertedClass.AppendChild(CharConverter.CreateElement(output, "level", "number", value));

                classes.AppendChild(convertedClass);
            }
        }

        static public void processSpecialabilitylist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value;
            XmlDocument output = outChar.OwnerDocument;
            List<XmlNodeList> nodeLists = new List<XmlNodeList>();

            nodeLists.Add(inChar.SelectNodes("otherspecials/special"));
            nodeLists.Add(inChar.SelectNodes("spelllike/special"));
            nodeLists.Add(inChar.SelectNodes("immunities/special"));
            nodeLists.Add(inChar.SelectNodes("resistances/special"));
            nodeLists.Add(inChar.SelectNodes("weaknesses/special"));
            nodeLists.Add(inChar.SelectSingleNode("traits").ChildNodes);
            nodeLists.Add(inChar.SelectSingleNode("flaws").ChildNodes);
            nodeLists.Add(inChar.SelectSingleNode("skilltricks").ChildNodes);
            nodeLists.Add(inChar.SelectSingleNode("skillabilities").ChildNodes);

            XmlNode node = targetNode;
            foreach (XmlNodeList nodelist in nodeLists)
            {
                foreach (XmlNode source in nodelist)
                {
                    value = app.ValueOf(source, "@name");
                    if (!app.CheckForDuplicateValue(node, value))
                    {
                        XmlNode converted = output.CreateElement(CharConverter.CreateId(node));

                        //value
                        converted.AppendChild(CharConverter.CreateElement(output, "value", "string", value));

                        //source
                        value = app.ValueOf(source, "@sourcetext", false);
                        if (String.IsNullOrEmpty(value))
                            value = app.ValueOf(source, "@categorytext", false);
                        converted.AppendChild(CharConverter.CreateElement(output, "source", "string", value));

                        //description
                        value = app.ValueOf(source, "description");
                        converted.AppendChild(CharConverter.CreateElement(output, "description", "string", value));

                        node.AppendChild(converted);
                    }
                }
            }
        }

        static public void processFeatlist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value;
            XmlDocument output = outChar.OwnerDocument;
            XmlNode target = targetNode;
            foreach (XmlNode source in inChar.SelectNodes("feats/feat"))
            {
                value = app.ValueOf(source, "@name");
                if (value.Contains("Armor Proficiency")
                || value.Contains("Shield Proficiency")
                || value.Contains("Weapon Proficiency"))
                    continue;

                XmlNode converted = output.CreateElement(CharConverter.CreateId(target));

                String name = "";
                String description = "";

                name = value;
                description = app.ValueOf(source, "description");

                converted.AppendChild(CharConverter.CreateElement(output, "value", "string", name));
                converted.AppendChild(CharConverter.CreateElement(output, "description", "string", description));
                target.AppendChild(converted);
            }
        }

        static public void processProficiencyweapon(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value;
            XmlDocument output = outChar.OwnerDocument;
            XmlNode target = targetNode;
            foreach (XmlNode source in inChar.SelectNodes("feats/feat"))
            {
                value = app.ValueOf(source, "@name");

                if (value.Contains("Weapon Proficiency"))
                {
                    var wepsb = new StringBuilder();
                    String[] splitValue = value.Split(' ');
                    for (int i = 0; i < splitValue.Length; i++)
                    {
                        String temp = splitValue[i];
                        if (temp.Equals("Weapon"))
                            break;
                        wepsb.Append(temp);
                        wepsb.Append(" ");
                    }
                    value = wepsb.ToString();
                }
                else
                    continue;

                XmlNode converted = output.CreateElement(CharConverter.CreateId(target));

                //value
                converted.AppendChild(CharConverter.CreateElement(output, "value", "string", value));

                //description
                value = app.ValueOf(source, "description");
                converted.AppendChild(CharConverter.CreateElement(output, "description", "string", value));

                target.AppendChild(converted);
            }
        }

        static public void processProficiencyarmor(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String value;
            XmlDocument output = outChar.OwnerDocument;
            XmlNode target = targetNode;
            foreach (XmlNode source in inChar.SelectNodes("feats/feat"))
            {
                value = app.ValueOf(source, "@name");

                if (value.Contains("Armor Proficiency"))
                {
                    try
                    {
                        value = value.Split('(')[1];
                        value = value.Split(')')[0];
                    }
                    catch (Exception e)
                    {
                        app.AppendConsole("Feat parse fail. Name:" + value + "; Exception:" + e.ToString());
                        continue;
                    }
                }
                else if (value.Contains("Shield Proficiency"))
                {
                    var shsb = new StringBuilder();
                    String[] splitValue = value.Split(' ');
                    for (int i = 0; i < splitValue.Length; i++)
                    {
                        String temp = splitValue[i];
                        if (temp.Equals("Proficiency"))
                            break;
                        shsb.Append(temp);
                        shsb.Append(" ");
                    }
                    value = shsb.ToString();
                }
                else
                    continue;

                XmlNode converted = output.CreateElement(CharConverter.CreateId(target));

                //value
                converted.AppendChild(CharConverter.CreateElement(output, "value", "string", value));

                //description
                value = app.ValueOf(source, "description");
                converted.AppendChild(CharConverter.CreateElement(output, "description", "string", value));

                target.AppendChild(converted);
            }
        }

        static public void processAttackbonusGrappleSize(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String sizeModifier = CharConverter.StripPlusSign(app.ValueOf(inChar, "armorclass/@fromsize"));
            targetNode.InnerText = Convert.ToString(CharConverter.ConvertToInt(sizeModifier) * -1);
        }

        static public void processAttackbonusGrappleTotal(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            targetNode.InnerText = CharConverter.StripPlusSign(app.ValueOf(inChar, "maneuvers/@cmb", false));
        }

        static public void processAttackbonusGrappleMisc(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String grappleAbilityModifier = CharConverter.StripPlusSign(app.ValueOf(inChar, "attributes/attribute[@name='Strength']/attrbonus/@modified"));
            String sizeModifier = CharConverter.StripPlusSign(app.ValueOf(inChar, "armorclass/@fromsize"));
            int baseInt = CharConverter.ConvertToInt(app.ValueOf(inChar, "attack/@baseattack"));
            String grappleSizeMod = Convert.ToString(CharConverter.ConvertToInt(sizeModifier) * -1);
            String grappleTotal = CharConverter.StripPlusSign(app.ValueOf(inChar, "maneuvers/@cmb", false));
            if (!String.IsNullOrEmpty(grappleTotal))
                targetNode.InnerText = Convert.ToString((CharConverter.ConvertToInt(grappleTotal)) -
                    (baseInt + CharConverter.ConvertToInt(grappleSizeMod) + CharConverter.ConvertToInt(grappleAbilityModifier)));
        }

        static public void processAttackbonusMeleeTotal(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            targetNode.InnerText = CharConverter.StripPlusSign(app.ValueOf(inChar, "attack/@meleeattack").Split('/')[0]);
        }

        static public void processAttackbonusMeleeMisc(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String meleeAbilityModifier = CharConverter.StripPlusSign(app.ValueOf(inChar, "attributes/attribute[@name='Strength']/attrbonus/@modified"));
            String sizeModifier = CharConverter.StripPlusSign(app.ValueOf(inChar, "armorclass/@fromsize"));
            int baseInt = CharConverter.ConvertToInt(app.ValueOf(inChar, "attack/@baseattack"));
            String meleeTotal = CharConverter.StripPlusSign(app.ValueOf(inChar, "attack/@meleeattack").Split('/')[0]);
            targetNode.InnerText = Convert.ToString((CharConverter.ConvertToInt(meleeTotal)) -
                (baseInt + CharConverter.ConvertToInt(sizeModifier) + CharConverter.ConvertToInt(meleeAbilityModifier)));
        }

        static public void processAttackbonusRangedTotal(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            targetNode.InnerText = CharConverter.StripPlusSign(app.ValueOf(inChar, "attack/@rangedattack").Split('/')[0]);
        }

        static public void processAttackbonusRangedMisc(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            String rangedAbilityModifier = CharConverter.StripPlusSign(app.ValueOf(inChar, "attributes/attribute[@name='Dexterity']/attrbonus/@modified"));
            String sizeModifier = CharConverter.StripPlusSign(app.ValueOf(inChar, "armorclass/@fromsize"));
            int baseInt = CharConverter.ConvertToInt(app.ValueOf(inChar, "attack/@baseattack"));
            String rangedTotal = CharConverter.StripPlusSign(app.ValueOf(inChar, "attack/@rangedattack").Split('/')[0]);
            targetNode.InnerText = Convert.ToString((CharConverter.ConvertToInt(rangedTotal)) -
                (baseInt + CharConverter.ConvertToInt(sizeModifier) + CharConverter.ConvertToInt(rangedAbilityModifier)));
        }

        static void AppendInventory(CharConverter app, XmlNode source, XmlDocument output, ref XmlNode targetNode, String type)
        {
            // Ignore armor items in inventory that are equipped (otherwise double entries and natural armor entry)
            if (type == "Armor")
            {
                XmlNode nodeCheckEquipped = source.Attributes.GetNamedItem("equipped");
                if (nodeCheckEquipped != null)
                    return;
            }

            String value;
            XmlNode node = targetNode;
            XmlNode converted = output.CreateElement(CharConverter.CreateId(node));

            converted.AppendChild(CharConverter.CreateElement(output, "carried", "number", "1"));

            converted.AppendChild(CharConverter.CreateElement(output, "showonminisheet", "number", "1"));

            value = app.ValueOf(source, "@quantity");
            converted.AppendChild(CharConverter.CreateElement(output, "count", "number", value));

            value = app.ValueOf(source, "@name");
            converted.AppendChild(CharConverter.CreateElement(output, "name", "string", value));

            value = app.ValueOf(source, "weight/@value");
            if (value.Equals("-1"))
                value = "";
            converted.AppendChild(CharConverter.CreateElement(output, "weight", "number", value));

            XmlNode description = CharConverter.CreateElement(output, "description", "formattedtext");
            description.AppendChild(output.CreateElement("p"));
            description.LastChild.InnerText = app.ValueOf(source, "description");
            converted.AppendChild(description);

            value = app.ValueOf(source, "cost/@text").ToLower();
            converted.AppendChild(CharConverter.CreateElement(output, "cost", "string", value));

            converted.AppendChild(CharConverter.CreateElement(output, "isidentified", "number", "1"));

            converted.AppendChild(CharConverter.CreateElement(output, "type", "string", type));

            value = app.ValueOf(source, "itemslot", false);
            converted.AppendChild(CharConverter.CreateElement(output, "location", "string", value));

            node.AppendChild(converted);
        }

        static public void processInventorylist(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            XmlDocument output = outChar.OwnerDocument;
            XmlNode node = targetNode;
            List<XmlNodeList> nodelists = new List<XmlNodeList>();
            nodelists.Add(inChar.SelectNodes("defenses/armor"));
            nodelists.Add(inChar.SelectNodes("magicitems/item"));
            nodelists.Add(inChar.SelectNodes("gear/item"));
            List<String> types = new List<string>();
            types.Add("Armor");
            types.Add("Magic Item");
            types.Add("");

            for (int i = 0; i < nodelists.Count; i++)
            {
                foreach (XmlNode source in nodelists[i])
                    AppendInventory(app, source, output, ref targetNode, types[i]);
            }
        }

        static public void processSavesFortitudeMisc(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            int misc = CharConverter.ConvertToInt(CharConverter.StripPlusSign(app.ValueOf(inChar, "saves/save[@name='Fortitude Save']/@frommisc")));
            misc += CharConverter.ConvertToInt(CharConverter.StripPlusSign(app.ValueOf(inChar, "saves/save[@name='Fortitude Save']/@fromresist")));
            targetNode.InnerText = Convert.ToString(misc);
        }

        static public void processSavesReflexMisc(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            int misc = CharConverter.ConvertToInt(CharConverter.StripPlusSign(app.ValueOf(inChar, "saves/save[@name='Reflex Save']/@frommisc")));
            misc += CharConverter.ConvertToInt(CharConverter.StripPlusSign(app.ValueOf(inChar, "saves/save[@name='Reflex Save']/@fromresist")));
            targetNode.InnerText = Convert.ToString(misc);
        }

        static public void processSavesWillMisc(CharConverter app, String call, ref XmlNode inChar, ref XmlNode outChar, ref XmlNode targetNode)
        {
            int misc = CharConverter.ConvertToInt(CharConverter.StripPlusSign(app.ValueOf(inChar, "saves/save[@name='Will Save']/@frommisc")));
            misc += CharConverter.ConvertToInt(CharConverter.StripPlusSign(app.ValueOf(inChar, "saves/save[@name='Will Save']/@fromresist")));
            targetNode.InnerText = Convert.ToString(misc);
        }
    }
}
