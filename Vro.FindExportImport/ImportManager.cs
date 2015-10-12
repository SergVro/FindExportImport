using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.Json;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Import;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport
{
    public class ImportManager
    {
        public string ImportFromStream(string siteId, Stream inputStream)
        {
            var importers = GetImporters();
            var serializer = Serializer.CreateDefault();
            var result = "";
            using (var streamReader = new StreamReader(inputStream))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var importedData = serializer.Deserialize<IEnumerable<EntitySet<IOptimizationEntity>>>(jsonReader);
                    foreach (var importedEntities in importedData)
                    {
                        var importer = importers.FirstOrDefault(i => i.EntityKey.Equals(importedEntities.Key));
                        result += importer?.Import(siteId, importedEntities.Entities);
                    }
                }
            }
            return result;
        }

        public List<IImporter> GetImporters()
        {
            return ServiceLocator.Current.GetAllInstances<IImporter>().ToList();
        }

        public void Delete(List<string> entityKeys, string siteId, string language)
        {
            var importers = GetImporters().Where(i => entityKeys.Contains(i.EntityKey)).ToList();
            importers.ForEach(i => i.DeleteAll(siteId, language));
        }
    }
}
