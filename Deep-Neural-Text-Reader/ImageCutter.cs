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
        private Bitmap wordImage;
        private int minY;
        private int maxY;

        const float MAX_VALUE_PER_LINE = 100;

        public ImageCutter(Bitmap wordImage)
        {
            this.wordImage = wordImage;
        }

        public void FindMinMaxlValue()
        {
            for (int y = 0; y < wordImage.Height; y++)
            {
                int sum = 0;
                for (int x = 0; x < wordImage.Width; x++)
                {
                    Color pixel = wordImage.GetPixel(x, y);
                    if (1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0 > 0.25)
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
                    Color pixel = wordImage.GetPixel(x, y);
                    if (1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0 > 0.25)
                        sum++;
                }
                if (sum > wordImage.Width / MAX_VALUE_PER_LINE)
                {
                    maxY = y;
                    break;
                }
            }
        }
    }
}
