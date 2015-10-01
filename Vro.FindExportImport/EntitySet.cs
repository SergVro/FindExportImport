using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Vro.FindExportImport
{
    public class EntitySet
    {
        public string Key { get; set; }
        public List<JObject> Entities { get; set; }
    }
}