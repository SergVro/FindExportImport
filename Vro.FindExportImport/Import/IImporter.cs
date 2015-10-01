using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Vro.FindExportImport.Import
{
    interface IImporter
    {
        string EntityKey { get; set; }
        void Import(List<JObject> entities);
    }
}
