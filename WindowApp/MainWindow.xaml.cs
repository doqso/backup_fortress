using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using Amazon.S3.Model;
using Shared.Factory;
using Shared.Services;
using Shared.util;
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
    public static ICloudService? Cloud;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        wbLocalFiles.Source = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
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
        tvBucketObjects.Items.Clear();

        var selectedBucket = cbBuckets.SelectedItem;
        if (selectedBucket == null || Cloud == null) return;


        (await Cloud.ListFilesAsync(selectedBucket.ToString()))
            .Select(o =>
            {
                var root = new TreeViewItem { Header = o.Key.Key, Uid = o.Key.VersionId, Tag = o.Key };

                o.Value.ForEach(v =>
                root.Items
                .Add(new TreeViewItem { Header = v.Key + " | " + v.LastModified, Uid = v.VersionId, Tag = v }));

                return root;
            }).ToList()
            .ForEach(e => tvBucketObjects.Items.Add(e));
    }

    private async void tvBucketObjects_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

        // Note that you can have more than one file.
        var files = e.Data.GetData(DataFormats.FileDrop) as string[];

        e.Handled = true;

        // handle the files here!

        // ...
        if (files == null || files.Length < 1) return;

        var filePath = files[0];

        var selectedItem = cbBuckets.SelectedItem.ToString();

        new Thread(async () =>
        {
            var isDirectory = Directory.Exists(filePath);

            if (isDirectory) FilesIO.CompressAndWrite(ref filePath);

            await Cloud?.UploadFileAsync(selectedItem, filePath);

            if (isDirectory) FilesIO.RemoveFoldersBackup(filePath);

            Dispatcher.Invoke(() => cbBuckets_SelectionChanged(sender, null));
        }).Start();
    }

    private void tvBucketObjects_DragOver(object sender, DragEventArgs e)
    {
        if (cbBuckets.SelectedItem != null) return;

        e.Effects = DragDropEffects.None;
        e.Handled = true;
    }

    private async void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var radioText = ((sender as RadioButton)?.Content as TextBlock)?.Text;
        var credentials = ConfigIO.ReadAccountCredentials("Aws");

        Cloud = radioText switch
        {
            "AWS" => await new AwsServiceFactory().CreateCloudService(credentials.AccessKey,
                credentials.SecretAccessKey),
            "OVH Cloud" => null,
            _ => await new AwsServiceFactory().CreateCloudService(credentials.AccessKey, credentials.SecretAccessKey),
        };


        if (Cloud == null)
        {
            MessageBox.Show(
                $"La conexión a {radioText} o los permisos han fallado. " +
                "Revisa tus credenciales y vuelve a intentarlo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            (sender as RadioButton)!.IsChecked = false;
            return;
        }

        RefreshBucketList();
    }

    private async Task RefreshBucketList()
    {
        cbBuckets.Items.Clear();

        (await Cloud?.ListBucketsAsync())
            .ForEach(b => cbBuckets.Items.Add(b.BucketName));

        if (cbBuckets.Items.Count > 0)
            cbBuckets.SelectedIndex = 0;
    }

    private async void idDownloadBucketItem_Click(object sender, RoutedEventArgs e)
    {
        if (tvBucketObjects.SelectedItem == null) return;

        var item = (tvBucketObjects.SelectedItem as TreeViewItem)?.Tag as S3ObjectVersion;

        using var fbw = new SaveFileDialog();

        fbw.Filter = "All files (*.*)|*.*";
        fbw.FileName = item?.Key.ToString();

        var dialogResult = fbw.ShowDialog();

        if (dialogResult != System.Windows.Forms.DialogResult.OK) return;

        var downloadedFile = await Cloud?
            .DownloadFileAsync(
                cbBuckets.SelectedItem.ToString(),
                item?.Key.ToString(),
                item?.VersionId,
                fbw.FileName);

        if (downloadedFile.Equals(HttpStatusCode.OK))
            MessageBox.Show("Descarga de " + fbw.FileName + " completada",
                "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        else MessageBox.Show("La descarga del archivo ha fallado",
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);

    }

    private void btAwsAccount_Click(object sender, RoutedEventArgs e)
    {
        var credentials = ConfigIO.ReadAccountCredentials("Aws");

        var awsManager = new ManageAccountWindow(credentials);

        awsManager.Title = "Cuenta de AWS";
        awsManager.ShowDialog();

        awsManager.Close();
    }

    private void btSyncFiles_Click(object sender, RoutedEventArgs e)
    {
        var synchronizedFiles = new SynchronizedFilesWindow();
        synchronizedFiles.ShowDialog();
        synchronizedFiles.Close();
    }

    private void itUpdateBucketList_Click(object sender, RoutedEventArgs e)
    {
        cbBuckets_SelectionChanged(sender, null);
    }

    private async void itDeleteBucketItem_Click(object sender, RoutedEventArgs e)
    {
        var selectedBucket = cbBuckets.SelectedItem;
        if (selectedBucket == null || Cloud == null) return;

        var selectedItem = (tvBucketObjects.SelectedItem as TreeViewItem)?.Tag as S3ObjectVersion;
        if (selectedItem == null) return;

        await Cloud.DeleteFileAsync(selectedBucket.ToString(), selectedItem.Key.ToString(), selectedItem.VersionId);

        cbBuckets_SelectionChanged(sender, null);
    }

    private void itCreateBucket_Click(object sender, RoutedEventArgs e)
    {
        var createBucketWnd = new CreateBucketWindow();

        var result = createBucketWnd.ShowDialog();

        if (result.HasValue && result.Value)
        {
            RefreshBucketList();
        }
    }

    private void itDeleteBucket_Click(object sender, RoutedEventArgs e)
    {
        DeleteBucketWindow wnd = new DeleteBucketWindow(cbBuckets.Items.OfType<string>());

        wnd.laService.Content = Cloud?.Name;

        var result = wnd.ShowDialog();

        if (result.HasValue && result.Value)
        {
            RefreshBucketList();
        }
    }
}