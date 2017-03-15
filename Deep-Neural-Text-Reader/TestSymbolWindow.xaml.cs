using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Deep_Neural_Text_Reader
{
    /// <summary>
    /// Logika interakcji dla klasy TestSymbolWindow.xaml
    /// </summary>
    public partial class TestSymbolWindow : Window
    {
        private Network network;
        private Bitmap loadetImage;

        public TestSymbolWindow(Network network)
        {
            InitializeComponent();

            this.network = network;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image files (*.bmp, *.jpg, *.jpeg, *.jpe, *.png) | *.bmp; *.jpg; *.jpeg; *.jpe; *.png|All files|*.*";
            dialog.InitialDirectory = Environment.CurrentDirectory;

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;

                try
                {
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
                    src.EndInit();
                    imagePreview.Source = src;

                    loadetImage = new Bitmap(fileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Cannot open file: " + fileName, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TestSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            double[] input = new double[network.InputsCount];
            for (int i = 0; i < loadetImage.Height; ++i)
            {
                for (int j = 0; j < loadetImage.Width; ++j)
                {
                    System.Drawing.Color pixel = loadetImage.GetPixel(j, i);
                    input[i * loadetImage.Width + j] = 1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0;
                }
            }

            double[] output = network.CalculateAnswer(input);
            char answer = ConvertOutputArrayToChar(output);

            calculatedValue.Content = "Calculated value: " + answer;
        }

        private char ConvertOutputArrayToChar(double[] output)
        {
            int maxIndex = 0;
            for (int i = 1; i < output.Length; ++i)
            {
                if (output[i] > output[maxIndex])
                    maxIndex = i;
            }
            char c = '?';
            if (maxIndex < 26)
            {
                c = (char)('A' + maxIndex);
            }
            else
            {
                c = (char)('0' + maxIndex - 26);
            }

            return c;
        }
    }
}
