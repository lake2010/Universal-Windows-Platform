﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StackedControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Display_Loaded(object sender, RoutedEventArgs e)
        {
            Display.Palette = new List<Windows.UI.Color>()
            {
                Windows.UI.Colors.Black,
                Windows.UI.Colors.Gray,
                Windows.UI.Colors.Red,
                Windows.UI.Colors.Orange,
                Windows.UI.Colors.Yellow,
                Windows.UI.Colors.Green,
                Windows.UI.Colors.Cyan,
                Windows.UI.Colors.Blue,
                Windows.UI.Colors.Magenta,
                Windows.UI.Colors.Purple,
            };
            Func<int, int> fibonacci = null;
            fibonacci = value => value > 1 ?
                fibonacci(value - 1) + fibonacci(value - 2) : value;
            Display.Items = Enumerable.Range(0, Display.Palette.Count())
                .Select(fibonacci).Select(s => (double)s).ToList();
        }
    }
}
