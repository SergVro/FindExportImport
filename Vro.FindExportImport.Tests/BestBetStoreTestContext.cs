using System.Net;
using System.Net.Http;
using EPiServer;
using EPiServer.Core;
using Moq;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Tests
{
    public class BestBetStoreTestContext
    {
        public Mock<IContent> MockContent { get; }
        public Mock<IBestBetControllerFactory> MockBestBetControllerFactory { get; }
        public Mock<IBestBetsController> MockController { get; }
        public Mock<IContentRepository> MockContentRepository { get; }
        public HttpResponseMessage ResponseMessage { get; }

        public BestBetStore BestBetStore { get; private set; }

        public BestBetStoreTestContext()
        {
            MockController = new Mock<IBestBetsController>();
            ResponseMessage = new HttpResponseMessage();
            MockBestBetControllerFactory = new Mock<IBestBetControllerFactory>();
            MockBestBetControllerFactory.Setup(f => f.CreateController()).Returns(MockController.Object);
            MockContentRepository = new Mock<IContentRepository>();
            MockContent = new Mock<IContent>();
            BestBetStore = new BestBetStore(MockBestBetControllerFactory.Object, MockContentRepository.Object);

        }
    }
}