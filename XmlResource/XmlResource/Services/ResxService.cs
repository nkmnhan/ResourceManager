using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using XmlResource.Models;

namespace XmlResource.Services
{
    public static class ResxService
    {
        public static void Generate(string className, string folderPath, List<LanguageResourceModel> languageResources)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new Exception("Cannot find destination folder to save");
            }

            foreach (var langaugeResource in languageResources)
            {
                var xmlPath = GetResxPath(className, folderPath, langaugeResource.LanguageName);

                using (ResXResourceWriter resx = new ResXResourceWriter(xmlPath))
                {
                    foreach (var item in langaugeResource.Values)
                    {
                        resx.AddResource(item.Key, item.Value);
                    }
                }
            }
        }

        public static void UpdateResx(string className, string folderPath, List<LanguageResourceModel> languageResources)
        {
            foreach (var langaugeResource in languageResources)
            {
                XmlDocument document = new XmlDocument();

                var xmlPath = GetResxPath(className, folderPath, langaugeResource.LanguageName);

                if (!File.Exists(xmlPath))
                {
                    throw new Exception("Cannot find 'Resouce' file");
                }

                document.Load(xmlPath);

                foreach (var item in langaugeResource.Values)
                {
                    var node = document.SelectSingleNode($"/root/data[@name='{item.Key}']/value");
                    if (node != null)
                    {
                        node.InnerText = item.Value;
                    }
                }
                document.Save(xmlPath);
            }
        }

        public static string GetResxPath(string className, string folderPath, string languageName)
        {
            var defaultLanguage = ConfigurationManager.AppSettings["DefaultLanguage"];
            var subfix = languageName.Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase)
                            || languageName.Equals("default", StringComparison.InvariantCultureIgnoreCase) ? "" : $".{ languageName.ToLower()}";
            return @$"{folderPath}\{className}Resource{subfix}.resx";
        }

        public static async Task DifferentKeysReport(LanguageResourceModel languageResource, string resxFilePath)
        {
            using (var reader = new ResXResourceReader(resxFilePath))
            {
                var resxItems = reader.Cast<DictionaryEntry>().ToList();
                var keysNotInExcel = resxItems.Where(x => !languageResource.Values.ContainsKey(x.Key.ToString())).Select(x => x.Key.ToString());
                var keysNotInResxFile = languageResource.Values.Where(x => !resxItems.Any(t => t.Key.ToString().Equals(x.Key, StringComparison.InvariantCultureIgnoreCase))).Select(x => x.Key);


                using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "DifferentKeysReport.txt")))
                {
                    var report = $"Not in execel file:\n\t{string.Join("\n\t", keysNotInExcel)}\n\nNot in resx file:\n\t{string.Join("\n\t", keysNotInResxFile)}";
                    await outputFile.WriteAsync(report);
                }
            }
        }

        public static ExcelDataModel ConvertExcelData(string resxFolder)
        {
            var files = Directory.GetFiles(resxFolder).Where(x => x.EndsWith(".resx"));

            var excelData = new ExcelDataModel();
            var defaultLanguage = ConfigurationManager.AppSettings["DefaultLanguage"];

            foreach (var file in files)
            {
                using (var reader = new ResXResourceReader(file))
                {
                    var resxItems = reader.Cast<DictionaryEntry>();
                    var languageName = GetLanguageName(file);

                    if (string.IsNullOrWhiteSpace(languageName) || languageName.Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase))
                    {
                        excelData.Keys = resxItems.Select(x => x.Key.ToString()).ToList();
                        excelData.LanguageColumns.Insert(0, new LanguageColumnModel()
                        {
                            LanguageName = languageName.Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase) ? "default" : defaultLanguage,
                            Values = resxItems.ToList()
                        });
                    }
                    else
                    {
                        excelData.LanguageColumns.Add(new LanguageColumnModel()
                        {
                            LanguageName = languageName,
                            Values = resxItems.ToList()
                        });
                    }

                }
            }

            return excelData;
        }

        public static string GetLanguageName(string path)
        {

            var fileNameRegex = new Regex(@"(?<FileName>[\w.]+).resx");
            var languageRegex = new Regex(@"(?:[\w]*).(?<LanguageName>[\w-]*)");

            var fileName = fileNameRegex.Match(path).Groups["FileName"].Value;
            return languageRegex.Match(fileName).Groups["LanguageName"].Value;
        }
    }
}
