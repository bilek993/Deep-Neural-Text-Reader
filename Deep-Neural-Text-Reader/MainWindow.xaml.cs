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

        private void TestNetwork(object iterationsCount)
        {
            double[][] input = new double[][] {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 }
            };
            int[] output = new int[] { 0, 1, 1, 0 };

            network = new Network(2, new[] { 2, 2, 1 });
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

            Thread thread = new Thread(TestNetwork);
            thread.Start((int)Math.Round(iterationsSlider.Value));
        }

        private void IterationsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            iterationsLabel.Content = "Learning iterations (" + iterationsSlider.Value + "):";
        }
    }
}
