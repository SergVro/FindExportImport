using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Vro.FindExportImport.Export
{
    public class ListResult
    {
        public int Total { get; set; }
        public string Status { get; set; }
        public List<JObject>  Hits { get; set; }
    }
}