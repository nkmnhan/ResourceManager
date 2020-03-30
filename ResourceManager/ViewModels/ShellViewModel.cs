using Caliburn.Micro;
using ResourceManager.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ResourceManager.ViewModels
{
    public class ShellViewModel : Screen, IHandle<StateMessage>
    {
        private DateTime startLoadingTime;
        private IEventAggregator events;
        private readonly SimpleContainer container;
        private INavigationService navigationService;

        private string header;
        private Visibility loading;

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                NotifyOfPropertyChange(() => Header);
            }
        }

        public Visibility Loading
        {
            get { return loading; }
            set
            {
                loading = value;
                NotifyOfPropertyChange(() => Loading);
            }
        }

        public ShellViewModel(SimpleContainer container, IEventAggregator events)
        {
            Loading = Visibility.Hidden;
            this.container = container;
            this.events = events;
            this.events.Subscribe(this);
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

        public void Handle(StateMessage message)
        {
            var period = DateTime.Now.Subtract(startLoadingTime).Milliseconds;
            if (message.Loading == Visibility.Hidden && period < 3000)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(3000 - int.Parse(period.ToString()));
                    Loading = message.Loading;
                });
            }
            else
            {
                startLoadingTime = DateTime.Now;
                Loading = message.Loading;
            }
        }
    }
}
