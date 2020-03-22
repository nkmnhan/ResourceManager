using Caliburn.Micro;

namespace ResourceManager.ViewModels
{
    public class ShellViewModel : Screen
    {
        public string Name { get; set; }
        public ShellViewModel()
        {
            Name = "123";
        }
    }
}
