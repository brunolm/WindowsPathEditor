using System.Windows;
using WindowsPathEditor.Properties;

namespace WindowsPathEditor
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            Closing += MainView_Closing;

            DataContext = new MainViewModel();
        }

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
