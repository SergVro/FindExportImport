using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
using EPiServer.Find.Connection;
using EPiServer.Find.Json;
using Newtonsoft.Json.Linq;

namespace Vro.FindExportImport.Import
{
    public abstract class BaseImporter : IImporter
    {
        public string EntityKey { get; set; }
        public string Url { get; set; }

        public JsonRequestFactory RequestFactory { get; set; }

        protected BaseImporter(string entityKey, string entityPostUrl)
        {
            EntityKey = entityKey;

            var config = Configuration.GetConfiguration();
            Url = String.Format("{0}{1}/{2}", config.ServiceUrl, config.DefaultIndex, entityPostUrl);
            var requestTimeout = Configuration.GetConfiguration().DefaultRequestTimeout;
            RequestFactory = new JsonRequestFactory(requestTimeout);
        }

        public void Import(List<JObject> entities)
        {
            var serializer = Serializer.CreateDefault();
            foreach (var entity in entities)
            {
                var request = RequestFactory.CreateRequest(Url, HttpVerbs.Put, null);
                //serializer.Serialize()
                //var responseBody = request.GetResponse();
                //return responseBody;
            }
        }
    }
}
