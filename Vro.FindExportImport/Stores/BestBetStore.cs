using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using EPiServer;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Find.Json;
using EPiServer.Find.UI.Controllers;
using EPiServer.Find.UI.Models;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public class BestBetStore : IStore<BestBetEntity>
    {
        private const string PageBestBetSelector = "PageBestBetSelector";
        private const string CommerceBestBetSelector = "CommerceBestBetSelector";
        public JsonSerializer DefaultSerializer { get; set; }
        private readonly IContentRepository _contentRepository;

        public BestBetStore()
        {
            DefaultSerializer = Serializer.CreateDefault();
            _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        }

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
                var listResult = DefaultSerializer.Deserialize<ListResult<BestBetEntity>>(jsonReader);
                foreach (var entity in listResult.Hits)
                {
                    if (IsContentBestBet(entity))
                    {
                        var targetContentReference = ContentReference.Parse(entity.TargetKey);
                        var targetContent = _contentRepository.Get<IContent>(targetContentReference);
                        entity.TargetName = targetContent.Name;
                    }
                }
                return listResult;
            }
        }

        private static bool IsContentBestBet(BestBetEntity entity)
        {
            return entity.TargetType == PageBestBetSelector || entity.TargetType == CommerceBestBetSelector;
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
                TargetKey = entity.TargetKey,
                BestBetHasOwnStyle = entity.BestBetHasOwnStyle
            };
            if (IsContentBestBet(entity))
            {
                ConvertTarget(entity, bestBetModel);
            }

            var responseMessage = bestBetsController.Post(bestBetModel);
            var responseContentAsync = responseMessage.Content.ReadAsStringAsync();
            var response = responseContentAsync.Result;
            return GetIdFromResponse(response);
        }

        protected virtual void ConvertTarget(BestBetEntity bestBetEntity, BestBetModel bestBetModel)
        {
            var searchQuery = SearchClient.Instance
                .Search<IContent>()
                .Filter(x => x.Name.Match(bestBetEntity.TargetName));

            if (bestBetEntity.TargetType.Equals(PageBestBetSelector))
            {
                searchQuery = searchQuery.Filter(x => !x.ContentLink.ProviderName.Exists());
            }
            else if (bestBetEntity.TargetType.Equals(CommerceBestBetSelector))
            {
                searchQuery = searchQuery.Filter(x => x.ContentLink.ProviderName.Match("CatalogContent"));
            }

            var searchResults = searchQuery.Select(c => c.ContentLink).Take(1).GetResult();
            var contentReference = searchResults.Hits.FirstOrDefault()?.Document;
            bestBetModel.TargetKey = contentReference?.ToString();
            if (bestBetModel.TargetKey == null)
            {
                throw new ServiceException("Can't find Content with name: "+bestBetEntity.TargetName);
            }
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