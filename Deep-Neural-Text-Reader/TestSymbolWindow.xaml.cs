﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
        private Bitmap loadedImage;

        public TestSymbolWindow(Network network)
        {
            InitializeComponent();

            this.network = network;
            setDrawingParameters(25);
        }

        private void setDrawingParameters(int drawingSize)
        {
            inkDrawing.DefaultDrawingAttributes.Width = drawingSize;
            inkDrawing.DefaultDrawingAttributes.Height = drawingSize;
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

                    loadedImage = new Bitmap(fileName);
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
            for (int i = 0; i < loadedImage.Height; ++i)
            {
                for (int j = 0; j < loadedImage.Width; ++j)
                {
                    System.Drawing.Color pixel = loadedImage.GetPixel(j, i);
                    input[i * loadedImage.Width + j] = 1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0;
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

        private void ClearDrawing_Click(object sender, RoutedEventArgs e)
        {
            inkDrawing.Strokes.Clear();
        }

        private void TestSymbolFromDrawing_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = CreateBitmapFromInkCanvas(inkDrawing);
            loadedImage = new Bitmap(bitmap, 9, 16);

            imagePreview.Source = ConvertBitmapToSource(loadedImage);
            

            double[] input = new double[network.InputsCount];
            for (int i = 0; i < loadedImage.Height; ++i)
            {
                for (int j = 0; j < loadedImage.Width; ++j)
                {
                    System.Drawing.Color pixel = loadedImage.GetPixel(j, i);
                    input[i * loadedImage.Width + j] = 1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0;
                }
            }

            double[] output = network.CalculateAnswer(input);
            char answer = ConvertOutputArrayToChar(output);

            calculatedValue.Content = "Calculated value: " + answer;
        }

        private Bitmap CreateBitmapFromInkCanvas(InkCanvas canvas)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Default);
            inkDrawing.Measure(new System.Windows.Size((int)canvas.Width, (int)canvas.Height));
            inkDrawing.Arrange(new Rect(new System.Windows.Size((int)canvas.Width, (int)canvas.Height)));

            rtb.Render(inkDrawing);

            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            byte[] bitmapBytes;

            Bitmap bm;
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                ms.Position = 0;
                bitmapBytes = ms.ToArray();

                bm = new Bitmap(ms);
            }

            return bm;
        }

        private BitmapImage ConvertBitmapToSource(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)loadedImage).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }
    }
}
