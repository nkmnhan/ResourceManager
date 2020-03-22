using System.Windows;
using System.Windows.Controls;

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

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GridMain.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "LoginCardItem":
                    this.GridMain.Children.Add(this.LoginCardControl);
                    break;
                case "SftpLoginCardItem":
                    this.GridMain.Children.Add(this.SftpLoginCardControl);
                    break;
                case "ConfigItem":
                    this.GridMain.Children.Add(this.ConfigControl);
                    break;
                default:
                    break;
            }
        }
    }
}
