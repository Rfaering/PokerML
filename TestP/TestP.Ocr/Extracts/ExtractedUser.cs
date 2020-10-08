using System;
using System.Collections.Generic;
using System.Text;

namespace TestP.Ocr.Extracts
{
    public class ExtractedPlayer
    {
        public double? BB { get; set; }
        public bool Player { get; set; }


        public override string ToString()
        {
            return $"Player has {BB}";
        }
    }
}
