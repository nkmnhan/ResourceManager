using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceManager.Core
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var resxFolder = Path.Combine(Directory.GetCurrentDirectory(), "NewResources");

            //if (!Directory.Exists(resxFolder))
            //{
            //    Directory.CreateDirectory(resxFolder);
            //}

            //// Generate resx file from resource
            //// ClassName is a part of resource file name: MySampleResource.resx => ClassName = MySample
            //var languageResourece = ExcelService.Read(@"D:\StudyRepos\SampleData\data.xlsx");
            //ResxService.Create("ClassName", resxFolder, languageResourece);

            //// Update resx file base on excel
            //var languageResourece2 = ExcelService.Read(@"D:\StudyRepos\SampleData\data.xlsx");
            //ResxService.Update("ClassName", resxFolder, languageResourece2);


            //// Get different key report
            //var languageResourece3 = ExcelService.Read(@"D:\StudyRepos\SampleData\data.xlsx");
            //await ResxService.DifferentKeysReport(languageResourece3.FirstOrDefault(),
            //    @"D:\StudyRepos\SampleData\MyClassResource.resx");

            //var languageResourece4 = ExcelService.Read(@"D:\StudyRepos\SampleData\data.xlsx");
            //await I18nService.Update(@"D:\StudyRepos\SampleData\", languageResourece4);


            //var excelData = await I18nService.ConvertToExcelData(@"D:\StudyRepos\SampleData");
            //ExcelService.Export(excelData, @"D:\StudyRepos\Dummy\123.xlsx");

            //var excelData2 = ResxService.ConvertToExcelData(@"D:\StudyRepos\SampleData");
            //ExcelService.Export(excelData2, @"D:\StudyRepos\Dummy\456.xlsx");

            var list1 = new List<int> { 1, 2, 3, 4, 5 };
            var list2 = new List<int> { 3, 4, 5, 6, 7, 9 };
            var a = Helpers.CompareHelper.GetDifference<int>(list1, list2);
        }
    }
}
