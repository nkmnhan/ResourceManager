using System.Threading.Tasks;
using XmlResource.Services;

namespace XmlResource
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

            // Generate resx file from resource
            // ClassName is a part of resource file name: MySampleResource.resx => ClassName = MySample
            //var languageResourece = ExcelService.Read(@"D:\StudyRepos\SampleData\data.xlsx");
            //ResxService.Generate("ClassName", resxFolder, languageResourece);

            /*  // Update resx file base on excel
             var languageResourece = ExcelService.Read(@"D:\StudyRepos\SampleData\data.xlsx");
             ResxService.UpdateResx("ClassName", resxFolder, languageResourece);
             */

            /* // Get different key report
            var languageResourece = ExcelService.Read(@"D:\StudyRepos\SampleData\data.xlsx");
            await ResxService.DifferentKeysReport(languageResourece.FirstOrDefault(),
                @"D:\StudyRepos\SampleData\MyClassResource.resx");
            */

            var excelData = ResxService.ConvertExcelData(@"D:\StudyRepos\SampleData");
            ExcelService.ExportXmlResxToExcel(excelData, @"D:\test.xlsx");
        }
    }
}
