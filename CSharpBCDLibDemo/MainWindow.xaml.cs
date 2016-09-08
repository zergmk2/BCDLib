// Copyright (c) 2016 Lu Cao
// 
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php

using CSharpBCDLib;
using CSharpBCDLibDemo.Model;
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

namespace CSharpBCDLibDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BcdModel model;
        public MainWindow()
        {
            InitializeComponent();
            model = new BcdModel();
        }

        private void RefreshBCDComboxBox()
        {
            model.RefreshOSLoaderObjects();
            comboBox.SelectedItem = null;
            comboBox.Items.Clear();
            foreach (BcdObject obj in model.OSLoaderObjects)
            {
                comboBox.Items.Add(new BCDComboBoxItem(obj));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshBCDComboxBox();
        }
    }

    internal class BCDComboBoxItem
    {
        private BcdObject bcdObj;
        public string ItemName;
        public BCDComboBoxItem(BcdObject obj)
        {
            if (obj != null)
            {
                bcdObj = obj;
                BcdElement element = bcdObj.elementsDict[(uint)BuiltinElementType.BcdLibraryString_Description];
                if (element is BcdStringElement)
                {
                    ItemName = ((BcdStringElement)element).StringValue;
                }
            }
        }

        public override string ToString()
        {
            return ItemName;
        }
    }
}
