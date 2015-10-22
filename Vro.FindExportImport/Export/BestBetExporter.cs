using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof (IExporter))]
    public class BestBetExporter : ExporterBase<BestBetEntity>
    {
        public BestBetExporter() : base(StoreFactory.GetStore<BestBetEntity>())
        {
        }
    }
}