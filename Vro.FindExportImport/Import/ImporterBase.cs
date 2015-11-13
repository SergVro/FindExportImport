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
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            Store = store;
            EntityKey = typeof (T).Name;
            DefaultSerializer = Serializer.CreateDefault();
        }

        public IStore<T> Store { get; }
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


        public void UpdateSiteId(string siteId, IOptimizationEntity entity)
        {
            var tags = entity.Tags;
            var siteIdTag = tags.FirstOrDefault(t => t.StartsWith(Helpers.SiteIdTag));
            if (siteIdTag == null)
            {
                return;
            }
            var siteIdTagIndex = tags.IndexOf(siteIdTag);
            var newSiteIdtag = Helpers.SiteIdTag + siteId;
            entity.Tags[siteIdTagIndex] = newSiteIdtag;
        }
    }
}