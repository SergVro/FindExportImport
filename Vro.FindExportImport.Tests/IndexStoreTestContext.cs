using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
