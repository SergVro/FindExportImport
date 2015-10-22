using System.Collections.Generic;

namespace Vro.FindExportImport.Models
{
    public class EntitySet<T>
    {
        public string Key { get; set; }
        public List<T> Entities { get; set; }
    }
}