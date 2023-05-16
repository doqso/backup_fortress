using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;

namespace WindowApp
{
    /// <summary>
    /// Lógica de interacción para DeleteBucketWindow.xaml
    /// </summary>
    public partial class DeleteBucketWindow : Window
    {
        public DeleteBucketWindow(IEnumerable<string> buckets)
        {
            InitializeComponent();

            if (MainWindow.Cloud == null) btDeleteBucket.IsEnabled = false;

            cbBuckets.ItemsSource = buckets;

            if (buckets.Any())
                cbBuckets.SelectedIndex = 0;
        }

        private async void btDeleteBucket_Click(object sender, RoutedEventArgs e)
        {
            var result = await MainWindow.Cloud.DeleteBucketAsync(cbBuckets.SelectedItem.ToString());

            if (result.Equals(HttpStatusCode.OK))
            {
                MessageBox.Show("Contenedor borrado", "Información",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }

            else MessageBox.Show("Error al borrar el contenedor",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
