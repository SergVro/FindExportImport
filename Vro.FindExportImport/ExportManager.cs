using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EPiServer.Find.Cms;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Export;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport
{
    public class ExportManager
    {
        public void ExportToStream(List<string> selectedExporters, string siteId, Stream stream)
        {
            var exporters = GetExporters().Where(e => selectedExporters.Contains(e.EntityKey));

            using (var streamWriter = new StreamWriter(stream))
            {
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    writer.WriteStartArray();
                    foreach (var exporter in exporters)
                    {
                        exporter.WriteToStream(siteId, writer);
                    }
                    writer.WriteEndArray();
                }
            }
        }

        public IEnumerable<IExporter> GetExporters()
        {
            return ServiceLocator.Current.GetAllInstances<IExporter>();
        }

        public List<SiteModel> GetSites()
        {
            var loader = new CmsSiteIdentityLoader();
            var sites = loader.SiteIdentites.Select(i => new SiteModel{ Id = i.Id.ToString(), Name = i.Name}).ToList();
            sites.Insert(0, new SiteModel {Id = loader.AllSitesId, Name ="All sites"} );
            return sites;
        }
    }
}
