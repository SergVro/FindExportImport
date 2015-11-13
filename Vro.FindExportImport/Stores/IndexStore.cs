using System;
using System.IO;
using System.Net;
using System.Text;
using EPiServer.Find;
using EPiServer.Find.Api;
using EPiServer.Find.Connection;
using EPiServer.Find.Helpers;
using EPiServer.Find.Json;
using Newtonsoft.Json;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public class IndexStore<T> : IStore<T> where T : IOptimizationEntity
    {
        public string BaseUrl { get; set; }
        public string ListUrlTemplate { get; set; }
        public string GetUrlTemplate { get; set; }
        public string DeleteUrlTemplate { get; set; }
        public string CreateUrlTemplate { get; set; }

        public IJsonRequestFactory RequestFactory { get; set; }
        public JsonSerializer DefaultSerializer { get; set; }

        public IndexStore(string entityUrl, FindConfiguration config)
        {
            var serviceUrl = config.ServiceUrl.Trim();
            if (serviceUrl.EndsWith("/"))
            {
                serviceUrl = serviceUrl.Remove(serviceUrl.Length - 1);
            }
            var index = config.DefaultIndex;

            BaseUrl = $"{serviceUrl}/{index}/{entityUrl}";
            ListUrlTemplate = BaseUrl + "/list?from={0}&size={1}&tags=" + Helpers.SiteIdTag + "{2}," + Helpers.LanguageTag + "{3}";
            GetUrlTemplate = BaseUrl + "/{0}";
            DeleteUrlTemplate = BaseUrl + "/{0}";
            CreateUrlTemplate = BaseUrl;

            RequestFactory = new JsonRequestFactory(config.DefaultRequestTimeout);
            DefaultSerializer = Serializer.CreateDefault();

        }

        public string Create(T entity)
        {
            string result = null;
            var request = RequestFactory.CreateRequest(CreateUrlTemplate, HttpVerbs.Put, null);
            request.WriteBody(DefaultSerializer.Serialize(entity));

            try
            {
                var responseBody = request.GetResponse();
                result = GetIdFromResponse(responseBody);
            }
            catch (WebException originalException)
            {
                HandleException(originalException);
            }
            return result;
        }

        public T Get(string id)
        {
            T result = default(T);
            var url = string.Format(GetUrlTemplate, id);
            var request = RequestFactory.CreateRequest(url, HttpVerbs.Get, null);
            try
            {
                var responseBody = request.GetResponse();
                using (var reader = new StringReader(responseBody))
                {
                    var jsonReader = new JsonTextReader(reader);
                    result = DefaultSerializer.Deserialize<T>(jsonReader);
                }
            }
            catch (WebException webException)
            {
                if (webException.Response.IsNotNull())
                {
                    var statusCode = ((HttpWebResponse)webException.Response).StatusCode;
                    if (statusCode == HttpStatusCode.NotFound)
                    {
                        return default(T);
                    }
                    HandleException(webException);
                }
            }

            return result;

        }

        public ListResult<T> List(string siteId, string language, int @from, int size)
        {
            ListResult<T> result = null;
            var url = string.Format(ListUrlTemplate, from, size, siteId, language);
            var request = RequestFactory.CreateRequest(url, HttpVerbs.Get, null);
            try
            {
                var responseBody = request.GetResponse();
                using (var reader = new StringReader(responseBody))
                {
                    var jsonReader = new JsonTextReader(reader);
                    result = DefaultSerializer.Deserialize<ListResult<T>>(jsonReader);
                }
            }
            catch (WebException originalException)
            {
                HandleException(originalException);
            }

            return result;
        }

        public void Delete(string id)
        {
            var url = string.Format(DeleteUrlTemplate, id);
            var request = RequestFactory.CreateRequest(url, HttpVerbs.Delete, null);
            try
            {
                var responseBody = request.GetResponse();
                GetIdFromResponse(responseBody);
            }
            catch (WebException originalException)
            {
                HandleException(originalException);
            }
        }


        private static void HandleException(WebException originalException)
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
                            var errorResponse = JsonConvert.DeserializeObject<ServiceError>(response);
                            response = errorResponse?.Error;
                        }
                        catch (JsonException)
                        {
                            // ignored
                        }
                    }
                    message = message + Environment.NewLine + response;
                }
            }
            throw new ServiceException(message, originalException);
        }

        protected string GetIdFromResponse(string responseBody)
        {
            using (var reader = new StringReader(responseBody))
            {
                var jsonReader = new JsonTextReader(reader);
                var result = DefaultSerializer.Deserialize<StatusResponse>(jsonReader);
                if (result.Status == null || !result.Status.Equals("ok"))
                {
                    throw new ServiceException($"Error response: {result.Status ?? responseBody}");
                }
                return result.Id;
            }
        }
    }
}
