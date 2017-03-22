﻿using System;
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

namespace Deep_Neural_Text_Reader {
    /// <summary>
    /// Interaction logic for TestWordWindow.xaml
    /// </summary>
    public partial class TestWordWindow : Window {

        Network network;

        public TestWordWindow(Network network) {
            InitializeComponent();

            this.network = network;
        }
    }
}
