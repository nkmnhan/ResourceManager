
using Caliburn.Micro;
using ResourceManager.Core.Helpers;
using ResourceManager.Core.Services;
using ResourceManager.Helpers;
using ResourceManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace ResourceManager.ViewModels
{
    public class ResxViewModel : Screen, IHandle<StateMessage>
    {
        private string errorMsg;
        private IEventAggregator events;
        private ObservableCollection<string> keysNotInExcel;
        private ObservableCollection<string> keysNotInResx;

        // Generate
        private string generateSelectedExcel;
        private string generateSaveFolder;
        private string generateClassName;

        // Export
        private string exportSourceFolder;
        private string exportSaveFolder;
        private string exportExcelName;

        // Update
        private string updateSelectedExcel;
        private string updateResxFolder;
        private string updateClassName;

        public ResxViewModel(IEventAggregator events)
        {
            this.events = events;
            this.events.Subscribe(this);
            this.KeysNotInExcel = new ObservableCollection<string>();
            this.KeysNotInResx = new ObservableCollection<string>();
        }

        public string ErrorMsg
        {
            get { return errorMsg; }
            set
            {
                errorMsg = value;
                NotifyOfPropertyChange(() => ErrorMsg);
                ShowErrorMessage();
            }
        }

        public string GenerateSelectedExcel
        {
            get { return generateSelectedExcel; }
            set
            {
                generateSelectedExcel = value;
                NotifyOfPropertyChange(() => GenerateSelectedExcel);
            }
        }
        public string GenerateSaveFolder
        {
            get { return generateSaveFolder; }
            set
            {
                generateSaveFolder = value;
                NotifyOfPropertyChange(() => GenerateSaveFolder);
            }
        }
        public string GenerateClassName
        {
            get { return generateClassName; }
            set
            {
                generateClassName = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
                NotifyOfPropertyChange(() => GenerateClassName);
            }
        }

        public string ExportSourceFolder
        {
            get { return exportSourceFolder; }
            set
            {
                exportSourceFolder = value;
                NotifyOfPropertyChange(() => ExportSourceFolder);
            }
        }
        public string ExportSaveFolder
        {
            get { return exportSaveFolder; }
            set
            {
                exportSaveFolder = value;
                NotifyOfPropertyChange(() => ExportSaveFolder);
            }
        }
        public string ExportExcelName
        {
            get { return exportExcelName; }
            set
            {
                exportExcelName = value;
                NotifyOfPropertyChange(() => ExportExcelName);
            }
        }

        public string UpdateSelectedExcel
        {
            get { return updateSelectedExcel; }
            set
            {
                updateSelectedExcel = value;
                NotifyOfPropertyChange(() => UpdateSelectedExcel);
            }
        }
        public string UpdateResxFolder
        {
            get { return updateResxFolder; }
            set
            {
                updateResxFolder = value;
                NotifyOfPropertyChange(() => UpdateResxFolder);
            }
        }
        public ObservableCollection<string> KeysNotInExcel
        {
            get { return keysNotInExcel; }
            set
            {
                keysNotInExcel = value;
                NotifyOfPropertyChange(() => KeysNotInExcel);
            }
        }

        public ObservableCollection<string> KeysNotInResx
        {
            get { return keysNotInResx; }
            set
            {
                keysNotInResx = value;
                NotifyOfPropertyChange(() => KeysNotInResx);
            }
        }
        public string UpdateClassName
        {
            get { return updateClassName; }
            set
            {
                updateClassName = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
                NotifyOfPropertyChange(() => UpdateClassName);
            }
        }

        public void SelectFile(object source, RoutedEventArgs eventArgs)
        {
            var button = source as Button;
            if (button != null && !string.IsNullOrWhiteSpace(button?.Tag?.ToString()))
            {
                var propInfo = this.GetType().GetProperty(button.Tag.ToString());
                var filePath = Pickers.FilePicker();
                propInfo.SetValue(this, filePath);
            }

        }

        public void SelectFolder(object source, RoutedEventArgs eventArgs)
        {
            var button = source as Button;
            if (button != null && !string.IsNullOrWhiteSpace(button?.Tag?.ToString()))
            {
                var propInfo = this.GetType().GetProperty(button.Tag.ToString());
                var folder = Pickers.FolderPicker();
                propInfo.SetValue(this, folder);
            }

        }

        public void Generate(object source, RoutedEventArgs eventArgs)
        {
            if (!ValidateGenerate())
            {
                return;
            }

            try
            {
                SetLoading(Visibility.Visible);
                var languageResources = ExcelService.Read(GenerateSelectedExcel);
                ResxService.Create(GenerateClassName, GenerateSaveFolder, languageResources);
                SetLoading(Visibility.Hidden);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public void Export(object source, RoutedEventArgs eventArgs)
        {
            if (!ValidateExport())
            {
                return;
            }


            try
            {
                SetLoading(Visibility.Visible);
                var excelData = ResxService.ConvertToExcelData(exportSourceFolder);
                var excelPath = Path.Combine(ExportSaveFolder, $"{ExportExcelName}.xlsx");
                ExcelService.Export(excelData, excelPath);
                SetLoading(Visibility.Hidden);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }

        }

        public void Update(object source, RoutedEventArgs eventArgs)
        {
            if (!ValidateUpdate())
            {
                return;
            }

            try
            {
                SetLoading(Visibility.Visible);
                var languageResources = ExcelService.Read(UpdateSelectedExcel);
                ResxService.Update(UpdateClassName, UpdateResxFolder, languageResources);

                var resxKeys = ResxService.GetAllKeys(UpdateResxFolder);
                var excelKeys = new List<string>();
                foreach (var item in languageResources)
                {
                    excelKeys.AddRange(item.Values.Keys);
                    excelKeys = excelKeys.Distinct().ToList();
                }

                var (NotInA, NotInB) = CompareHelper.GetDifference(excelKeys, resxKeys);

                KeysNotInExcel = new ObservableCollection<string>(NotInA);
                KeysNotInResx = new ObservableCollection<string>(NotInB);

                SetLoading(Visibility.Hidden);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        private bool ValidateGenerate()
        {
            if (string.IsNullOrWhiteSpace(GenerateSelectedExcel))
            {
                ErrorMsg = "Invalid excel path";
                return false;
            }
            if (string.IsNullOrWhiteSpace(GenerateSaveFolder))
            {
                ErrorMsg = "Invalid save folder";
                return false;
            }
            return true;
        }

        private bool ValidateExport()
        {
            if (string.IsNullOrWhiteSpace(ExportSourceFolder))
            {
                ErrorMsg = "Invalid source folder";
                return false;
            }
            if (string.IsNullOrWhiteSpace(ExportSaveFolder))
            {
                ErrorMsg = "Invalid save folder";
                return false;
            }
            if (string.IsNullOrWhiteSpace(ExportExcelName))
            {
                ErrorMsg = "Empty excel name";
                return false;
            }
            return true;
        }

        private bool ValidateUpdate()
        {
            if (string.IsNullOrWhiteSpace(UpdateSelectedExcel))
            {
                ErrorMsg = "Invalid excel path";
                return false;
            }
            if (string.IsNullOrWhiteSpace(UpdateResxFolder))
            {
                ErrorMsg = "Invalid resx folder";
                return false;
            }
            return true;
        }

        private void ShowErrorMessage()
        {
            MessageBox.Show($"{errorMsg}", "Error", MessageBoxButton.OK);
        }

        private void ExceptionHandler(Exception exception)
        {
            ErrorMsg = string.IsNullOrWhiteSpace(exception?.InnerException?.Message) ? exception?.Message : exception?.InnerException?.Message;
            SetLoading(Visibility.Hidden);
        }

        public void Handle(StateMessage message)
        {
        }

        private void SetLoading(Visibility visibility)
        {
            events.PublishOnUIThread(new StateMessage { Loading = visibility });
        }
    }
}
