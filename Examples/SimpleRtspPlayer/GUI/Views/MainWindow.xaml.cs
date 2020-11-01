using System.Windows;

namespace SimpleRtspPlayer.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = (ViewModels.MainWindowViewModel)DataContext;
            //viewModel.OnStartButtonClick();
        }
        private void OnCBClickfalse(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Topmost = false;
        }
        private void OnCBClicktrue(object sender, RoutedEventArgs e)
        {

            ((MainWindow)Application.Current.MainWindow).Topmost = true;
        }
    }
}