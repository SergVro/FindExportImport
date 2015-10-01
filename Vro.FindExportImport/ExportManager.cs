using System.Collections.Generic;
using System.IO;
using System.Linq;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Export;

namespace Vro.FindExportImport
{
    public class ExportManager
    {
        public void ExportToStream(Stream stream, List<string> selectedExporters)
        {
            var exporters = GetExporters().Where(e => selectedExporters.Contains(e.EntityKey));

            using (var streamWriter = new StreamWriter(stream))
            {
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    writer.WriteStartArray();
                    foreach (var exporter in exporters)
                    {
                        exporter.WriteToStream(writer);
                    }
                    writer.WriteEndArray();
                }
            }
        }

        public IEnumerable<IExporter> GetExporters()
        {
            return ServiceLocator.Current.GetAllInstances<IExporter>();
        }
    }
}
