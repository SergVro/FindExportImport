using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using EPiServer;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Find.Json;
using EPiServer.Find.UI.Models;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public class BestBetStore : IStore<BestBetEntity>
    {
        private const string PageBestBetSelector = "PageBestBetSelector";
        private const string CommerceBestBetSelector = "CommerceBestBetSelector";
        private readonly IBestBetControllerFactory _bestBetControllerFactory;
        private readonly IContentRepository _contentRepository;

        public BestBetStore(IBestBetControllerFactory bestBetControllerFactory, IContentRepository contentRepository)
        {
            _bestBetControllerFactory = bestBetControllerFactory;
            DefaultSerializer = Serializer.CreateDefault();
            _contentRepository = contentRepository;
        }

        public JsonSerializer DefaultSerializer { get; set; }

        public BestBetEntity Get(string id)
        {
            var bestBetsController = _bestBetControllerFactory.CreateController();
            var responseMessage = bestBetsController.Get(id);
            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            var responseContentAsync = responseMessage.Content.ReadAsStringAsync();
            var response = responseContentAsync.Result;
            using (var reader = new StringReader(response))
            {
                var jsonReader = new JsonTextReader(reader);
                var bestBetItemModel = DefaultSerializer.Deserialize<BestBetItemModel>(jsonReader);
                if (bestBetItemModel == null)
                {
                    return null;
                }

                var bestBetModel = bestBetItemModel.Item;
                var bestBetEntity = ConvertToBestBetEntity(bestBetModel);
                return bestBetEntity;
            }
        }

        public ListResult<BestBetEntity> List(string siteId, string language, int from, int size)
        {
            var bestBetsController = _bestBetControllerFactory.CreateController();
            var responseMessage = bestBetsController.GetList(from, size, $"siteid:{siteId},language:{language}");
            var responseContentAsync = responseMessage.Content.ReadAsStringAsync();
            var response = responseContentAsync.Result;
            var listResult = new ListResult<BestBetEntity> {Hits = new List<BestBetEntity>()};
            using (var reader = new StringReader(response))
            {
                var jsonReader = new JsonTextReader(reader);
                var bestBetsModel = DefaultSerializer.Deserialize<BestBetsModel>(jsonReader);
                listResult.Status = bestBetsModel.Status;
                listResult.Total = bestBetsModel.Total;
                foreach (var model in bestBetsModel.Hits)
                {
                    var entity = ConvertToBestBetEntity(model);
                    listResult.Hits.Add(entity);
                }
                return listResult;
            }
        }

        public string Create(BestBetEntity entity)
        {
            var bestBetsController = _bestBetControllerFactory.CreateController();

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

        public void Delete(string id)
        {
            var bestBetsController = _bestBetControllerFactory.CreateController();
            bestBetsController.Delete(id);
        }

        private BestBetEntity ConvertToBestBetEntity(BestBetModel bestBetModel)
        {
            var bestBetEntity = new BestBetEntity
            {
                Id = bestBetModel.Id,
                Phrase = bestBetModel.Phrases,
                BestBetHasOwnStyle = bestBetModel.BestBetHasOwnStyle,
                BestBetTargetDescription = bestBetModel.BestBetTargetDescription,
                BestBetTargetTitle = bestBetModel.BestBetTargetTitle,
                Tags = bestBetModel.Tags.ToList(),
                TargetType = bestBetModel.TargetType,
                TargetKey = bestBetModel.TargetKey
            };
            if (IsContentBestBet(bestBetEntity))
            {
                var targetContentReference = ContentReference.Parse(bestBetEntity.TargetKey);
                var targetContent = _contentRepository.Get<IContent>(targetContentReference);
                if (targetContent != null)
                {
                    bestBetEntity.TargetName = targetContent.Name;
                }
            }
            return bestBetEntity;
        }

        private static bool IsContentBestBet(BestBetEntity entity)
        {
            return entity.TargetType == PageBestBetSelector || entity.TargetType == CommerceBestBetSelector;
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
                throw new ServiceException("Can't find Content with name: " + bestBetEntity.TargetName);
            }
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