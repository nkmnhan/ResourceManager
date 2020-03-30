using Newtonsoft.Json.Linq;
using ResourceManager.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ResourceManager.Core.Services
{
    public static class I18nService
    {
        public static async Task Create(string savePath, List<LanguageModel> languages)
        {
            foreach (var item in languages)
            {
                var i18n = new JObject();
                foreach (var language in item.Values)
                {
                    var jObject = CreateJObj(language.Key, language.Value);
                    i18n.Merge(jObject, new JsonMergeSettings
                    {
                        // union array values together to avoid duplicates
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                }

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(savePath, $"{item.Name}.json")))
                {
                    await outputFile.WriteAsync(i18n.ToString());
                }
            }
        }

        public static async Task Update(string i18nFolder, List<LanguageModel> languageResources)
        {
            foreach (var languageResource in languageResources)
            {

                var jsonPath = Path.Combine(i18nFolder, $"{languageResource.Name.ToLower()}.json");

                if (!File.Exists(jsonPath))
                {
                    throw new Exception("Cannot find 'Resouce' json");
                }

                var jsonText = await File.ReadAllTextAsync(jsonPath);
                var jsonObject = JObject.Parse(jsonText);

                foreach (var language in languageResource.Values)
                {
                    var token = jsonObject.SelectToken(language.Key);
                    if (token != null)
                    {
                        token.Replace(language.Value);
                    }
                }

                await File.WriteAllTextAsync(jsonPath, jsonObject.ToString());
            }
        }

        public static async Task<ExcelModel> ConvertToExcelData(string i18nFolder)
        {
            var files = Directory.GetFiles(i18nFolder, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => s.EndsWith(".json"));

            var columns = new List<ColumnModel>();
            var keys = new List<string>();

            foreach (var file in files)
            {
                using (StreamReader reader = File.OpenText(file))
                {
                    var result = await reader.ReadToEndAsync();

                    var regex = new Regex(@"(?<fileName>[\w]*)\.json$");
                    var match = regex.Match(file);

                    var languageValues = GetLanguageValues(JToken.Parse(result));
                    var languageKeys = languageValues.Select(x => x.Key.ToString());

                    keys.AddRange(languageKeys);
                    keys = keys.Distinct().ToList();

                    columns.Add(new ColumnModel()
                    {
                        LanguageName = match.Groups["fileName"].Value,
                        Values = languageValues
                    });
                }
            }

            return new ExcelModel
            {
                Keys = keys,
                Columns = columns
            };

        }

        public static async Task<List<string>> GetAllKeys(string i18nFolder)
        {
            var files = Directory.GetFiles(i18nFolder, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => s.EndsWith(".json"));

            var keys = new List<string>();

            foreach (var file in files)
            {
                using (StreamReader reader = File.OpenText(file))
                {
                    var result = await reader.ReadToEndAsync();

                    var regex = new Regex(@"(?<fileName>[\w]*)\.json$");
                    var match = regex.Match(file);

                    var languageValues = GetLanguageValues(JToken.Parse(result));
                    var languageKeys = languageValues.Select(x => x.Key.ToString());

                    keys.AddRange(languageKeys);
                    keys = keys.Distinct().ToList();
                }
            }

            return keys;
        }

        private static JObject CreateJObj(string queryPath, string value)
        {
            if (queryPath.Contains("."))
            {
                var props = queryPath.Split('.');

                var result = new JObject();
                string newQueryPath = string.Join('.', props.Skip(1));
                result.Add(props[0], CreateJObj(newQueryPath, value));
                return result;
            }

            return new JObject(new JProperty(queryPath, value));
        }

        private static List<DictionaryEntry> GetLanguageValues(IEnumerable<JToken> tokens)
        {
            var result = new List<DictionaryEntry>();

            foreach (var token in tokens)
            {
                var queryPath = string.Empty;
                if (token.Type == JTokenType.Property)
                {
                    JProperty prop = (JProperty)token;
                    if (!prop.Value.HasValues)
                    {
                        queryPath = $"{queryPath}.{token.Path}".Remove(0, 1);
                        result.Add(new DictionaryEntry(queryPath.ToString(), prop.Value.ToString()));
                    }
                }
                if (token.HasValues)
                {
                    queryPath = $"{queryPath}.{token.Path}";
                    result.AddRange(GetLanguageValues(token.Children()));
                }
            }

            return result;
        }
    }
}
