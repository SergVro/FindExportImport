using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Vro.FindExportImport.Models
{
    public class EntitySet<T>
    {
        public string Key { get; set; }
        public List<T> Entities { get; set; }
    }
}