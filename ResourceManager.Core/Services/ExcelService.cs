using OfficeOpenXml;
using ResourceManager.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ResourceManager.Core.Services
{
    public static class ExcelService
    {
        public static List<LanguageModel> Read(string path)
        {
            // If you are a commercial business and have
            // purchased commercial licenses use the static property
            // LicenseContext of the ExcelPackage class :
            ExcelPackage.LicenseContext = LicenseContext.Commercial;

            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                //Load the datatable and set the number formats...
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                var keyColIndex = GetKeyColIndex(worksheet);

                var languageModels = GetResources(worksheet, 2, keyColIndex);

                if (!languageModels.Any())
                {
                    throw new Exception("Cannot find any language");
                }

                return languageModels;
            }
        }

        public static void Export(ExcelModel excelData, string newFilePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.FirstOrDefault();
                if (workSheet == null)
                {
                    workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                }

                workSheet.Cells[1, 1].Value = "Key";
                var headerInfo = new Dictionary<int, string>();
                foreach (var column in excelData.Columns.Select((resourceInfo, index) => new { resourceInfo, index }))
                {
                    var headerColIndex = column.index + 2;
                    var languageName = column.resourceInfo.LanguageName;

                    workSheet.Cells[1, headerColIndex].Value = languageName;
                    headerInfo.Add(headerColIndex, languageName);
                }

                foreach (var resource in excelData.Keys.Select((value, index) => new { value, index }))
                {
                    var rowIndex = resource.index + 2;

                    workSheet.Cells[rowIndex, 1].Value = resource.value;

                    foreach (var header in headerInfo)
                    {
                        var valueByHeader = excelData.Columns
                                                     .FirstOrDefault(x => x.LanguageName.Equals(header.Value)).Values
                                                     .FirstOrDefault(x => x.Key.Equals(resource.value)).Value;

                        workSheet.Cells[rowIndex, header.Key].Value = valueByHeader;
                    }
                }

                FileInfo excelFile = new FileInfo(newFilePath);
                excel.SaveAs(excelFile);
            }
        }

        private static string GetMergedRangeAddress(this ExcelRange excelRange)
        {
            if (excelRange.Merge)
            {
                var idx = excelRange.Worksheet.GetMergeCellId(excelRange.Start.Row, excelRange.Start.Column);
                return excelRange.Worksheet.MergedCells[idx - 1]; //the array is 0-indexed but the mergeId is 1-indexed...
            }
            else
            {
                return excelRange.Address;
            }
        }

        private static int GetMergedRowRange(this ExcelRange excelRange)
        {
            var range = excelRange.GetMergedRangeAddress();
            var regex = new Regex(@"([a-zA-Z]+)(?<startIndex>[\d]+):([a-zA-Z]+)(?<endIndex>[\d]+)");
            var match = regex.Match(range);
            if (match.Groups.Count > 1)
            {
                var startIndex = int.Parse(match.Groups["startIndex"].Value);
                var endIndex = int.Parse(match.Groups["endIndex"].Value);

                return endIndex - startIndex;
            }
            return 0;
        }

        private static int GetKeyColIndex(ExcelWorksheet worksheet)
        {
            int colCount = worksheet.Dimension.End.Column;

            var keyCol = -1;
            for (int i = 1; i <= colCount; i++)
            {
                var cell = worksheet.Cells[1, i].Value?.ToString().ToString();
                if (!string.IsNullOrEmpty(cell) && cell.Equals("key", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    keyCol = i;
                    break;
                }
            }

            if (keyCol < 1)
            {
                throw new Exception("Cannot find any language");
            }

            return keyCol;
        }

        private static List<LanguageModel> GetResources(ExcelWorksheet worksheet, int startRowIndex, int keyColIndex)
        {
            int colCount = worksheet.Dimension.End.Column;
            int rowCount = worksheet.Dimension.End.Row;

            var languageModels = new List<LanguageModel>();
            for (int col = keyColIndex + 1; col <= colCount; col++)
            {
                var language = worksheet.Cells[1, col].Value?.ToString().Trim();

                if (string.IsNullOrEmpty(language))
                {
                    continue;
                }

                var languageResource = new LanguageModel
                {
                    Name = language
                };

                for (int row = startRowIndex; row <= rowCount; row++)
                {
                    var keyCell = worksheet.Cells[row, keyColIndex];

                    if (keyCell.Style.Font.Strike)
                    {
                        continue;
                    }

                    var languageKey = keyCell.Value?.ToString().ToString();
                    if (string.IsNullOrWhiteSpace(languageKey))
                    {
                        throw new Exception($"Error reading excel at ROW={row}, COLUMN={keyColIndex}, KEY is null or empty");
                    }

                    var languageValue = worksheet.Cells[row, col].Value?.ToString().Trim();

                    var rowRange = keyCell.GetMergedRowRange();
                    if (rowRange > 0)
                    {
                        for (int i = 1; i <= rowRange; i++)
                        {
                            languageValue = $"{languageValue}\n{worksheet.Cells[row + i, col].Value?.ToString().Trim()}";
                        }
                        row += rowRange;
                    }

                    if (!languageResource.Values.TryAdd(languageKey, languageValue))
                    {
                        throw new Exception($"Error reading excel at ROW={row}, COLUMN={col}, KEY = {languageKey}, VALUE = {languageValue}");
                    }
                }

                languageModels.Add(languageResource);
            }

            return languageModels;
        }
    }
}
