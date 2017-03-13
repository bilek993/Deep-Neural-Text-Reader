using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Deep_Neural_Text_Reader
{
    class LearnDataLoader
    {
        private double[][] inputs;
        private int[][] outputs;

        public double[][] Inputs
        {
            get
            {
                return inputs;
            }
        }

        public int[][] Outputs
        {
            get
            {
                return outputs;
            }
        }

        public async void LoadLearnData(string fileName)
        {
            if (File.Exists(fileName))
            {
                string directoryName = fileName.Substring(0, fileName.LastIndexOf('\\'));

                List<string> lines = new List<string>();
                int linesCount = 0;
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        ++linesCount;
                        lines.Add(line);
                    }
                }

                inputs = new double[linesCount][];
                outputs = new int[linesCount][];

                int i = 0;
                foreach (string line in lines)
                {
                    int index = line.IndexOf(';');
                    string bitmapFileName = directoryName + "\\" + line.Substring(0, index);
                    char ch = Char.ToUpper(line.Substring(index + 1)[0]);

                    inputs[i] = LoadBitmap(bitmapFileName);
                    outputs[i] = ConvertCharToIntArray(ch);

                    ++i;
                }
            }
            else
            {
                MessageBox.Show("Cannot open file: " + fileName, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double[] LoadBitmap(string fileName)
        {

            if (File.Exists(fileName))
            {
                Bitmap bitmap = new Bitmap(fileName);

                double[] pixels = new double[bitmap.Width * bitmap.Height];

                for (int i = 0; i < bitmap.Height; ++i)
                {
                    for (int j = 0; j < bitmap.Width; ++j)
                    {
                        System.Drawing.Color pixel = bitmap.GetPixel(j, i);
                        pixels[i * bitmap.Width + j] = 1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0;
                    }
                }

                return pixels;
            }
            else
            {
                MessageBox.Show("Cannot open file: " + fileName, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        private int[] ConvertCharToIntArray(char c)
        {
            int[] array = new int[Network.OUTPUTS_COUNT];
            for (int i = 0; i < Network.OUTPUTS_COUNT; ++i)
            {
                array[i] = 0;
            }

            if (Char.IsLetter(c))
                array[c - 'A'] = 1;
            else if (Char.IsDigit(c))
                array[26 + c - '0'] = 1;

            return array;
        }
    }
}
