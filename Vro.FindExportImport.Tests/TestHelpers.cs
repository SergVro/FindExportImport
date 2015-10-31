using System;
using EPiServer.Find.Connection;
using Moq;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Tests
{
    public class TestHelpers
    {
        public Mock<IJsonRequestFactory> GetMockRequestFactory(string response, Action<string, HttpVerbs, int?> callback)
        {
            StoreFactory.Config = new FindConfiguration
            {
                ServiceUrl = "http://myfindurl",
                DefaultIndex = "myindex"
            };
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
    }
}