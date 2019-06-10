using MusicRnn.ViewModels;
using System.Windows;

namespace MusicRnn
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new ConfigWindow().ShowDialog();

            var vm = new MainVM();
            DataContext = vm;

            vm.OutputUpdate += (s, a) =>
            {
                console.Dispatcher.Invoke(() =>
                {
                    console.Text = (string)s;
                });
            };

            vm.PropertyChanged += delegate
            {
                listView.Dispatcher.Invoke(() =>
                {
                    listView.ItemsSource = vm.ResultFiles;
                });
            };
        }
    }
}
