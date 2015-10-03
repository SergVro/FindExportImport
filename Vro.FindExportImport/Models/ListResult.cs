using System.Collections.Generic;

namespace Vro.FindExportImport.Models
{
    public class ListResult<T>
    {
        public int Total { get; set; }
        public string Status { get; set; }
        public List<T>  Hits { get; set; }
    }
}