using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XmlResource.Services;

namespace XmlResource
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var resxFolder = Path.Combine(Directory.GetCurrentDirectory(), "NewResources");

            if (!Directory.Exists(resxFolder))
            {
                Directory.CreateDirectory(resxFolder);
            }

            var languageResourece = ExecelService.Read(@"D:\StudyRepos\SampleData\data.xlsx");
            ResxService.Generate("ClassName", resxFolder, languageResourece);
            await ResxService.DifferentKeysReport(languageResourece.FirstOrDefault(),
                @"D:\StudyRepos\SampleData\MyClassResource.resx");
        }
    }
}
