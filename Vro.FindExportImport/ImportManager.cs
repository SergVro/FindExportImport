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

namespace Vro.FindExportImport
{
    public class ImportManager
    {
        public string ImportFromStream(Stream inputStream)
        {
            var importers = ServiceLocator.Current.GetAllInstances<IImporter>().ToList();
            var serializer = Serializer.CreateDefault();
            var result = "";
            using (var streamReader = new StreamReader(inputStream))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var importedData = serializer.Deserialize<IEnumerable<EntitySet>>(jsonReader);
                    foreach (var importedEntities in importedData)
                    {
                        var importer = importers.FirstOrDefault(i => i.EntityKey.Equals(importedEntities.Key));
                        if (importer != null)
                        {
                            importer.Import(importedEntities.Entities);
                        }
                    }
                }
            }
            return result;
        }
    }
}
