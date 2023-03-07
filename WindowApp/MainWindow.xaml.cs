using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace WindowApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			using (FolderBrowserDialog fbd = new FolderBrowserDialog())
			{
				if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					webBrowser.Source = new Uri(fbd.SelectedPath);
				}
			}
		}

		private void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			if (laRuta != null)
				laRuta.Text = webBrowser.Source.LocalPath;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			webBrowser.Source = new Uri(@"C:\");
			laRuta.Text = webBrowser.Source.LocalPath;
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			if (webBrowser.CanGoBack) webBrowser.GoBack();
		}

		private void btnForward_Click(object sender, RoutedEventArgs e)
		{
			if (webBrowser.CanGoForward) webBrowser.GoForward();
		}

		private void btGoParent_Click(object sender, RoutedEventArgs e)
		{
			string newUrl = webBrowser.Source.AbsolutePath
				.Substring(0, webBrowser.Source.AbsolutePath.LastIndexOf("/"));
			webBrowser.Source = new Uri(newUrl + "/");
		}
	}
}