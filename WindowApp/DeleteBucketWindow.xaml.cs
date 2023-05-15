using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
