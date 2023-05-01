using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Shared.util;
using WindowApp.models;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;

namespace WindowApp
{
    /// <summary>
    /// Lógica de interacción para SynchronizedFiles.xaml
    /// </summary>
    public partial class SynchronizedFiles : Window
    {
        private readonly ObservableCollection<CloudFileWrapper> files;

        public SynchronizedFiles()
        {
            InitializeComponent();

            files = new ObservableCollection<CloudFileWrapper>();

            ConfigFileIO.ReadSynchronizedFiles()
                .Select(x => new CloudFileWrapper(x))
                .ToList()
                .ForEach(d => files.Add(d));

            DgFiles.ItemsSource = files;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            var cloudFiles = files.Select(x => x.GetCloudFile()).ToList();

            ConfigFileIO.WriteSynchronizedFiles(cloudFiles);

            MessageBox.Show("Guardado ✔", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                files.Remove((CloudFileWrapper)DgFiles.SelectedItem);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        private void btLocalFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(((Button)sender).Content.ToString());
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}