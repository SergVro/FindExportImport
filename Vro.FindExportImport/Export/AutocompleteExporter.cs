using EPiServer.ServiceLocation;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class AutocompleteExporter : BaseExporter
    {
        public AutocompleteExporter() : base(EntityType.Autocomplete, "_autocomplete//list?from={0}&size={1}")
        {
        }
    }
}