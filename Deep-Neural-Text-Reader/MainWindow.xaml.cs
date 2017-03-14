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
                if (network.CalculateProgress() < 100 && network.error > 0)
                    errorCollections[0].Values.Add(network.error);
        }

        private void LearnNetwork(object iterationsCount)
        {
            if (inputsForLearn == null || outputsForLearn == null)
                return;

            if (network == null)
            {
                network = new Network(inputsForLearn[0].Length, GetHiddenLayers());
            }
            network.iterationCount = Convert.ToInt32(iterationsCount);

            network.Learn(inputsForLearn, outputsForLearn);

            double[] answers = network.CalculateAnswer(inputsForLearn).GetColumn(0);
            int[] answers2 = new int[answers.Length];
            for (int j = 0; j < answers.Length; ++j)
            {
                answers2[j] = (int)Math.Round(answers[j]);
            }
        }

        private int[] GetHiddenLayers()
        {
            int[] hiddenLayers = new int[neuronsList.Items.Count + 1];
            hiddenLayers[hiddenLayers.Length - 1] = Network.OUTPUTS_COUNT;

            for (int i = 0; i < hiddenLayers.Length - 1; i++)
            {
                NeuronItem item = neuronsList.Items.GetItemAt(i) as NeuronItem;
                hiddenLayers[i] = item.Neurons;
            }

            return hiddenLayers;
        }

        private void LockNeuronList()
        {
            neuronsList.IsEnabled = false;
            numberOfNeuronsTextBox.IsEnabled = false;
            buttonAdd.IsEnabled = false;
            buttonRemove.IsEnabled = false;
        }

        private void MenuItemLearn_Click(object sender, RoutedEventArgs e)
        {
            LockNeuronList();
            errorCollections[0].Values.Clear();

            Thread thread = new Thread(LearnNetwork);
            thread.Start((int)Math.Round(iterationsSlider.Value));
        }

        private void IterationsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            iterationsLabel.Content = "Learning iterations (" + iterationsSlider.Value + "):";
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files|*.*";
            dialog.InitialDirectory = Environment.CurrentDirectory;

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                pathLabel.Content = "PATH: " + fileName;

                LearnDataLoader loader = new LearnDataLoader();
                loader.LoadLearnData(fileName);
                inputsForLearn = loader.Inputs;
                outputsForLearn = loader.Outputs;
                SetInputAndOutputLabels(inputsForLearn[0].Length);
            }
        }

        private void SetInputAndOutputLabels(int inputSize)
        {
            inputSizeLabel.Content = "Inputs count: " + inputSize.ToString();
            outputSizeLabel.Content = "Output count: " + Network.OUTPUTS_COUNT;
        }

        private void MenuItemFileExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files|*.*";
            dialog.InitialDirectory = Environment.CurrentDirectory;

            if (dialog.ShowDialog() == true)
            {
                ConfigFileManager configFileManager = new ConfigFileManager();

                network = configFileManager.LoadConfig(dialog.FileName);

                iterationsSlider.Value = configFileManager.iterationsCount;
                inputSizeLabel.Content = network.InputsCount;
                outputSizeLabel.Content = network.OutputsCount;

                for (int i = 0; i < network.NeuronsCount.Length - 1; ++i)
                {
                    AddHiddenLayer(network.NeuronsCount[i]);
                }
                LockNeuronList();

            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files|*.*";
            dialog.InitialDirectory = Environment.CurrentDirectory;

            if (dialog.ShowDialog() == true && network != null)
            {
                ConfigFileManager configFileManager = new ConfigFileManager();
                configFileManager.iterationsCount = (int)Math.Round(iterationsSlider.Value);
                configFileManager.SaveConfig(dialog.FileName, network);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void AddHiddenLayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int neuronsCount = Int32.Parse(numberOfNeuronsTextBox.Text);
                numberOfNeuronsTextBox.Text = "";
                AddHiddenLayer(neuronsCount);
            }
            catch
            {
                numberOfNeuronsTextBox.Text = "Problem with parsing data!";
            }
        }

        private void AddHiddenLayer(int neuronsCount)
        {
            neuronsList.Items.Add(new NeuronItem { Id = neuronsList.Items.Count, Neurons = neuronsCount });
        }

        private void RemoveHiddenLayer_Click(object sender, RoutedEventArgs e)
        {
            neuronsList.Items.RemoveAt(neuronsList.SelectedIndex);
        }
    }
}
