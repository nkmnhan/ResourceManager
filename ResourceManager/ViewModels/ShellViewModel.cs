using Caliburn.Micro;
using System.Windows.Controls;

namespace ResourceManager.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly SimpleContainer container;
        private INavigationService navigationService;
        private string header;

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                NotifyOfPropertyChange(() => header);
            }
        }

        public ShellViewModel(SimpleContainer container)
        {
            this.container = container;
        }
        public void RegisterFrame(Frame frame)
        {
            navigationService = new FrameAdapter(frame);

            container.Instance(navigationService);

            Header = "I18n";
            navigationService.NavigateToViewModel(typeof(I18nViewModel));
        }

        public void SelectionChanged(object source, SelectionChangedEventArgs eventArgs)
        {
            switch (((ListViewItem)((ListView)source).SelectedItem).Name)
            {
                case "I18nItem":
                    Header = "I18n";
                    navigationService.NavigateToViewModel(typeof(I18nViewModel));
                    break;
                case "ResxItem":
                    Header = "Resx";
                    navigationService.NavigateToViewModel(typeof(ResxViewModel));
                    break;
                default:
                    Header = "Home";
                    break;
            }

        }
    }
}
