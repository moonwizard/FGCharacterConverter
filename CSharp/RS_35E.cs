// Copyright (c) 2012, SmiteWorks USA LLC

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace CharacterConverter
{
    class RS_35E
    {
        public const String Name = "3.5E";
        public const String InternalName = "3.5E";

        public const String LocalPath = @"\characters\3.5E.xml";
        public const String LocalBackupPath = @"\characters\3.5E.import.xml";

        public const String Release = @"11|CoreRPG:3";

        static Dictionary<String, String> listSpellDCAbility = new Dictionary<String, String>
        {
            {"wizard", "intelligence"},
            {"cleric", "wisdom"},
            {"druid", "wisdom"},
            {"ranger", "wisdom"},
            {"paladin", "wisdom"},
            {"bard", "charisma"},
            {"sorcerer", "charisma"}
        };

        static public String getSpellDCAbility(String key)
        {
            if (listSpellDCAbility.ContainsKey(key))
                return listSpellDCAbility[key];
            return "";
        }

        static XmlNode CheckForExistingSpell(CharConverter app, XmlNode oldNode, String name, int levelindex)
        {
            XmlNodeList nodelist = oldNode.SelectSingleNode("spellset").ChildNodes;
            foreach (XmlNode spellset in nodelist)
            {
                if (spellset.SelectSingleNode("levels") == null)
                    continue;

                XmlNode level = spellset.SelectSingleNode("levels/level" + Convert.ToString(levelindex));

                if (level == null)
                    continue;

                XmlNodeList spells = level.SelectSingleNode("spells").ChildNodes;
                foreach (XmlNode spell in spells)
                {
                    String value = app.ValueOf(spell, "name");
                    if (value.Equals(name))
                        return spell;
                }
            }
            return null;
        }

        static public void ReplaceCharacter(CharConverter app, XmlNode parentNode, XmlNode newNode, XmlNode oldNode)
        {
            //spellset
            XmlNodeList nodelist = newNode.SelectSingleNode("spellset").ChildNodes;
            foreach (XmlNode spellset in nodelist)
            {
                if (spellset.SelectSingleNode("levels") == null)
                    continue;

                XmlNodeList levels = spellset.SelectSingleNode("levels").ChildNodes;
                for (int levelindex = 0; levelindex < levels.Count; levelindex++)
                {
                    XmlNode level = levels[levelindex];
                    XmlNode spells = level.SelectSingleNode("spells");
                    XmlNodeList spellslist = spells.ChildNodes;
                    for (int i = spellslist.Count - 1; i >= 0; i--)
                    {
                        XmlNode spell = spellslist[i];
                        String value = app.ValueOf(spell, "name");
                        XmlNode existingspell = CheckForExistingSpell(app, oldNode, value, levelindex);

                        if (existingspell == null)
                            continue;

                        spells.RemoveChild(spell);
                        spells.AppendChild(CharConverter.ImportIdNode(newNode.OwnerDocument, spells, existingspell));
                    }
                }
            }

            //notes
            app.ReplaceElement(oldNode, newNode, "notes");

            //appearance
            app.ReplaceElement(oldNode, newNode, "appearance");
        }
    }
}
