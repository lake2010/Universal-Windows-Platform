﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private double[] box_values =
    {
            0.01, 0.10, 0.50, 1, 5, 10, 50, 100, 250, 500, 750,
            1000, 3000, 5000, 10000, 15000, 20000, 35000, 50000, 75000, 100000, 250000
    };
    private string[] box_colors =
    {
        "0026ff", "0039ff", "004dff", "0060ff", "0073ff", "0086ff", "0099ff", "0099ff", "0099ff", "00acff", "00bfff",
        "ff5900", "ff4d00", "ff4000", "ff3300", "ff2600", "ff2600", "ff2600", "ff2600", "ff1a00", "ff1c00", "ff0d00",
    };
    private string[] box_names = {
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k",
        "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v"
    };

    private Random random = new Random((int)DateTime.Now.Ticks);
    private List<double> amounts = new List<double>();
    private double amount;
    private bool dealt;
    private int turn;

    public static Color ConvertHexToColor(string hex)
    {
        hex = hex.Remove(0, 1);
        byte a = hex.Length == 8 ? Byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) : (byte)255;
        byte r = Byte.Parse(hex.Substring(hex.Length - 6, 2), NumberStyles.HexNumber);
        byte g = Byte.Parse(hex.Substring(hex.Length - 4, 2), NumberStyles.HexNumber);
        byte b = Byte.Parse(hex.Substring(hex.Length - 2), NumberStyles.HexNumber);
        return Color.FromArgb(a, r, g, b);
    }

    private async Task<ContentDialogResult> ShowDialogAsync(string primary, string secondary, object content)
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Deal Or Not",
            PrimaryButtonText = primary,
            SecondaryButtonText = secondary,
            Content = content
        };
        return await dialog.ShowAsync();
    }

    private List<int> Shuffle(int total)
    {
        return Enumerable.Range(0, total).OrderBy(r => random.Next(0, total)).ToList();
    }

    private Grid GetAmount(double value, Color background)
    {
        Grid grid = new Grid()
        {
            Background = new SolidColorBrush(background)
        };
        TextBlock text = new TextBlock()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Colors.White),
            Margin = new Thickness(10),
            FontSize = 33,
            Text = String.Format(new CultureInfo("en-GB"), "{0:c}", value)
        };
        grid.Children.Add(text);
        return grid;
    }

    private double GetOffer()
    {
        int count = 0;
        double total = 0.0;
        foreach (double amount in amounts)
        {
            total += amount;
            count++;
        }
        double average = total / count;
        double offer = (average * turn) / 10;
        return Math.Round(offer, 0);
    }

    private Color GetBackground(double amount)
    {
        int position = 0;
        while (amount != box_values[position])
        {
            position++;
        }
        return ConvertHexToColor($"#ff{box_colors[position]}");
    }

    private async void Pick(Button button, string name)
    {
        if (turn < box_names.Length && !dealt)
        {
            double offer = 0;
            button.Opacity = 0;
            amount = amounts[Array.IndexOf(box_names, name)];
            await ShowDialogAsync("OK", string.Empty, GetAmount(amount, GetBackground(amount)));
            if (turn % 5 == 0 && turn > 1)
            {
                offer = GetOffer();
                ContentDialogResult result = await ShowDialogAsync("Deal", "Not", GetAmount(offer, Colors.Black));
                if (result == ContentDialogResult.Primary)
                {
                    amount = offer;
                    dealt = true;
                }
            }
            turn++;
        }
        if (turn == box_names.Length || dealt)
        {
            object content = dealt ? GetAmount(amount, Colors.Black) : GetAmount(amount, GetBackground(amount));
            await ShowDialogAsync("Game Over", string.Empty, content);
        }
    }

    private void AddBox(ref StackPanel panel, string name, int value)
    {
        Button button = new Button()
        {
            Name = $"box.{name}"
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            Pick((Button)sender, name);
        };
        StackPanel box = new StackPanel()
        {
            Width = 100
        };
        Rectangle lid = new Rectangle()
        {
            Height = 10,
            Fill = new SolidColorBrush(Colors.DarkRed)
        };
        Grid front = new Grid()
        {
            Height = 75,
            Background = new SolidColorBrush(Colors.Red)
        };
        Grid label = new Grid()
        {
            Width = 50,
            Background = new SolidColorBrush(Colors.White),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        TextBlock text = new TextBlock()
        {
            TextAlignment = TextAlignment.Center,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 32,
            Text = value.ToString()
        };
        label.Children.Add(text);
        front.Children.Add(label);
        box.Children.Add(front);
        button.Content = box;
        panel.Children.Add(button);
    }

    private StackPanel Boxes()
    {
        StackPanel panel = new StackPanel();
        int[] rows = { 5, 6, 6, 5 };
        int count = 0;
        for (int r = 0; r < 4; r++)
        {
            StackPanel places = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            for (int c = 0; c < rows[r]; c++)
            {
                AddBox(ref places, box_names[count], count + 1);
                count++;
            }
            panel.Children.Add(places);
        }
        return panel;
    }

    private void Layout(ref Grid grid)
    {
        grid.Children.Clear();
        Viewbox view = new Viewbox()
        {
            Child = Boxes()
        };
        grid.Children.Add(view);
    }

    public void New(ref Grid grid)
    {
        turn = 0;
        dealt = false;
        List<int> positions = Shuffle(22);
        amounts = new List<double>();
        foreach (int position in positions)
        {
            amounts.Add(box_values[position]);
        }
        Layout(ref grid);
    }
}