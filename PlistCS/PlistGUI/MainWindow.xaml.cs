using Dumpify;
using Microsoft.Win32;
using PlistCS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace PlistGUI;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged = null!;

    public string DumpText
    {
        get => (string)GetValue(DumpTextProperty);
        set
        {
            SetValue(DumpTextProperty, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DumpText)));
        }
    }

    public static readonly DependencyProperty DumpTextProperty =
        DependencyProperty.Register(nameof(DumpText), typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnOpenFile(string fileName)
    {
        if (File.Exists(fileName))
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)Plist.readPlist(fileName);
            DumpText = dict.DumpText();
        }
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                if (new FileInfo(files[0]).Extension?.Equals(".plist", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    OnOpenFile(files[0]);
                }
                else
                {
                    DumpText = $"The file '{files[0]}' is not a plist file.";
                }
            }
        }
    }

    private void OnOpenClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new()
        {
            Title = "Select the Info.plist file",
            RestoreDirectory = true,
            DefaultExt = "*.plist",
            Filter = "*.plist|*.plist",
        };

        if (dialog.ShowDialog() ?? false)
        {
            OnOpenFile(dialog.FileName);
        }
    }
}
