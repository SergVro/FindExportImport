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
        protected ExporterBase(IStore<T> store)
        {
            Store = store;
            EntityKey = typeof (T).Name;
            PageSize = 20;
            DefaultSerializer = Serializer.CreateDefault();
        }

        public IStore<T> Store { get; set; }
        public string Url { get; set; }
        public int PageSize { get; set; }
        public JsonSerializer DefaultSerializer { get; set; }
        public string EntityKey { get; set; }

        public virtual void WriteToStream(string siteId, string language, JsonWriter writer)
        {
            if (Store == null)
            {
                throw new InvalidOperationException("Store property is not set");
            }

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
    }
}