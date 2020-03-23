
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;

namespace ResourceManager.ViewModels
{
    public class I18nViewModel : Screen
    {
        public string Name { get; set; }
        public I18nViewModel()
        {
            Name = "I18n";
        }

        public void SelectFolder(object source, RoutedEventArgs eventArgs)
        {
            var button = source as Button;
            //using (var fbd = new FolderBrowserDialog())
            //{
            //    DialogResult result = fbd.ShowDialog();

            //    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            //    {
            //        string[] files = Directory.GetFiles(fbd.SelectedPath);

            //        System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            //    }
            //}

            //using (var fbd = new OpenFileDialog())
            //{
            //    fbd.DefaultExt = ".xlsx";
            //    fbd.Filter = "Excel |*.xlsx";
            //    DialogResult result = fbd.ShowDialog();
            //}
        }
    }
}
