using System.Linq;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public interface ISearchService
    {
        ContentReference FindMatchingContent(BestBetEntity bestBetEntity);
    }

    [ServiceConfiguration(typeof(ISearchService))]
    public class SearchService : ISearchService
    {
        public ContentReference FindMatchingContent(BestBetEntity bestBetEntity)
        {
            var searchQuery = SearchClient.Instance
                .Search<IContent>()
                .Filter(x => x.Name.Match(bestBetEntity.TargetName));

            if (bestBetEntity.TargetType.Equals(Helpers.PageBestBetSelector))
            {
                searchQuery = searchQuery.Filter(x => !x.ContentLink.ProviderName.Exists());
            }
            else if (bestBetEntity.TargetType.Equals(Helpers.CommerceBestBetSelector))
            {
                searchQuery = searchQuery.Filter(x => x.ContentLink.ProviderName.Match("CatalogContent"));
            }

            var searchResults = searchQuery.Select(c => c.ContentLink).Take(1).GetResult();
            return searchResults.Hits.FirstOrDefault()?.Document;
        }
    }
}