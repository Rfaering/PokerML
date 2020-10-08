using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using TestP.Ocr.Image;

namespace TestP.Ocr.Data
{
    public static class Images
    {
        public static List<NamedBitMap> Files = new List<NamedBitMap>();
        public static string ImagePath = @"C:\Users\rfaer\source\repos\TestP\TestP.Ocr\Data\Images\";

        static Images()
        {
            var files = Directory.GetFiles(ImagePath, "", SearchOption.AllDirectories);

            foreach (var item in files)
            {
                Files.Add(new NamedBitMap(item));
            }
        }

        public static Bitmap Test1 => Files.FirstOrDefault(x=>x.Name == "Test001").Bitmap;
        public static Bitmap Test2 => Files.FirstOrDefault(x => x.Name == "Test002").Bitmap;
        public static Bitmap Test3 => Files.FirstOrDefault(x => x.Name == "Test003").Bitmap;
        public static Bitmap NamePlate => Files.FirstOrDefault(x => x.Name == "NamePlate").Bitmap;
        public static Bitmap Card => Files.FirstOrDefault(x => x.Name == "Card").Bitmap;

        public static ICollection<NamedBitMap> BBNumbers => Files.Where(x => x.RelativePath.StartsWith("BB")).ToList();
    }
}
