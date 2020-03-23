using System.Windows;

namespace ResourceManager.Views
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class ShellView
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonCloseMenu.Visibility = Visibility.Visible;
            this.ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonCloseMenu.Visibility = Visibility.Collapsed;
            this.ButtonOpenMenu.Visibility = Visibility.Visible;
        }
    }
}
