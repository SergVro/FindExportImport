using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Find;
using EPiServer.Find.Json;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Import
{
    public abstract class ImporterBase<T> : IImporter where T : IOptimizationEntity
    {
        protected ImporterBase(IStore<T> store)
        {
            Store = store;
            EntityKey = typeof (T).Name;
            DefaultSerializer = Serializer.CreateDefault();
        }

        public IStore<T> Store { get; set; }
        public JsonSerializer DefaultSerializer { get; }
        public string EntityKey { get; set; }

        public virtual string Import(string siteId, List<IOptimizationEntity> entities)
        {
            var resultMessageString = "";
            foreach (var entity in entities)
            {
                try
                {
                    UpdateSiteId(siteId, entity);
                    Store.Create((T) entity);
                }
                catch (ServiceException ex)
                {
                    resultMessageString +=
                        $"Error importing {Helpers.GetEntityName(EntityKey)} with id {entity.Id}. {ex.Message}{Environment.NewLine}";
                }
            }

            return resultMessageString;
        }

        public void DeleteAll(string siteId, string language)
        {
            int pageSize = 50;
            int total;
            do
            {
                var listToDelete = Store.List(siteId, language, 0, pageSize);
                foreach (var hit in listToDelete.Hits)
                {
                    Store.Delete(hit.Id);
                }
                total = listToDelete.Total;
            } while (total > 0);
        }

        public void UpdateSiteId(string siteId, IOptimizationEntity entity)
        {
            var tags = entity.Tags;
            var siteIdTag = tags.FirstOrDefault(t => t.StartsWith("siteid:"));
            if (siteIdTag == null)
            {
                return;
            }
            var siteIdTagIndex = tags.IndexOf(siteIdTag);
            var newSiteIdtag = "siteid:" + siteId;
            entity.Tags[siteIdTagIndex] = newSiteIdtag;
        }
    }
}