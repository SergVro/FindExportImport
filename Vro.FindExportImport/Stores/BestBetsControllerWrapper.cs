using System.Net.Http;
using System.Web.Http;
using EPiServer.Find.UI.Controllers;
using EPiServer.Find.UI.Models;

namespace Vro.FindExportImport.Stores
{
    public interface IBestBetsController
    {
        HttpResponseMessage Get(string id);
        HttpResponseMessage GetList(int from, int size, string tags);
        HttpResponseMessage Post(BestBetModel model);
        HttpResponseMessage Delete(string id);
    }

    public interface IBestBetControllerFactory
    {
        IBestBetsController CreateController();
    }

    public class BestBetControllerDefaultFactory : IBestBetControllerFactory
    {
        public IBestBetsController CreateController()
        {
            var bestBetsController = new BestBetsController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            return new BestBetsControllerWrapper(bestBetsController);
        }
    }

    public class BestBetsControllerWrapper : IBestBetsController
    {
        private readonly BestBetsController _wrappedObject;

        public BestBetsControllerWrapper(BestBetsController wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        public HttpResponseMessage Get(string id)
        {
            return _wrappedObject.Get(id);
        }

        public HttpResponseMessage GetList(int @from, int size, string tags)
        {
            return _wrappedObject.GetList(tags, from, size);
        }

        public HttpResponseMessage Post(BestBetModel model)
        {
            return _wrappedObject.Post(model);
        }

        public HttpResponseMessage Delete(string id)
        {
            return _wrappedObject.Delete(id);
        }
    }
}