
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
    public class I18nViewModel : Screen, IHandle<StateMessage>
    {
        private string errorMsg;
        private IEventAggregator events;
        private ObservableCollection<string> keysNotInExcel;
        private ObservableCollection<string> keysNotInI18n;

        // Generate
        private string generateSelectedExcel;
        private string generateSaveFolder;

        // Export
        private string exportSourceFolder;
        private string exportSaveFolder;
        private string exportExcelName;

        // Update
        private string updateSelectedExcel;
        private string updateI18nFolder;

        public I18nViewModel(IEventAggregator events)
        {
            this.events = events;
            this.events.Subscribe(this);
            this.KeysNotInExcel = new ObservableCollection<string>();
            this.KeysNotInI18n = new ObservableCollection<string>();
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
        public string UpdateI18nFolder
        {
            get { return updateI18nFolder; }
            set
            {
                updateI18nFolder = value;
                NotifyOfPropertyChange(() => UpdateI18nFolder);
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

        public ObservableCollection<string> KeysNotInI18n
        {
            get { return keysNotInI18n; }
            set
            {
                keysNotInI18n = value;
                NotifyOfPropertyChange(() => KeysNotInI18n);
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

        public async void Generate(object source, RoutedEventArgs eventArgs)
        {
            if (!ValidateGenerate())
            {
                return;
            }

            try
            {
                SetLoading(Visibility.Visible);
                var languageResources = ExcelService.Read(GenerateSelectedExcel);
                await I18nService.Create(languageResources, GenerateSaveFolder);
                SetLoading(Visibility.Hidden);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public async void Export(object source, RoutedEventArgs eventArgs)
        {
            if (!ValidateExport())
            {
                return;
            }


            try
            {
                SetLoading(Visibility.Visible);
                var excelData = await I18nService.ConvertToExcelData(exportSourceFolder);
                var excelPath = Path.Combine(ExportSaveFolder, $"{ExportExcelName}.xlsx");
                ExcelService.Export(excelData, excelPath);
                SetLoading(Visibility.Hidden);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }

        }

        public async void Update(object source, RoutedEventArgs eventArgs)
        {
            if (!ValidateUpdate())
            {
                return;
            }

            try
            {
                SetLoading(Visibility.Visible);
                var languageResources = ExcelService.Read(UpdateSelectedExcel);
                await I18nService.Update(UpdateI18nFolder, languageResources);

                var i18nKeys = await I18nService.GetAllKeys(UpdateI18nFolder);
                var excelKeys = new List<string>();
                foreach (var item in languageResources)
                {
                    excelKeys.AddRange(item.Values.Keys);
                    excelKeys = excelKeys.Distinct().ToList();
                }

                var compareResult = CompareHelper.GetDifference<string>(excelKeys, i18nKeys);

                KeysNotInExcel = new ObservableCollection<string>(compareResult.NotInA);
                KeysNotInI18n = new ObservableCollection<string>(compareResult.NotInB);

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
            if (string.IsNullOrWhiteSpace(UpdateI18nFolder))
            {
                ErrorMsg = "Invalid i18n folder";
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
