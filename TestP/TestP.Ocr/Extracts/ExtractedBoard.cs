using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TestP.Ocr.Extracts
{
    public class ExtractedBoard
    {
        public ExtractedBoard(Bitmap image)
        {
            Image = image;
        }

        public Bitmap Image { get; }

        public List<ExtractedPlayer> Players { get; } = new List<ExtractedPlayer>();
        public List<ExtractedCard> Cards { get; } = new List<ExtractedCard>();

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("---Players---");

            foreach (var item in Players)
            {
                sb.AppendLine(item.ToString());
            }

            return sb.ToString();
        }
    }
}
