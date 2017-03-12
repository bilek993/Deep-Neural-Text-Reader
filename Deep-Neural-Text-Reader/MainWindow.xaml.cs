using Accord.Controls;
using Accord.Math;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading;

using Microsoft.Win32;
using System.IO;
using System.Drawing;

namespace Deep_Neural_Text_Reader
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SystemMonitor systemMonitor;
        private SeriesCollection errorCollections;
        private Network network;

        private const int OUTPUTS_COUNT = 36;

        private double[][] inputsForLearn;
        private int[][] outputsForLearn;

        public MainWindow()
        {
            InitializeComponent();

            TimerInitializer();
            SetSystemMonitor();
            InitializeLinearChart();
        }

        private void InitializeLinearChart()
        {
            errorCollections = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Error rate:",
                    Values = new ChartValues<double> {}
                }
            };
            linearChartOfLearning.Series = errorCollections;
        }

        private void TimerInitializer()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (1000);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            UpdateCpuAndRam();
            UpdateProgress();
            UpdateLearningGraph();
        }

        private void SetSystemMonitor()
        {
            systemMonitor = new SystemMonitor();
            ramUsageGauge.To = systemMonitor.getTotalRam();
        }

        private void UpdateCpuAndRam()
        {
            cpuUsageGauge.Value = systemMonitor.getCpuUsage();
            ramUsageGauge.Value = ramUsageGauge.To - systemMonitor.getRamUsage();        
        }

        private void UpdateProgress()
        {
            if (network != null)
                progressGauge.Value = network.CalculateProgress();
        }

        private void UpdateLearningGraph()
        {
            if (network != null)
                if (network.CalculateProgress() < 100)
                    errorCollections[0].Values.Add(network.error);
        }

        private void LearnNetwork(object iterationsCount)
        {
            double[][] input = new double[][] {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 }
            };
            int[] output = new int[] { 0, 1, 1, 0 };

            network = new Network(2, new[] { 4, 2, 1 });
            network.iterationCount = Convert.ToInt32(iterationsCount);

            network.Learn(input, output);

            double[] answers = network.CalculateAnswer(input).GetColumn(0);
            int[] answers2 = new int[answers.Length];
            for (int j = 0; j < answers.Length; ++j)
            {
                answers2[j] = (int)Math.Round(answers[j]);
            }
        }

        private void MenuItemLearn_Click(object sender, RoutedEventArgs e)
        {
            errorCollections[0].Values.Clear();

            Thread thread = new Thread(LearnNetwork);
            thread.Start((int)Math.Round(iterationsSlider.Value));
        }

        private void IterationsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            iterationsLabel.Content = "Learning iterations (" + iterationsSlider.Value + "):";
        }

        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files|*.*";
            dialog.InitialDirectory = Environment.CurrentDirectory;

            if (dialog.ShowDialog() == true)
            {
                LoadLearnData(dialog.FileName);
            }
        }

        private async void LoadLearnData(string fileName)
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

                inputsForLearn = new double[linesCount][];
                outputsForLearn = new int[linesCount][];

                int i = 0;
                foreach (string line in lines)
                {
                    int index = line.IndexOf(';');
                    string bitmapFileName = directoryName + "\\" + line.Substring(0, index);
                    char ch = Char.ToUpper(line.Substring(index + 1)[0]);

                    inputsForLearn[i] = LoadBitmap(bitmapFileName);
                    outputsForLearn[i] = ConvertCharToIntArray(ch);

                    ++i;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Cannot open file: " + fileName, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                System.Windows.MessageBox.Show("Cannot open file: " + fileName, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        private int[] ConvertCharToIntArray(char c)
        {
            int[] array = new int[OUTPUTS_COUNT];
            for (int i = 0; i < OUTPUTS_COUNT; ++i)
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
