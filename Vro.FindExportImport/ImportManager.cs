using System.Collections.Generic;
using System.IO;
using System.Linq;
using EPiServer.Find.Json;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Vro.FindExportImport.Import;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport
{
    public class ImportManager
    {
        private readonly List<IImporter> _importers;

        public ImportManager()
        {
            _importers = ServiceLocator.Current.GetAllInstances<IImporter>().ToList();
        }

        public ImportManager(List<IImporter> importers)
        {
            _importers = importers;
        }

        public string ImportFromStream(string siteId, Stream inputStream)
        {
            var serializer = Serializer.CreateDefault();
            var result = "";
            using (var streamReader = new StreamReader(inputStream))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var importedData = serializer.Deserialize<IEnumerable<EntitySet<IOptimizationEntity>>>(jsonReader);
                    foreach (var importedEntities in importedData)
                    {
                        var importer = _importers.FirstOrDefault(i => i.EntityKey.Equals(importedEntities.Key));
                        result += importer?.Import(siteId, importedEntities.Entities);
                    }
                }
            }
            return result;
        }

        public List<IImporter> GetImporters()
        {
            return _importers;
        }

      
    }
}
