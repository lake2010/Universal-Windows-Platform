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

namespace ContactsApp
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

        Library library = new Library();

        private async void Display_Loaded(object sender, RoutedEventArgs e)
        {
            Display.ItemsSource = await library.ListAsync();
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (await library.Add(Add))
            {
                Display.ItemsSource = await library.ListAsync();
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (await library.Edit((AppBarButton)sender))
            {
                Display.ItemsSource = await library.ListAsync();
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (await library.Delete((AppBarButton)sender))
            {
                Display.ItemsSource = await library.ListAsync();
            }
        }
    }
}
