﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public class Library
{
    public IEnumerable<string> GetTemplates()
    {
        return Enum.GetValues(typeof(TileTemplateType)).Cast<TileTemplateType>()
            .Select(s => s.ToString()).Distinct();
    }

    private void UpdateTile(string style, string value)
    {
        TileTemplateType template = (TileTemplateType)Enum.Parse(typeof(TileTemplateType), style);
        XmlDocument tile = TileUpdateManager.GetTemplateContent(template);
        XmlNodeList text = tile.GetElementsByTagName("text");
        if (text.Length > 0)
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i].AppendChild(tile.CreateTextNode(value));
            }
        }
        XmlNodeList image = tile.GetElementsByTagName("image");
        if (image.Length > 0)
        {
            for (int i = 0; i < image.Length; i++)
            {
                image[i].Attributes.GetNamedItem("src").NodeValue =
                "Assets/Square44x44Logo.scale-200.png";
            }
        }
        TileNotification notification = new TileNotification(tile);
        string output = tile.GetXml();
        TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
    }

    private async Task<Tuple<string, string>> Dialog()
    {
        ComboBox template = new ComboBox()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(5),
            ItemsSource = GetTemplates()
        };
        template.SelectedIndex = 0;
        TextBox text = new TextBox()
        {
            PlaceholderText = "Text",
            Margin = new Thickness(5)
        };
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Vertical
        };
        panel.Children.Add(template);
        panel.Children.Add(text);
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Tile Styles",
            PrimaryButtonText = "Update",
            CloseButtonText = "Cancel",
            Content = panel
        };
        ContentDialogResult result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            return new Tuple<string, string>((string)template.SelectedItem, text.Text);
        }
        return null;
    }

    public async Task<bool> PinAsync()
    {
        bool isPinned = false;
        AppListEntry entry = (await Package.Current.GetAppListEntriesAsync()).FirstOrDefault();
        if (entry != null)
        {
            isPinned = await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry);
        }
        if (!isPinned)
        {
            isPinned = await StartScreenManager.GetDefault().RequestAddAppListEntryAsync(entry);
        }
        return isPinned;
    }

    public async void Tile()
    {
        Tuple<string, string> result = await Dialog();
        if (result != null)
        {
            UpdateTile(result.Item1, result.Item2);
        }
    }
}