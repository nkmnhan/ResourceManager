using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Xml;
using XmlResource.Models;

namespace XmlResource.Services
{
    public static class ResxService
    {
        public static void Generate(string className, string folderPath, List<LanguageResource> languageResources)
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

        public static void UpdateResx(string className, string folderPath, List<LanguageResource> languageResources)
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

        public static async Task DifferentKeysReport(LanguageResource languageResource, string resxFilePath)
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
    }
}
