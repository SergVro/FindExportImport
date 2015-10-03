using System.Net.Http;
using System.Web;
using System.Web.Http;
using EPiServer.Find.UI.Controllers;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class BestBetExporter : ExporterBase<BestBetEntity>
    {
        public BestBetExporter() : base("")
        {
        }

        protected override string LoadPage(int @from, int size)
        {
            var bestBetsController = new BestBetsController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var listMessage = bestBetsController.GetList(from: from, size: size);
            var stringTask = listMessage.Content.ReadAsStringAsync();
            return stringTask.Result;
        }
    }
}