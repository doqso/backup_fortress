using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WindowApp.Factory;
using ComboBox = System.Windows.Controls.ComboBox;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using RadioButton = System.Windows.Controls.RadioButton;

namespace WindowApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICloudService cloud;

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
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    wbLocalFiles.Source = new Uri(fbd.SelectedPath);
                }
            }
        }

        private void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
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
            string newUrl = wbLocalFiles.Source.AbsolutePath
                .Substring(0,
                    wbLocalFiles.Source.AbsolutePath
                        .LastIndexOf("/", StringComparison.Ordinal));

            wbLocalFiles.Source = new Uri(newUrl + "/");
        }

        private void webBrowser_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Back) btnBack_Click(sender, e);
        }

        private async void cbBuckets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvBucketObjects.Items.Clear();
            
            var selectedBucket = cbBuckets.SelectedItem;
            if (selectedBucket == null) return;

            (await cloud.ListFilesAsync(selectedBucket.ToString() ?? string.Empty))
                .ForEach(o => lvBucketObjects.Items.Add(o.Key));
        }

        private void lvBucketObjects_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];

                e.Handled = true;
                // handle the files here!
            }
        }

        private void lvBucketObjects_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (cbBuckets.SelectedItem != null) return;

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioText = ((sender as RadioButton)?.Content as TextBlock)?.Text;
            laCloudProviderName.Content = radioText;

            switch (radioText)
            {
                case "AWS":
                    cloud = new AwsServiceFactory().CreateCloudService();

                    cbBuckets.Items.Clear();

                    (await cloud.ListBucketsAsync())
                        .ForEach(b => cbBuckets.Items.Add(b.BucketName));
                    break;
                /*case "OVH Cloud":
                    cloud = new OvhServiceFactory().CreateCloudService();
                    break;
                case "Google":
                    cloud = new GoogleServiceFactory().CreateCloudService();
                    break;*/
            }

            cbBuckets.SelectedIndex = 0;
        }
    }
}