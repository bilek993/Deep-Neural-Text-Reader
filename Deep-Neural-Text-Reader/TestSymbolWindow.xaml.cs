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
using System.Windows.Shapes;

namespace Deep_Neural_Text_Reader
{
    /// <summary>
    /// Logika interakcji dla klasy TestSymbolWindow.xaml
    /// </summary>
    public partial class TestSymbolWindow : Window
    {
        private Network network;

        public TestSymbolWindow(Network network)
        {
            InitializeComponent();

            this.network = network;
        }
    }
}
