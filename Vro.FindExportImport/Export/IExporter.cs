using Newtonsoft.Json;

namespace Vro.FindExportImport.Export
{
    public interface IExporter
    {
        string EntityKey { get; }
        string UiUrl { get; }
        void WriteToStream(string siteId, string language, JsonWriter writer);
        int GetTotalCount(string siteId, string language);
        void DeleteAll(string siteId, string language);
        
    }
}