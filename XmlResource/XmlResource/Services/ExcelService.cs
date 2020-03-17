using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using XmlResource.Models;

namespace XmlResource.Services
{
    public static class ExcelService
    {
        public static List<LanguageResourceModel> Read(string path)
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
                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                int rowCount = worksheet.Dimension.End.Row;     //get row count

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
                    throw new System.Exception("Cannot find any language");
                }

                var languageModels = new List<LanguageResourceModel>();
                for (int col = keyCol + 1; col <= colCount; col++)
                {
                    var language = worksheet.Cells[1, col].Value?.ToString().Trim();

                    if (string.IsNullOrEmpty(language))
                    {
                        continue;
                    }


                    var languageResource = new LanguageResourceModel
                    {
                        LanguageName = language
                    };

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var rowRange = worksheet.Cells[row, keyCol].GetMergedRowRange();

                        var languageKey = worksheet.Cells[row, keyCol].Value?.ToString().ToString();

                        var languageValue = worksheet.Cells[row, col].Value?.ToString().Trim();

                        if (rowRange > 0)
                        {
                            for (int i = 1; i <= rowRange; i++)
                            {
                                languageValue = $"{languageValue}\n{worksheet.Cells[row + i, col].Value?.ToString().Trim()}";
                            }
                            row += rowRange;
                        }

                        languageResource.Values.Add(languageKey, languageValue);

                    }

                    languageModels.Add(languageResource);
                }

                if (!languageModels.Any())
                {
                    throw new System.Exception("Cannot find any language");
                }

                return languageModels;
            }
        }

        public static string GetMergedRangeAddress(this ExcelRange @this)
        {
            if (@this.Merge)
            {
                var idx = @this.Worksheet.GetMergeCellId(@this.Start.Row, @this.Start.Column);
                return @this.Worksheet.MergedCells[idx - 1]; //the array is 0-indexed but the mergeId is 1-indexed...
            }
            else
            {
                return @this.Address;
            }
        }

        public static int GetMergedRowRange(this ExcelRange @this)
        {
            var range = @this.GetMergedRangeAddress();
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

        public static void ExportXmlResxToExcel(ExcelDataModel excelData, string newFilePath)
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
                foreach (var column in excelData.LanguageColumns.Select((resourceInfo, index) => new { resourceInfo, index }))
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
                        var valueByHeader = excelData.LanguageColumns
                                                     .FirstOrDefault(x => x.LanguageName.Equals(header.Value)).Values
                                                     .FirstOrDefault(x => x.Key.Equals(resource.value)).Value;

                        workSheet.Cells[rowIndex, header.Key].Value = valueByHeader;
                    }
                }

                FileInfo excelFile = new FileInfo(newFilePath);
                excel.SaveAs(excelFile);
            }
        }
    }
}
