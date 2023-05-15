using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Shared.util;
using WindowApp.models;
using FileDialog = Microsoft.Win32.FileDialog;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WindowApp
{
    /// <summary>
    /// Lógica de interacción para SynchronizedFiles.xaml
    /// </summary>
    public partial class SynchronizedFilesWindow : Window
    {
        private readonly ObservableCollection<CloudFileWrapper> files;
        private bool isChanged = false;

        public SynchronizedFilesWindow()
        {
            InitializeComponent();

            files = new ObservableCollection<CloudFileWrapper>();

            ConfigIO.ReadSynchronizedFiles()
                .Select(x => new CloudFileWrapper(x))
                .ToList()
                .ForEach(d => files.Add(d));

            files.CollectionChanged += (sender, args) =>
            {
                isChanged = true;
            };

            DgFiles.ItemsSource = files;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            var cloudFiles = files.Select(x => x.GetCloudFile()).ToList();

            ConfigIO.WriteSynchronizedFiles(cloudFiles);

            MessageBox.Show("Guardado ✔", "Información", MessageBoxButton.OK, MessageBoxImage.Information);

            isChanged = false;
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
                FileDialog fd = new OpenFileDialog();
                fd.Title = "Selecciona un archivo";
                fd.InitialDirectory = System.IO.Path.GetDirectoryName((sender as Button)?.Content.ToString());

                if (!(fd.ShowDialog() ?? false)) return;

                var itemFromDataGrid = (CloudFileWrapper)DgFiles.SelectedItem;

                var index = files.IndexOf(itemFromDataGrid);

                itemFromDataGrid.LocalPath = fd.FileName;
                files[index] = itemFromDataGrid;

                DgFiles.CommitEdit();

                CollectionViewSource.GetDefaultView(DgFiles.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (isChanged)
            {
                var result = MessageBox.Show("Hay cambios pendientes de guardar. Salir sin guardar?", "Finalizar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }
    }
}