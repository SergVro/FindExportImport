using System;
using System.Net.Http;
using System.Web.Http;
using EPiServer.Find.UI.Controllers;
using EPiServer.Find.UI.Models;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Export;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Import
{
    [ServiceConfiguration(typeof(IImporter))]
    public class BestBetImporter : ImporterBase<BestBetEntity>
    {
        public BestBetImporter() : base(StoreFactory.GetStore<BestBetEntity>())
        {
        }
    }
}