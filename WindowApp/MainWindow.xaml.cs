using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using Shared.Factory;
using Shared.Services;
using Shared.util;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;
using Timer = System.Timers.Timer;

namespace WindowApp;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ICloudService? _cloud;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        wbLocalFiles.Source = new Uri(@"C:\");
        laRoute.Text = wbLocalFiles.Source.LocalPath;
    }

    private void btSelectFolder_Click(object sender, RoutedEventArgs e)
    {
        using var fbd = new FolderBrowserDialog();

        if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            wbLocalFiles.Source = new Uri(fbd.SelectedPath);
    }

    private void webBrowser_Navigated(object sender, NavigationEventArgs e)
    {
        laRoute.Text = wbLocalFiles.Source.LocalPath;
        wbLocalFiles.Focus();
    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
        if (wbLocalFiles.CanGoBack) wbLocalFiles.GoBack();
    }

    private void btnForward_Click(object sender, RoutedEventArgs e)
    {
        if (wbLocalFiles.CanGoForward) wbLocalFiles.GoForward();
    }

    private void btGoParent_Click(object sender, RoutedEventArgs e)
    {
        var newUrl = wbLocalFiles.Source
            .AbsolutePath[..wbLocalFiles.Source.AbsolutePath
                .LastIndexOf("/", StringComparison.Ordinal)];

        wbLocalFiles.Source = new Uri(newUrl + "/");
    }

    private void webBrowser_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Back) btnBack_Click(sender, e);
    }

    private async void cbBuckets_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        lvBucketObjects.Items.Clear();

        var selectedBucket = cbBuckets.SelectedItem;
        if (selectedBucket == null || _cloud == null) return;

        (await _cloud.ListFilesAsync(selectedBucket.ToString()))
            .ForEach(o => lvBucketObjects.Items.Add(o.Key));
    }

    private void lvBucketObjects_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

        // Note that you can have more than one file.
        var files = e.Data.GetData(DataFormats.FileDrop) as string[];

        e.Handled = true;
        // handle the files here!
    }

    private void lvBucketObjects_DragOver(object sender, DragEventArgs e)
    {
        if (cbBuckets.SelectedItem != null) return;

        e.Effects = DragDropEffects.None;
        e.Handled = true;
    }

    private async void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var radioText = ((sender as RadioButton)?.Content as TextBlock)?.Text;

        _cloud = radioText switch
        {
            "AWS" => await new AwsServiceFactory().CreateCloudService(),
            "OVH Cloud" => null,
            _ => await new AwsServiceFactory().CreateCloudService(),
        };

        cbBuckets.Items.Clear();

        if (_cloud == null)
        {
            MessageBox.Show(
                $"La conexión a {radioText} o los permisos han fallado. " +
                "Revisa tus credenciales y vuelve a intentarlo.");

            (sender as RadioButton)!.IsChecked = false;
            return;
        }

        (await _cloud.ListBucketsAsync())
            .ForEach(b => cbBuckets.Items.Add(b.BucketName));

        cbBuckets.SelectedIndex = 0;
    }

    private async void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (lvBucketObjects.SelectedItem == null) return;

        using var fbw = new SaveFileDialog();
        
        fbw.Filter = "All files (*.*)|*.*";
        fbw.FileName = lvBucketObjects.SelectedItem.ToString();
        
        var dialogResult = fbw.ShowDialog();

        if (dialogResult != System.Windows.Forms.DialogResult.OK) return;

        var downloadedFile = _cloud?
            .DownloadFileAsync(cbBuckets.SelectedItem.ToString(),
                lvBucketObjects.SelectedItem.ToString(),
                fbw.FileName);

        MessageBox.Show(fbw.FileName + " | " + await downloadedFile!);
    }

    private void MenuItem_Click_1(object sender, RoutedEventArgs e)
    {
        var synchronizedFiles = new SynchronizedFiles();
        synchronizedFiles.ShowDialog();
        synchronizedFiles.Close();
    }
}