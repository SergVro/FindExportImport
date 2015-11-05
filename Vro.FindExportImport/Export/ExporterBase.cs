using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Find.Json;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Export
{
    public abstract class ExporterBase<T> : IExporter where T : IOptimizationEntity
    {
        protected ExporterBase(IStore<T> store, string uiUrl)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            Store = store;
            UiUrl = uiUrl;
            EntityKey = typeof (T).Name;
            PageSize = 20;
            DefaultSerializer = Serializer.CreateDefault();
        }

        public IStore<T> Store { get; }
        public int PageSize { get; set; }
        public JsonSerializer DefaultSerializer { get; set; }
        public string EntityKey { get; set; }
        public string UiUrl { get; }

        public virtual void WriteToStream(string siteId, string language, JsonWriter writer)
        {
            var entitySet = new EntitySet<IOptimizationEntity>
            {
                Key = EntityKey,
                Entities = new List<IOptimizationEntity>()
            };

            var page = 0;
            int total;
            do
            {
                var result = Store.List(siteId, language, page*PageSize, PageSize);
                entitySet.Entities.AddRange(result.Hits.Cast<IOptimizationEntity>());
                total = result.Total;
                page++;
            } while (page*PageSize < total);

            DefaultSerializer.Serialize(writer, entitySet);
        }

        public int GetTotalCount(string siteId, string language)
        {
            var result = Store.List(siteId, language, 0, 0);
            return result.Total;
        }


        public void DeleteAll(string siteId, string language)
        {
            int total;
            do
            {
                var listToDelete = Store.List(siteId, language, 0, PageSize);
                foreach (var hit in listToDelete.Hits)
                {
                    Store.Delete(hit.Id);
                }
                total = listToDelete.Total;
            } while (total > 0);
        }
    }
}