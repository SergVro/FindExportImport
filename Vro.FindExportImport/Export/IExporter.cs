using System.IO;
using Newtonsoft.Json;

namespace Vro.FindExportImport.Export
{
    public interface IExporter
    {
        string EntityKey { get; set; }
        void WriteToStream(JsonWriter writer);
    }
}