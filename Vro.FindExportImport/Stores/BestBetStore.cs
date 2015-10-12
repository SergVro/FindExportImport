using System;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Json;
using EPiServer.Find.UI.Controllers;
using EPiServer.Find.UI.Models;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public class BestBetStore : IStore<BestBetEntity>
    {
        public BestBetStore()
        {
            DefaultSerializer = Serializer.CreateDefault();
        }

        public JsonSerializer DefaultSerializer { get; set; }

        public BestBetEntity Get(string id)
        {
            var bestBetsController = CreateBestBetsController();
            var responseMessage = bestBetsController.Get(id);
            var responseContentAsync = responseMessage.Content.ReadAsStringAsync();
            var response = responseContentAsync.Result;
            using (var reader = new StringReader(response))
            {
                var jsonReader = new JsonTextReader(reader);
                return DefaultSerializer.Deserialize<BestBetEntity>(jsonReader);
            }
        }

        public ListResult<BestBetEntity> List(string siteId, string language, int @from, int size)
        {
            var bestBetsController = CreateBestBetsController();
            var responseMessage = bestBetsController.GetList(from: from, size: size, 
                tags: $"siteid:{siteId},language:{language}");
            var responseContentAsync = responseMessage.Content.ReadAsStringAsync();
            var response = responseContentAsync.Result;
            using (var reader = new StringReader(response))
            {
                var jsonReader = new JsonTextReader(reader);
                return DefaultSerializer.Deserialize<ListResult<BestBetEntity>>(jsonReader);
            }
        }

        public string Create(BestBetEntity entity)
        {
            var bestBetsController = CreateBestBetsController();

            var bestBetModel = new BestBetModel
            {
                Id = entity.Id,
                Phrases = entity.Phrase,
                BestBetTargetTitle = entity.BestBetTargetTitle,
                BestBetTargetDescription = entity.BestBetTargetDescription,
                Tags = entity.Tags,
                TargetType = entity.TargetType,
                
                // TODO: this causes validation exception if there are best bets with duplicate phrases, but different contents
                TargetKey = ContentReference.RootPage.ToString(), //we have to replace entity.TargetKey here with some existing ContentReference

                BestBetHasOwnStyle = entity.BestBetHasOwnStyle
            };
            
            var responseMessage = bestBetsController.Post(bestBetModel);
            var responseContentAsync = responseMessage.Content.ReadAsStringAsync();
            var response = responseContentAsync.Result;
            return GetIdFromResponse(response);
        }

        public void Update(BestBetEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            var bestBetsController = CreateBestBetsController();
            bestBetsController.Delete(id);
        }

        private static BestBetsController CreateBestBetsController()
        {
            var bestBetsController = new BestBetsController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return bestBetsController;
        }

        protected string GetIdFromResponse(string responseBody)
        {
            using (var reader = new StringReader(responseBody))
            {
                var jsonReader = new JsonTextReader(reader);
                StatusResponse result;
                try
                {
                    result = DefaultSerializer.Deserialize<StatusResponse>(jsonReader);
                }
                catch (JsonSerializationException)
                {
                    result = new StatusResponse
                    {
                        Status = responseBody
                    };
                }
                if (result.Status == null || !result.Status.Equals("ok"))
                {
                    throw new ServiceException($"Error response: {result.Status ?? responseBody}");
                }
                return result.Id;
            }
        }
    }
}