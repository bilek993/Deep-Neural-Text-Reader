using Microsoft.Win32;
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

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
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

                    Bitmap loadedImage = new Bitmap(fileName);
                    imageCutter = new ImageCutter(loadedImage);
                    imageCutter.FindMinMaxlValue();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Cannot open file: " + fileName, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
