using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using SharedLibrary.Factory;
using SharedLibrary.Services;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;

namespace WindowApp;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const byte AWS_POS = 0;

    private const byte OVH_POS = 1;

    private readonly ICloudService?[] _connectedClouds = new ICloudService[2];

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

        if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) wbLocalFiles.Source = new Uri(fbd.SelectedPath);
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

        (await _cloud.ListFilesAsync(selectedBucket.ToString() ?? string.Empty))
            .ForEach(o => lvBucketObjects.Items.Add(o.Key));
    }

    private void lvBucketObjects_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Note that you can have more than one file.
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];

            e.Handled = true;
            // handle the files here!
        }
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

        switch (radioText)
        {
            case "AWS":
                _connectedClouds[AWS_POS] = await new AwsServiceFactory().CreateCloudService();

                _cloud = _connectedClouds[AWS_POS];

                break;

            case "OVH Cloud":
                // cloudServices[OVH_POS] = new OvhServiceFactory().CreateCloudService();
                //
                // selectedCloud = cloudServices[OVH_POS];

                _cloud = null;

                break;
        }

        cbBuckets.Items.Clear();

        if (_cloud == null)
        {
            MessageBox.Show(
                $"La conexión a {radioText} o los permisos han fallado. " +
                "Revisa tus credenciales y vuelve a intentarlo.");

            ((RadioButton)sender).IsChecked = false;
            return;
        }

        (await _cloud.ListBucketsAsync())
            .ForEach(b => cbBuckets.Items.Add(b.BucketName));

        cbBuckets.SelectedIndex = 0;
    }

    private async void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (lvBucketObjects.SelectedItem == null) return;

        using var fbw = new FolderBrowserDialog();

        fbw.ShowDialog();

        MessageBox.Show(fbw.SelectedPath + " | " + await _cloud?
            .DownloadFileAsync(cbBuckets.SelectedItem.ToString(),
                lvBucketObjects.SelectedItem.ToString(),
                fbw.SelectedPath));
    }
}