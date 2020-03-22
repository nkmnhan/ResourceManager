using System.Collections.Generic;

namespace ResourceManager.Core.Models
{
    public class LanguageModel
    {
        public string Name { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public LanguageModel()
        {
            Values = new Dictionary<string, string>();
        }
    }
}
