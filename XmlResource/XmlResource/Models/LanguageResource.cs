using System.Collections.Generic;

namespace XmlResource.Models
{
    public class LanguageResource
    {
        public string LanguageName { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public LanguageResource()
        {
            Values = new Dictionary<string, string>();
        }
    }
}
