using System;
using System.Net.Http;
using System.Web.Http;
using EPiServer.Find.UI.Controllers;
using EPiServer.Find.UI.Models;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Export;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Import
{
    [ServiceConfiguration(typeof(IImporter))]
    public class BestBetImporter : ImporterBase<BestBetEntity>
    {
        public BestBetImporter() : base("")
        {
        }

        protected override string CreateEntity(BestBetEntity entity, string resultMessageString)
        {
            var bestBetsController = new BestBetsController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var bestBetModel = new BestBetModel
            {
                Id = entity.Id,
                Phrases = entity.Phrase,
                BestBetTargetTitle = entity.BestBetTargetTitle,
                BestBetTargetDescription = entity.BestBetTargetDescription,
                Tags = entity.Tags,
                TargetType = entity.TargetType,
                TargetKey = entity.TargetKey,
                BestBetHasOwnStyle = entity.BestBetHasOwnStyle
            };
            var postMessage = bestBetsController.Post(bestBetModel);
            var stringTask = postMessage.Content.ReadAsStringAsync();
            resultMessageString = CheckResponse(stringTask.Result, entity.Id, resultMessageString);
            return resultMessageString;
        }
    }
}