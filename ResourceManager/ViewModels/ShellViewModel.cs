using Caliburn.Micro;
using System.Windows.Controls;

namespace ResourceManager.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly SimpleContainer container;
        private INavigationService navigationService;
        public ShellViewModel(SimpleContainer container)
        {
            this.container = container;
        }
        public void RegisterFrame(Frame frame)
        {
            navigationService = new FrameAdapter(frame);

            container.Instance(navigationService);

            navigationService.NavigateToViewModel(typeof(I18nViewModel));
        }

        public void SelectionChanged(object source, SelectionChangedEventArgs eventArgs)
        {
            switch (((ListViewItem)((ListView)source).SelectedItem).Name)
            {
                case "I18nItem":
                    navigationService.NavigateToViewModel(typeof(I18nViewModel));
                    break;
                case "ResxItem":
                    navigationService.NavigateToViewModel(typeof(ResxViewModel));
                    break;
                default:
                    break;
            }

        }
    }
}
