// Copyright (c) 2012, SmiteWorks USA LLC

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace CharacterConverter
{
    class RS_4E
    {
        public const String Name = "4E";
        public const String InternalName = "4E";

        public const String LocalPath = @"\characters\4E.xml";
        public const String LocalBackupPath = @"\characters\4E.import.xml";

        public const String Release = @"27|CoreRPG:3";

        static public XmlNode CheckForExistingPower(CharConverter app, XmlNode node, String name)
        {
            String value;
            XmlNode nodePowers = node.SelectSingleNode("powers");
            XmlNodeList powers = nodePowers.ChildNodes;
            for (int nPower = powers.Count - 1; nPower >= 0; nPower--)
            {
                XmlNode nodePower = powers[nPower];
                value = app.ValueOf(nodePower, "name");
                if (name.Equals(value))
                    return nodePower;
            }
            /* DEBUG
                        XmlNodeList nodelist = node.SelectNodes("powers/*");
                        for (int powercatindex = 0; powercatindex < nodelist.Count; powercatindex++)
                        {
                            XmlNode powercat = nodelist[powercatindex];
                            XmlNode powernode = powercat.SelectSingleNode("power");
                            XmlNodeList powers = powernode.ChildNodes;
                            for (int powerindex = powers.Count - 1; powerindex >= 0; powerindex--)
                            {
                                XmlNode power = powers[powerindex];
                                value = app.ValueOf(power, "name");
                                if (name.Equals(value))
                                    return power;
                            }
                        }
             */
            return null;
        }

        static public void ReplaceCharacter(CharConverter app, XmlNode parentNode, XmlNode newNode, XmlNode oldNode)
        {
            //powers
            /* DEBUG
            XmlNodeList nodelist = newNode.SelectSingleNode("powers").ChildNodes;
            for (int powercatindex = 0; powercatindex < nodelist.Count; powercatindex++)
            {
                XmlNode powercat = nodelist[powercatindex];
                XmlNode powernode = powercat.SelectSingleNode("power");
                XmlNodeList powers = powernode.ChildNodes;
                foreach (XmlNode power in powers)
                {
                    String value = app.ValueOf(power, "name");
                    XmlNode existingpower = CheckForExistingPower(app, oldNode, value);

                    if (existingpower == null)
                        continue;

                    powernode.RemoveChild(power);
                    powernode.AppendChild(CharConverter.ImportIdNode(newNode.OwnerDocument, powernode, existingpower));
                }
            }
            */
            XmlNode nodePowers = newNode.SelectSingleNode("powers");
            XmlNodeList nodePowerList = nodePowers.ChildNodes;
            foreach (XmlNode nodePower in nodePowerList)
            {
                String value = app.ValueOf(nodePower, "name");
                XmlNode existingpower = CheckForExistingPower(app, oldNode, value);

                if (existingpower == null)
                    continue;

                nodePowers.RemoveChild(nodePower);
                nodePowers.AppendChild(CharConverter.ImportIdNode(newNode.OwnerDocument, nodePower, existingpower));
            }

            //notes
            app.ReplaceElement(oldNode, newNode, "notes");

            //appearance
            app.ReplaceElement(oldNode, newNode, "appearance");
        }
    }
}
