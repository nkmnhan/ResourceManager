using System.Collections.Generic;

namespace XmlResource.Models
{
    public class LanguageResourceModel
    {
        public string LanguageName { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public LanguageResourceModel()
        {
            Values = new Dictionary<string, string>();
        }
    }
}
