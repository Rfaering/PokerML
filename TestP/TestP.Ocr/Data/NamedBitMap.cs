using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using TestP.Ocr.Data;

namespace TestP.Ocr.Image
{
    public class NamedBitMap
    {
        public string Name { get; }

        public string RelativePath { get; }

        public Bitmap Bitmap { get; }

        public NamedBitMap(string filepath)
        {
            RelativePath = Path.GetRelativePath(Images.ImagePath, filepath);            
            Name = Path.GetFileNameWithoutExtension(filepath);
            Bitmap = new Bitmap(filepath);
        }
    }
}
