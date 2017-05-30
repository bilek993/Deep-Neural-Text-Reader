using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Deep_Neural_Text_Reader
{
    class ImageCutter
    {
        public List<Bitmap> listOfLetters { get; set; }

        private Bitmap wordImage;
        private int minY = -1;
        private int maxY = -1;

        const float MAX_VALUE_PER_LINE = 100;

        public ImageCutter(Bitmap wordImage)
        {
            this.wordImage = wordImage;
        }

        private bool IsBlackPixel (Color pixel)
        {
            return 1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0 > 0.25;
        }

        public void FindMinMaxlValue()
        {
            for (int y = 0; y < wordImage.Height; y++)
            {
                int sum = 0;
                for (int x = 0; x < wordImage.Width; x++)
                {
                    if (IsBlackPixel(wordImage.GetPixel(x, y)))
                        sum++;
                }
                if (sum > wordImage.Width/MAX_VALUE_PER_LINE)
                {
                    minY = y;
                    break;
                }
            }

            for (int y = wordImage.Height - 1; y >= 0; y--)
            {
                int sum = 0;
                for (int x = 0; x < wordImage.Width; x++)
                {
                    if (IsBlackPixel(wordImage.GetPixel(x, y)))
                        sum++;
                }
                if (sum > wordImage.Width / MAX_VALUE_PER_LINE)
                {
                    maxY = y;
                    break;
                }
            }
        }

        public void CutWord()
        {
            if (minY == -1 || maxY == -1)
                return;

            listOfLetters = new List<Bitmap>();
            int blackStart = -1;

            for (int x = 0; x < wordImage.Width; x++)
            {
                int sum = 0;
                for (int y = 0; y < wordImage.Height; y++)
                {
                    if (IsBlackPixel(wordImage.GetPixel(x, y)))
                        sum++;
                }

                if (sum > wordImage.Height / MAX_VALUE_PER_LINE)
                {
                    if (blackStart == -1)
                        blackStart = x;
                }
                else
                {
                    if (blackStart != -1)
                    {
                        Bitmap tmpSymbol = wordImage.Clone(new Rectangle(blackStart, minY, x - blackStart, maxY - minY), wordImage.PixelFormat);
                        listOfLetters.Add(tmpSymbol);
                        blackStart = -1;
                    }
                }
            }
        }
    }
}
