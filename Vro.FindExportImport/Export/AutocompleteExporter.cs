using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class AutocompleteExporter : ExporterBase<AutocompleteEntity>
    {
        public AutocompleteExporter() : base("_autocomplete/list?from={0}&size={1}")
        {
        }
    }
}