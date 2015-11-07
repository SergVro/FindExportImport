using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof (IExporter))]
    public class SynonymExporter : ExporterBase<SynonymEntity>
    {
        public SynonymExporter(IStoreFactory storeFactory) : base(storeFactory.GetStore<SynonymEntity>(), "#manage/optimization/synonyms")
        {
        }
    }
}