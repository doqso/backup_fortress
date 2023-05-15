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
    /// Lógica de interacción para CreateBucketWindow.xaml
    /// </summary>
    public partial class CreateBucketWindow : Window
    {
        public CreateBucketWindow()
        {
            InitializeComponent();
            if (MainWindow.Cloud == null) CreateBucket.IsEnabled = false;
        }

        private async void CreateBucket_Click(object sender, RoutedEventArgs e)
        {
            var result = await MainWindow.Cloud?.CreateBucketAsync(BucketName.Text);

            if (!result.Equals(HttpStatusCode.OK))
            {
                MessageBox.Show("No se pudo crear el bucket", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Bucket creado correctamente", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
