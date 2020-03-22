using System.Collections.Generic;

namespace ResourceManager.Core.Models
{
    public class ExcelModel
    {
        public List<string> Keys { get; set; }
        public List<ColumnModel> Columns { get; set; }
        public ExcelModel()
        {
            Columns = new List<ColumnModel>();
        }
    }
}
