using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.Connection;
using Moq;
using Vro.FindExportImport.Stores;
using System.Web.Mvc;
using HttpVerbs = EPiServer.Find.Connection.HttpVerbs;

namespace Vro.FindExportImport.Tests
{
    public class IndexStoreTestContext
    {
        public IStoreFactory StoreFactory { get; set; }
        public FindConfiguration TestConfiguration { get; set; }
        public IndexStoreTestContext()
        {
            TestConfiguration = new FindConfiguration("http://myfindurl", "myindex", null);
            StoreFactory = new StoreFactory(TestConfiguration);
        }

        public Mock<IJsonRequestFactory> GetMockRequestFactory(string response, Action<string, HttpVerbs, int?> callback)
        {
            var mockJsonRequest = new Mock<IJsonRequest>();
            var mockRequestFactory = new Mock<IJsonRequestFactory>();
            var setup = mockRequestFactory.Setup(f => f.CreateRequest(It.IsAny<string>(), It.IsAny<HttpVerbs>(), null))
                .Returns(mockJsonRequest.Object);

            if (callback != null)
            {
                setup.Callback(callback);
            }

            mockJsonRequest.Setup(r => r.GetResponse()).Returns(response);

            return mockRequestFactory;
        }

        public Mock<IJsonRequestFactory> GetErrorRequestFactory(Exception exception)
        {
            var mockJsonRequest = new Mock<IJsonRequest>();
            var mockRequestFactory = new Mock<IJsonRequestFactory>();
            mockRequestFactory.Setup(f => f.CreateRequest(It.IsAny<string>(), It.IsAny<HttpVerbs>(), null))
                .Returns(mockJsonRequest.Object);

            mockJsonRequest.Setup(r => r.GetResponse()).Callback(() =>
            {
                throw exception;
            });

            return mockRequestFactory;
        }

        public WebException GetWebException(string errorMessage, string errorMessageInResponse, HttpStatusCode statusCode)
        {
            var response = new Mock<HttpWebResponse>();

            var expected = "{'error':'"+errorMessageInResponse+"', 'status':'error'}";
            var expectedBytes = Encoding.UTF8.GetBytes(expected);
            var responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            response.Setup(r => r.GetResponseStream()).Returns(responseStream);
            response.SetupGet(r => r.StatusCode).Returns(statusCode);
            var webException = new WebException(errorMessage, null, WebExceptionStatus.UnknownError, response.Object);
            return webException;
        }
    }
}
