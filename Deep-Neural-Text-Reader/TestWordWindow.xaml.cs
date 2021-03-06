﻿using Microsoft.Win32;
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

namespace Deep_Neural_Text_Reader {
    /// <summary>
    /// Interaction logic for TestWordWindow.xaml
    /// </summary>
    public partial class TestWordWindow : Window {

        Network network;
        ImageCutter imageCutter;

        public TestWordWindow(Network network) {
            InitializeComponent();

            this.network = network;
        }

        private void VerifyWord(String valueToVerify)
        {
            string suggestionsForExpander = String.Empty;
            TextBox txt = new TextBox();
            txt.SpellCheck.IsEnabled = true;
            txt.Text = valueToVerify.ToLower();
            SpellingError result = txt.GetSpellingError(0);

            if (result != null)
            {
                foreach (string s in result.Suggestions)
                    suggestionsForExpander += string.Format("{0}\n", s);
            }

            expanderSuggestions.Content = suggestionsForExpander;
            expanderSuggestions.IsExpanded = true;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.bmp, *.jpg, *.jpeg, *.jpe, *.png) | *.bmp; *.jpg; *.jpeg; *.jpe; *.png|All files|*.*";
            dialog.InitialDirectory = Environment.CurrentDirectory;

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                Bitmap loadedImage;

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
                    return;
                }

                imageCutter = new ImageCutter(loadedImage);
                imageCutter.FindMinMaxlValue();
                imageCutter.CutWord();

                StringBuilder wordStringBuilder = new StringBuilder("");
                for (int i = 0; i < imageCutter.listOfLetters.Count; ++i)
                {
                    double[] input = network.BitmapToNetworkInput(imageCutter.listOfLetters[i]);

                    double[] output = network.CalculateAnswer(input);
                    char answer = network.NetworkOutputToChar(output);

                    wordStringBuilder.Append(answer);
                }

                string word = wordStringBuilder.ToString();
                detectedWordLabel.Content = "Detected word: " + word;
                VerifyWord(word);
            }
        }
    }
}
