using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Import
{
    [ServiceConfiguration(typeof (IImporter))]
    public class AutocompleteImporter : ImporterBase<AutocompleteEntity>
    {
        public AutocompleteImporter(IStoreFactory storeFactory) : base(storeFactory.GetStore<AutocompleteEntity>())
        {
        }
    }
}