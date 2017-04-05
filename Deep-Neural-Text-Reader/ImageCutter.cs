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
            if (1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0 > 0.25)
                return true;
            else
                return false;
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

            ScaleLetters();
        }

        private void ScaleLetters()
        {
            for (int i = 0; i < listOfLetters.Count; ++i)
            {
                Bitmap letter = listOfLetters[i];

                double w, h;
                if ((double)letter.Width / letter.Height < 9.0 / 16.0)
                {
                    double x = letter.Height / 16.0;
                    w = x * 9.0;
                    h = letter.Height;
                }
                else
                {
                    double x = letter.Width / 9.0;
                    h = x * 16.0;
                    w = letter.Width;
                }

                double startX = (w - letter.Width) / 2.0;
                double startY = (h - letter.Height) / 2.0;

                Bitmap newLetter = new Bitmap((int)w, (int)h);
                using (Graphics g = Graphics.FromImage(newLetter))
                {
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, (int)w, (int)h));
                }

                for (int x = 0; x < letter.Width; ++x)
                {
                    for (int y = 0; y < letter.Height; ++y)
                    {
                        newLetter.SetPixel((int)startX + x, (int)startY + y, letter.GetPixel(x, y));
                    }
                }

                listOfLetters[i] = new Bitmap(newLetter, 9, 16);
            }
        }
    }
}
