using System;
using System.Linq;
using EPiServer;
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
                searchQuery = searchQuery.Filter(x => x.MatchTypeHierarchy(typeof(PageData)));
            }
            else if (bestBetEntity.TargetType.Equals(Helpers.CommerceBestBetSelector))
            {
                // resolving type from string to avoid referencing Commerce assemblies
                var commerceCatalogEntryType =
                    Type.GetType("EPiServer.Commerce.Catalog.ContentTypes.EntryContentBase, EPiServer.Business.Commerce");
                searchQuery = searchQuery.Filter(x => x.MatchTypeHierarchy(commerceCatalogEntryType));
            }

            var searchResults = searchQuery.Select(c => c.ContentLink).Take(1).GetResult();
            return searchResults.Hits.FirstOrDefault()?.Document;
        }
    }
}