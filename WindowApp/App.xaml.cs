using System.Windows;

namespace WindowApp;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /*private IContainer _container;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName)
            .AddJsonFile("configuration.json", false, true)
            .Build();

        var builder = new ContainerBuilder();
        builder.Register<IConfigurationRoot>(con => configuration).SingleInstance();

        _container = builder.Build();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        _container.Dispose();
    }*/
}