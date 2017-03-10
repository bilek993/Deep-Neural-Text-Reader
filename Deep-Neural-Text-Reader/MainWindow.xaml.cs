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

namespace Deep_Neural_Text_Reader
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Network network;

        public MainWindow()
        {
            InitializeComponent();

            TestNetwork();
        }

        public void TestNetwork()
        {
            double[][] input = new double[][] {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 }
            };
            int[] output = new int[] { 0, 1, 1, 0 };

            network = new Network(2, new[] { 2, 2, 1 });
            network.iterationCount = 100;
            network.input = input;
            network.output = output;

            network.Learn();

            double[] answers = network.CalculateAnswer(network.input).GetColumn(0);
            int[] answers2 = new int[answers.Length];
            for (int j = 0; j < answers.Length; ++j)
            {
                answers2[j] = (int)Math.Round(answers[j]);
            }


            ScatterplotBox.Show("Expected answers", input, output);
            ScatterplotBox.Show("Network answers", input, answers2);
        }
    }
}
