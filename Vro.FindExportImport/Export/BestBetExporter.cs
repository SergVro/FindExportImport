using System.Net.Http;
using System.Web;
using System.Web.Http;
using EPiServer.Find.UI.Controllers;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class BestBetExporter : ExporterBase<BestBetEntity>
    {
        public BestBetExporter() : base(StoreFactory.GetStore<BestBetEntity>())
        {
        }
    }
}