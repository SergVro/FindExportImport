using System.IO;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Export
{
    public interface IExporter
    {
        string EntityKey { get; }
        void WriteToStream(JsonWriter writer);
    }
}