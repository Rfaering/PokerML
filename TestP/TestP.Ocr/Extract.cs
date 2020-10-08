using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using Tesseract;
using TestP.Ocr.Data;
using TestP.Ocr.Extracts;
using TestP.Ocr.Image;

namespace TestP.Ocr
{
    public class Extract
    {
        public static ExtractedBoard FindNameImage(float tolerance = 0.1f)
        {
            var mainImage = Images.Test3;
            var board = new ExtractedBoard(mainImage);

            var imageFinder = new ImageFinder(board.Image);
            var users = imageFinder.Find(Images.NamePlate, 0.6f);

            ExtractUsersBlinds(board, imageFinder, users);

            Color(mainImage, users, System.Drawing.Color.FromArgb(100, 255, 0, 0));

            var cards = imageFinder.Find(Images.Card, 0.60f);
            Color(mainImage, cards, System.Drawing.Color.FromArgb(100, 0, 255, 0));

            return board;
        }

        private static void ExtractUsersBlinds(ExtractedBoard board, ImageFinder imageFinder, ICollection<Rectangle> users)
        {
            foreach (var user in users)
            {
                var sBuilder = new StringBuilder();
                var results = imageFinder.FindNamedBitMaps(Images.BBNumbers, user);
                
                foreach (var item in results)
                {
                    if (item.Name == "dot")
                    {
                        sBuilder.Append(".");
                    }
                    else
                    {
                        sBuilder.Append(item.Name);
                    }
                }

                try
                {
                    var bb = double.Parse(sBuilder.ToString(), CultureInfo.InvariantCulture);
                    board.Players.Add(new ExtractedPlayer() { BB = bb });
                }
                catch
                {
                    board.Players.Add(new ExtractedPlayer());
                }
            }
        }

        private static void Color(Bitmap bitmap, ICollection<Rectangle> results, Color color)
        {
            foreach (var item in results)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (SolidBrush myBrush = new SolidBrush(color))
                    {
                        graphics.FillRectangle(myBrush, item);
                    }
                }
            }
        }
    }
}
