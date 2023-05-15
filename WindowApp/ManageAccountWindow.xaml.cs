using Shared.Factory;
using Shared.models;
using Shared.util;
using System.Windows;

namespace WindowApp
{
    /// <summary>
    /// Lógica de interacción para ManageAccount.xaml
    /// </summary>
    public partial class ManageAccountWindow : Window
    {
        private CloudAccount _credentials;
        public ManageAccountWindow(CloudAccount credentials)
        {
            InitializeComponent();
            _credentials = credentials;

            tbAccessKey.Text = _credentials.AccessKey;
            tbSecretAccessKey.Text = _credentials.SecretAccessKey;
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btAccept_Click(object sender, RoutedEventArgs e)
        {
            _credentials.AccessKey = tbAccessKey.Text;
            _credentials.SecretAccessKey = tbSecretAccessKey.Text;

            var newCloudService = await new AwsServiceFactory()
                .CreateCloudService(_credentials.AccessKey, _credentials.SecretAccessKey);

            if (newCloudService == null)
            {
                MessageBox.Show("Credenciales incorrectos o carece de suficientes permisos", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ConfigIO.WriteAccountCredentials(_credentials, "Aws");

            MainWindow.Cloud = newCloudService;

            Close();
        }

        private async void btTest_Click(object sender, RoutedEventArgs e)
        {
            _credentials.AccessKey = tbAccessKey.Text;
            _credentials.SecretAccessKey = tbSecretAccessKey.Text;

            var newCloudService = await new AwsServiceFactory()
                .CreateCloudService(_credentials.AccessKey, _credentials.SecretAccessKey);

            if (newCloudService != null)
            {
                MessageBox.Show("Credenciales correctos",
                    "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Credenciales incorrectos o carece de suficientes permisos", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}