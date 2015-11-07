using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Import
{
    [ServiceConfiguration(typeof (IImporter))]
    public class BestBetImporter : ImporterBase<BestBetEntity>
    {
        public BestBetImporter(IStoreFactory storeFactory) : base(storeFactory.GetStore<BestBetEntity>())
        {
        }
    }
}