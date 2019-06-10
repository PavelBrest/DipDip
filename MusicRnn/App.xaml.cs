using MusicRnn.ViewModels;
using System.Windows;

namespace MusicRnn
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new MainWindow().ShowDialog();

            base.OnStartup(e);
        }
    }
}
