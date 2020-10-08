using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TestP.Ocr.Image
{
    public class NamedRectangle
    {
        public NamedRectangle(NamedBitMap namedBitMap, Rectangle rectangle)
        {
            NamedBitMap = namedBitMap;
            Rectangle = rectangle;
        }

        public NamedBitMap NamedBitMap { get; }

        public string Name => NamedBitMap.Name;
        public Rectangle Rectangle { get; }
    }
}
