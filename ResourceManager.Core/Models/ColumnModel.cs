using System.Collections;
using System.Collections.Generic;

namespace ResourceManager.Core.Models
{
    public class ColumnModel
    {
        public string LanguageName { get; set; }
        public List<DictionaryEntry> Values { get; set; }
        public ColumnModel()
        {
            Values = new List<DictionaryEntry>();
        }
    }
}
