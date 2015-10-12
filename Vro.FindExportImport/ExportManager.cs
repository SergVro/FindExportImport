using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Export;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport
{
    public class ExportManager
    {
        public void ExportToStream(List<string> selectedExporters, string siteId, string language, Stream stream)
        {
            var exporters = GetExporters().Where(e => selectedExporters.Contains(e.EntityKey));

            using (var streamWriter = new StreamWriter(stream))
            {
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    writer.WriteStartArray();
                    foreach (var exporter in exporters)
                    {
                        exporter.WriteToStream(siteId, language, writer);
                    }
                    writer.WriteEndArray();
                }
            }
        }

        public IEnumerable<IExporter> GetExporters()
        {
            return ServiceLocator.Current.GetAllInstances<IExporter>();
        }

        public List<TagSelectionModel> GetSites()
        {
            var loader = new CmsSiteIdentityLoader();
            var sites = loader.SiteIdentites.Select(i => new TagSelectionModel{ Id = i.Id.ToString(), Name = i.Name}).ToList();
            sites.Insert(0, new TagSelectionModel {Id = loader.AllSitesId, Name ="All sites"} );
            return sites;
        }

        public List<TagSelectionModel> GetLanguages()
        {
            var languages = SearchClient.Instance.Settings.Languages.Select(l => new TagSelectionModel {Id = l.FieldSuffix, Name = l.Name}).ToList();
            languages.Insert(0, new TagSelectionModel { Id = Languages.AllLanguagesSuffix, Name="All languages"});
            return languages;
        }
    }
}
