using System;
using Newtonsoft.Json;

namespace Vro.FindExportImport.Export
{
    public interface IExporter
    {
        string EntityKey { get; }
        void WriteToStream(string siteId, JsonWriter writer);
    }
}