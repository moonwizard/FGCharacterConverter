// Copyright (c) 2012, SmiteWorks USA LLC

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CharacterConverter
{
    public class Utility
    {
        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static void ScaleListViewColumns(ListView listview, SizeF factor)
        {
            foreach (ColumnHeader column in listview.Columns)
            {
                column.Width = (int)Math.Round(column.Width * factor.Width);
            }
        }

        public static void CreateFileFromBytes(byte[] bytes, String path, bool overwrite)
        {
            if (!overwrite && File.Exists(path))
                return;

            if (overwrite)
                File.Delete(path);

            Stream fileStream = new MemoryStream(bytes);
            Stream destination = File.OpenWrite(path);
            Utility.CopyStream(fileStream, destination);
            destination.Close();
            fileStream.Close();
        }
    }
}
