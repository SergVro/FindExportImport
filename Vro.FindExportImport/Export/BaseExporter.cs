using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EPiServer.Find;
using EPiServer.Find.Api;
using EPiServer.Find.Connection;
using EPiServer.Find.Helpers;
using EPiServer.Find.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vro.FindExportImport.Export
{
    public abstract class BaseExporter : IExporter
    {
        public string Url { get; set; }
        public string EntityKey { get; set; }
        public int PageSize { get; set; }
        public IJsonRequestFactory RequestFactory { get; set; }

        protected BaseExporter(string entityKey, string entityListUrlTemplate)
        {
            EntityKey = entityKey;
            PageSize = 50;
            var config = Configuration.GetConfiguration();
            Url = String.Format("{0}{1}/{2}", config.ServiceUrl, config.DefaultIndex, entityListUrlTemplate);
            var requestTimeout = Configuration.GetConfiguration().DefaultRequestTimeout;
            RequestFactory = new JsonRequestFactory(requestTimeout);
        }

        public string LoadPage(int from, int size)
        {
            var url =  String.Format(Url, from, size);
            var request = RequestFactory.CreateRequest(url, HttpVerbs.Get, null);
            var responseBody = request.GetResponse();
            return responseBody;
        }

        public void WriteToStream(JsonWriter writer)
        {
            var entitySet = new EntitySet();
            entitySet.Key = EntityKey;
            entitySet.Entities = new List<JObject>();

            var serializer = Serializer.CreateDefault();
            try
            {
                var total = 0;
                var page = 0;

                do
                {
                    var responseBody = LoadPage(page*PageSize, PageSize);
                    using (var reader = new StringReader(responseBody))
                    {
                        var jsonReader = new JsonTextReader(reader);
                        var result = serializer.Deserialize<ListResult>(jsonReader);
                        entitySet.Entities.AddRange(result.Hits);
                        total = result.Total;
                        page++;

                    }
                } while (page*PageSize < total);
            }
            catch (WebException originalException)
            {
                var message = originalException.Message;

                if (originalException.Response.IsNotNull())
                {
                    var responseStream = originalException.Response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    var response = streamReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(response))
                    {
                        try
                        {
                            response = JsonConvert.DeserializeObject<ServiceError>(response).Error;
                        }
                        catch (Exception)
                        {

                        }
                    }
                    message = message + Environment.NewLine + response;
                }
                throw new ServiceException(message, originalException);
            }

            serializer.Serialize(writer, entitySet);
        }

    }
}
