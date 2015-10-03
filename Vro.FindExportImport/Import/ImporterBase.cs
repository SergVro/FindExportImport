using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
using EPiServer.Find.Api;
using EPiServer.Find.Connection;
using EPiServer.Find.Helpers;
using EPiServer.Find.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Import
{
    public class StatusResponse
    {
        public string Status { get; set; }
        public string Id { get; set; }
    }

    public abstract class ImporterBase<T> : IImporter where T : IOptimizationEntity
    {
        public JsonSerializer ImportSerializer { get; }
        public string EntityKey { get; set; }
        public string Url { get; set; }

        public IJsonRequestFactory RequestFactory { get; set; }

        protected ImporterBase(string entityPostUrl)
        {
            EntityKey = typeof(T).Name;

            var config = Configuration.GetConfiguration();
            Url = $"{config.ServiceUrl}{config.DefaultIndex}/{entityPostUrl}";
            var requestTimeout = Configuration.GetConfiguration().DefaultRequestTimeout;
            RequestFactory = new JsonRequestFactory(requestTimeout);
            ImportSerializer = Serializer.CreateDefault();

        }

        public virtual string Import(List<IOptimizationEntity> entities)
        {
            var resultMessageString = "";
            foreach (var entity in entities)
            {
                try
                {
                    resultMessageString = CreateEntity((T)entity, resultMessageString);
                }
                catch (ServiceException ex)
                {
                    resultMessageString += $"Error importing {Helpers.GetEntityName(EntityKey)} with id {entity.Id}. {ex.Message}{Environment.NewLine}";
                }
            }

            return resultMessageString;
        }

        protected virtual string CreateEntity(T entity, string resultMessageString)
        {
            var request = RequestFactory.CreateRequest(Url, HttpVerbs.Put, null);
            request.WriteBody(ImportSerializer.Serialize(entity));

            try
            {
                var responseBody = request.GetResponse();
                resultMessageString = CheckResponse(responseBody, entity.Id, resultMessageString);
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
            return resultMessageString;
        }

        protected string CheckResponse(string responseBody, string entityId, string resultMessageString)
        {
            using (var reader = new StringReader(responseBody))
            {
                var jsonReader = new JsonTextReader(reader);
                var result = ImportSerializer.Deserialize<StatusResponse>(jsonReader);
                if (result.Status == null || !result.Status.Equals("ok"))
                {
                    resultMessageString += $"Error importing {Helpers.GetEntityName(EntityKey)} with ID {entityId}. Status {result.Status ?? responseBody}.{Environment.NewLine}";
                }
            }
            return resultMessageString;
        }
    }
}
