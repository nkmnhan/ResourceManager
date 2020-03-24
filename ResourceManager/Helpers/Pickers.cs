using System.Windows.Forms;

namespace ResourceManager.Helpers
{
    public static class Pickers
    {
        public static string FolderPicker()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    return folderBrowserDialog.SelectedPath;
                }
            }
            return string.Empty;
        }

        public static string FilePicker()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.DefaultExt = ".xlsx";
                openFileDialog.Filter = "Excel |*.xlsx";
                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
                {
                    return openFileDialog.FileName;
                }
            }
            return string.Empty;
        }
    }
}
