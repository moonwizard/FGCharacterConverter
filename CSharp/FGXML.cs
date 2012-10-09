// Copyright (c) 2012, SmiteWorks USA LLC

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace CharacterConverter
{
    class FGXML
    {
        public const String Format = @"Fantasy Grounds (.xml)";

        public const String Filter = @"Fantasy Grounds (*.xml)|*.xml";

        public const String CharPath = @"root/character";
        public const String NamePath = @"name";

        static public void CheckFile(XmlDocument doc, StringBuilder errorMessage)
        {
            if (doc.SelectSingleNode("root") == null)
                errorMessage.Append("Invalid Fantasy Grounds file.");
        }
    }
}
