using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Import
{
    [ServiceConfiguration(typeof (IImporter))]
    public class RelatedQueryImporter : ImporterBase<RelatedQueryEntity>
    {
        public RelatedQueryImporter(IStoreFactory storeFactory) : base(storeFactory.GetStore<RelatedQueryEntity>())
        {
        }
    }
}