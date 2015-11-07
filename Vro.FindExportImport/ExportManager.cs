using System.Collections.Generic;
using System.IO;
using System.Linq;
using EPiServer.Find;
using EPiServer.Find.Api;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Find.Framework.Statistics;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Export;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport
{
    public class ExportManager
    {
        private readonly List<IExporter> _exporters;
        private readonly ISiteIdentityLoader _siteIdentityLoader;
        private readonly Settings _settings;

        public ExportManager()
        {
            _exporters = ServiceLocator.Current.GetAllInstances<IExporter>().ToList();
            _siteIdentityLoader = new CmsSiteIdentityLoader();
            _settings = SearchClient.Instance.Settings;
        }

        public ExportManager(List<IExporter> exporters, ISiteIdentityLoader siteIdentityLoader, Settings clientSettings)
        {
            _exporters = exporters;
            _siteIdentityLoader = siteIdentityLoader;
            _settings = clientSettings;
        }

        public void ExportToStream(List<string> selectedExporters, string siteId, string language, Stream stream)
        {
            var exporters = _exporters.Where(e => selectedExporters.Contains(e.EntityKey));

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

        public List<IExporter> Exporters => _exporters;

        public List<TagSelectionModel> GetSites()
        {
            var sites = _siteIdentityLoader.SiteIdentites.Select(i => new TagSelectionModel{ Id = i.Id.ToString(), Name = i.Name}).ToList();
            sites.Insert(0, new TagSelectionModel {Id = _siteIdentityLoader.AllSitesId, Name ="All sites"} );
            return sites;
        }

        public List<TagSelectionModel> GetLanguages()
        {
            var languages = _settings.Languages.Select(l => new TagSelectionModel {Id = l.FieldSuffix, Name = l.Name}).ToList();
            languages.Insert(0, new TagSelectionModel { Id = Languages.AllLanguagesSuffix, Name="All languages"});
            return languages;
        }

        public void Delete(List<string> entityKeys, string siteId, string language)
        {
            var exporters = _exporters.Where(i => entityKeys.Contains(i.EntityKey)).ToList();
            exporters.ForEach(i => i.DeleteAll(siteId, language));
        }
    }
}
