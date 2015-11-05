using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class AutocompleteExporter : ExporterBase<AutocompleteEntity>
    {
        public AutocompleteExporter() : base(StoreFactory.GetStore<AutocompleteEntity>(), "#manage/optimization/autocomplete")
        {
        }
    }
}