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
            DrawGraph(output);

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
            char c = IndexToCharFromNetwork(maxIndex);

            return c;
        }

        private char IndexToCharFromNetwork(int index)
        {
            char c = '?';
            if (index < 26)
            {
                c = (char)('A' + index);
            }
            else
            {
                c = (char)('0' + index - 26);
            }

            return c;
        }

        private void DrawGraph(double[] output)
        {
            int maxIndex = 0;
            double[] graphValues = new double[4];

            for (int i = 1; i < output.Length; ++i)
            {
                if (output[i] > output[maxIndex])
                    maxIndex = i;
            }
            pieValue1.Title = IndexToCharFromNetwork(maxIndex).ToString();
            pieValue1.Values[0] = output[maxIndex];
            output[maxIndex] = -1;


            maxIndex = 0;
            graphValues = new double[4];

            for (int i = 1; i < output.Length; ++i)
            {
                if (output[i] > output[maxIndex])
                    maxIndex = i;
            }
            pieValue2.Title = IndexToCharFromNetwork(maxIndex).ToString();
            pieValue2.Values[0] = output[maxIndex];
            output[maxIndex] = -1;


            maxIndex = 0;
            graphValues = new double[4];

            for (int i = 1; i < output.Length; ++i)
            {
                if (output[i] > output[maxIndex])
                    maxIndex = i;
            }
            pieValue3.Title = IndexToCharFromNetwork(maxIndex).ToString();
            pieValue3.Values[0] = output[maxIndex];
            output[maxIndex] = -1;


            maxIndex = 0;
            graphValues = new double[4];

            for (int i = 1; i < output.Length; ++i)
            {
                if (output[i] > output[maxIndex])
                    maxIndex = i;
            }
            pieValue4.Title = IndexToCharFromNetwork(maxIndex).ToString();
            pieValue4.Values[0] = output[maxIndex];
            output[maxIndex] = -1;
        }

        private void ClearDrawing_Click(object sender, RoutedEventArgs e)
        {
            inkDrawing.Strokes.Clear();
        }

        private void DrawingToImage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
