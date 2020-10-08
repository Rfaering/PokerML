using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace TestP.Ocr.Image
{
    public class ImageFinder
    {
        private Bitmap _hayStack;
        private int[][] _hackStacPixelArray;

        public ImageFinder(Bitmap haystack)
        {
            _hayStack = haystack;
            _hackStacPixelArray = GetPixelArray(haystack);
        }

        public ICollection<NamedRectangle> FindNamedBitMaps(ICollection<NamedBitMap> namedBitmaps, Rectangle inArea, float tolerance = 0.85f)
        {
            List<NamedRectangle> results = new List<NamedRectangle>();

            Dictionary<NamedBitMap, int[][]> pixelArrays = new Dictionary<NamedBitMap, int[][]>();

            foreach (var item in namedBitmaps)
            {
                pixelArrays[item] = GetPixelArray(item.Bitmap);
            }

            for (int j = inArea.Y; j < inArea.Y+ inArea.Height; j++)
            {
                for (int i = inArea.X; i < inArea.X + inArea.Width; i++)
                {
                    var testPoint = new Point() { X = i, Y = j };

                    foreach (var namedBitMap in namedBitmaps)
                    {
                        if(IsNeedlePresentAtLocation(_hackStacPixelArray, pixelArrays[namedBitMap], testPoint, namedBitMap.Bitmap.Width * namedBitMap.Bitmap.Height, tolerance))
                        {
                            results.Add(new NamedRectangle(namedBitMap, new Rectangle(testPoint, namedBitMap.Bitmap.Size)));
                        }
                    }
                }
            }

            return results.OrderBy(x => x.Rectangle.X).ToList();
        }

        public ICollection<Rectangle> Find(Bitmap needle, float tolerance)
        {
            List<Rectangle> results = new List<Rectangle>();

            if (null == needle)
            {
                return results;
            }
            if (_hayStack.Width < needle.Width || _hayStack.Height < needle.Height)
            {
                return results;
            }

            
            var needleArray = GetPixelArray(needle);

            var needlePixelCount = (needle.Width * needle.Height);

            for (int j = 0; j < _hayStack.Height-needle.Height; j++)
            {
                for (int i = 0; i < _hayStack.Width-needle.Width; i++)
                {
                    var testPoint = new Point() { X = i, Y = j };
                    var testRect = new Rectangle( testPoint, needle.Size);

                    if (results.Any(x => x.IntersectsWith(testRect)))
                    {
                        continue;
                    }
                    
                    if (IsNeedlePresentAtLocation(_hackStacPixelArray, needleArray, testPoint, needlePixelCount, tolerance))
                    {
                        results.Add(testRect);
                    }
                }
            }

            return results;
        }

        private int[][] GetPixelArray(Bitmap bitmap)
        {
            var result = new int[bitmap.Height][];
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            for (int y = 0; y < bitmap.Height; ++y)
            {
                result[y] = new int[bitmap.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }

            bitmap.UnlockBits(bitmapData);

            return result;
        }

        private int Distance(byte[] rbga1, byte[] rbga2)
        {
            return Math.Abs(rbga1[0]- rbga2[0]) + Math.Abs(rbga1[1] - rbga2[1]) + Math.Abs(rbga1[2] - rbga2[2]);
        }


        private bool IsNeedlePresentAtLocation(int[][] haystack, int[][] needle, Point point, int needleMaxPixels, float tolerance)
        {
            int pixelMatched = CountPixels(haystack, needle, point, 10) * 100;
            var match = (pixelMatched / (float)needleMaxPixels);

            if(match <= tolerance)
            {
                return false;
            }

            pixelMatched = CountPixels(haystack, needle, point, 1) * 1;
            match = (pixelMatched / (float)needleMaxPixels);

            return match >= tolerance;
        }

        private static int CountPixels(int[][] haystack, int[][] needle, Point point, int downSize, bool includeTransparency = false)
        {
            int pixelMatched = 0;

            for (int y = 0; y < needle.Length; y += downSize)
            {
                int[] hackStackLine = haystack[y + point.Y];
                int hackStackStart = point.X;
                int[] needleLine = needle[y];
                int needleStart = 0;

                for (int i = 0; i < needle[y].Length; i += downSize)
                {
                    var pixel1 = hackStackLine[i + hackStackStart];
                    var pixel2 = needleLine[i + needleStart];

                    // var bytes1 = BitConverter.GetBytes(hackStackLine[i + hackStackStart]);
                    // var distance = Distance(bytes1, bytes2);

                    if (includeTransparency)
                    {
                        var needleRBGA = BitConverter.GetBytes(needleLine[i + needleStart]);
                        if (needleRBGA[3] == 255 || pixel1 == pixel2)
                        {
                            pixelMatched++;
                        }
                    } else
                    {
                        if (pixel1 == pixel2)
                        {
                            pixelMatched++;
                        }
                    }
                }
            }

            return pixelMatched;
        }
    }
}
