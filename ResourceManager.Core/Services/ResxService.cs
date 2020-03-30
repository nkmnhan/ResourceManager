using ResourceManager.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;

namespace ResourceManager.Core.Services
{
    public static class ResxService
    {
        public static void Create(string className, string folderPath, List<LanguageModel> languageResources)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new Exception("Cannot find destination folder to save");
            }

            foreach (var languageResource in languageResources)
            {
                var xmlPath = GetResxPath(className, folderPath, languageResource.Name);

                using (ResXResourceWriter resx = new ResXResourceWriter(xmlPath))
                {
                    foreach (var item in languageResource.Values)
                    {
                        resx.AddResource(item.Key, item.Value);
                    }
                }
            }
        }

        public static void Update(string className, string folderPath, List<LanguageModel> languageResources)
        {
            foreach (var languageResource in languageResources)
            {
                XmlDocument document = new XmlDocument();

                var xmlPath = GetResxPath(className, folderPath, languageResource.Name);

                if (!File.Exists(xmlPath))
                {
                    throw new Exception("Cannot find 'Resouce' file");
                }

                var resxItems = new List<DictionaryEntry>();
                using (var reader = new ResXResourceReader(xmlPath))
                {
                    resxItems = reader.Cast<DictionaryEntry>().ToList();
                }

                using (var writer = new ResXResourceWriter(xmlPath))
                {
                    foreach (var item in resxItems)
                    {
                        var value = item.Value;
                        if (languageResource.Values.Any(x => x.Key.Equals(item.Key.ToString())))
                        {
                            value = languageResource.Values.FirstOrDefault(x => x.Key.Equals(item.Key.ToString())).Value;
                        }
                        writer.AddResource(item.Key.ToString(), item.Value);
                    }
                    writer.Generate();
                }
            }
        }

        public static ExcelModel ConvertToExcelData(string resxFolder)
        {
            var keys = new List<string>();
            var columns = new List<ColumnModel>();
            var files = Directory.GetFiles(resxFolder, "*.resx");

            foreach (var file in files)
            {
                using (var reader = new ResXResourceReader(file))
                {
                    var resxItems = reader.Cast<DictionaryEntry>();
                    var languageName = GetLanguageName(file);

                    if (string.IsNullOrWhiteSpace(languageName))
                    {
                        languageName = "default";
                    }

                    keys.AddRange(resxItems.Select(x => x.Key.ToString()));
                    keys = keys.Distinct().ToList();

                    columns.Add(new ColumnModel()
                    {
                        LanguageName = languageName,
                        Values = resxItems.ToList()
                    });

                }
            }

            return new ExcelModel
            {
                Keys = keys,
                Columns = columns
            };
        }

        public static List<string> GetAllKeys(string updateResxFolder)
        {
            var rexsFiles = Directory.GetFiles(updateResxFolder, "*.resx");

            if (!rexsFiles.Any())
            {
                throw new Exception("Cannot not find any .resx file");
            }

            var keys = new List<string>();
            foreach (var file in rexsFiles)
            {
                using (var reader = new ResXResourceReader(file))
                {
                    var resxItems = reader.Cast<DictionaryEntry>().ToList();
                    keys.AddRange(resxItems.Select(x => x.Key.ToString()));
                    keys = keys.Distinct().ToList();
                }
            }
            return keys;
        }

        private static string GetResxPath(string className, string folderPath, string languageName)
        {
            var defaultLanguage = ConfigurationManager.AppSettings["DefaultLanguage"];
            var subfix = languageName.Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase)
                            || languageName.Equals("default", StringComparison.InvariantCultureIgnoreCase) ? "" : $".{ languageName.ToLower()}";
            return @$"{folderPath}\{className}Resource{subfix}.resx";
        }

        private static string GetLanguageName(string path)
        {
            var fileNameRegex = new Regex(@"(?<FileName>[\w.]+).resx");
            var languageRegex = new Regex(@"(?:[\w]*).(?<LanguageName>[\w-]*)");

            var fileName = fileNameRegex.Match(path).Groups["FileName"].Value;
            return languageRegex.Match(fileName).Groups["LanguageName"].Value;
        }
    }
}
