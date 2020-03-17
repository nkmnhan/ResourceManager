using System.Collections;
using System.Collections.Generic;

namespace XmlResource.Models
{
    public class LanguageColumnModel
    {
        public string LanguageName { get; set; }
        public List<DictionaryEntry> Values { get; set; }
        public LanguageColumnModel()
        {
            Values = new List<DictionaryEntry>();
        }
    }
}
