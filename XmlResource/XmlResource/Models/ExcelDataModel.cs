using System.Collections.Generic;

namespace XmlResource.Models
{
    public class ExcelDataModel
    {
        public List<string> Keys { get; set; }
        public List<LanguageColumnModel> LanguageColumns { get; set; }
        public ExcelDataModel()
        {
            LanguageColumns = new List<LanguageColumnModel>();
        }
    }
}
