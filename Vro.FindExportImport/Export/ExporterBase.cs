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
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Export
{
    public abstract class ExporterBase<T> : IExporter where T : IOptimizationEntity
    {
        public string Url { get; set; }
        public string EntityKey { get; set; }
        public int PageSize { get; set; }
        public IJsonRequestFactory RequestFactory { get; set; }

        protected ExporterBase(string entityListUrlTemplate)
        {
            EntityKey = typeof(T).Name;
            PageSize = 50;
            var config = Configuration.GetConfiguration();
            Url = $"{config.ServiceUrl}{config.DefaultIndex}/{entityListUrlTemplate}";
            var requestTimeout = Configuration.GetConfiguration().DefaultRequestTimeout;
            RequestFactory = new JsonRequestFactory(requestTimeout);
        }

        protected virtual string LoadPage(int from, int size)
        {
            var url = string.Format(Url, from, size);
            var request = RequestFactory.CreateRequest(url, HttpVerbs.Get, null);
            var responseBody = request.GetResponse();
            return responseBody;
        }

        public virtual void WriteToStream(JsonWriter writer)
        {
            var entitySet = new EntitySet<IOptimizationEntity>();
            entitySet.Key = EntityKey;
            entitySet.Entities = new List<IOptimizationEntity>();

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
                        var result = DeserializeResult(serializer, jsonReader);
                        entitySet.Entities.AddRange(result.Hits.Cast<IOptimizationEntity>());
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
                    if (responseStream != null)
                    {
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
                }
                throw new ServiceException(message, originalException);
            }

            serializer.Serialize(writer, entitySet);
        }

        protected virtual ListResult<T> DeserializeResult(JsonSerializer serializer, JsonTextReader jsonReader)
        {
            var result = serializer.Deserialize<ListResult<T>>(jsonReader);
            return result;
        }
    }
}
