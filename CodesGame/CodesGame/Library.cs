﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class CommandHandler : ICommand
{
    public event EventHandler CanExecuteChanged = null;
    private Action _action;

    public CommandHandler(Action action)
    {
        _action = action;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        _action();
    }
}

public class Code : INotifyPropertyChanged
{
    private int _value;
    private SolidColorBrush _foreGround;
    private SolidColorBrush _backGround;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public int Index { get; set; }
    public int Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }
    public SolidColorBrush Foreground { get { return _foreGround; } set { _foreGround = value; OnPropertyChanged("Foreground"); } }
    public SolidColorBrush Background { get { return _backGround; } set { _backGround = value; OnPropertyChanged("Background"); } }
    public Func<int, int> Action { get; set; }
    public ICommand Command { get { return new CommandHandler(() => this.Action(Index)); } }
    public event PropertyChangedEventHandler PropertyChanged;
}

public class Library
{
    private ObservableCollection<Code> codes = new ObservableCollection<Code>();
    private Random random = new Random((int)DateTime.Now.Ticks);
    private List<int> numbers = new List<int>();
    private int turns = 0;

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    private Code GetCode(int value, int index)
    {
        return new Code()
        {
            Action = (int i) =>
            {
                Code code = codes[i];
                code.Value = (code.Value == 4) ? 1 : code.Value + 1;
                code.Foreground = new SolidColorBrush(Colors.Black);
                code.Background = new SolidColorBrush(Colors.White);
                return code.Value;
            },
            Index = index,
            Value = value,
            Foreground = new SolidColorBrush(Colors.Black),
            Background = new SolidColorBrush(Colors.White)
        };
    }

    private bool Check(int number, int index)
    {
        Code code = codes[index];
        if (number == code.Value)
        {
            code.Foreground = new SolidColorBrush(Colors.Black);
            code.Background = new SolidColorBrush(Colors.White);
            return true;
        }
        else
        {
            code.Foreground = new SolidColorBrush(Colors.White);
            code.Background = new SolidColorBrush(Colors.Black);
            return false;
        }
    }

    private List<int> Shuffle(int start, int total)
    {
        return Enumerable.Range(start, total).OrderBy(r => random.Next(start, total)).ToList();
    }

    public void New(ref ItemsControl items)
    {
        turns = 0;
        codes.Clear();
        for (int i = 0; i < 4; i++)
        {
            codes.Add(GetCode(i + 1, i));
        }
        items.ItemsSource = codes;
        numbers = Shuffle(1, 4);
    }

    public void Accept(ref ItemsControl items)
    {
        int index = 0;
        int correct = 0;
        foreach (int number in numbers)
        {
            if (Check(number, index))
            {
                correct++;
            }
            index++;
        }
        turns++;
        if (correct == 4)
        {
            Show($"You got all the numbers correct in {turns} turns!", "Codes Game");
            New(ref items);
        }
    }
}