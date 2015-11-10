using EPiServer;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public interface IStoreFactory
    {
        IStore<T> GetStore<T>() where T : IOptimizationEntity;
    }

    [ServiceConfiguration(typeof(IStoreFactory))]
    public class StoreFactory : IStoreFactory
    {
        private readonly FindConfiguration _configuration;

        public StoreFactory(FindConfiguration configuration)
        {
            _configuration = configuration;
        }

        IStore<T> IStoreFactory.GetStore<T>()
        {
            if (typeof(AutocompleteEntity) == typeof(T))
            {
                return new IndexStore<T>("_autocomplete", _configuration);
            }
            if (typeof(RelatedQueryEntity) == typeof(T))
            {
                return new IndexStore<T>("_didyoumean", _configuration);
            }
            if (typeof(SynonymEntity) == typeof(T))
            {
                var store = new IndexStore<T>("_admin/synonym", _configuration);
                store.ListUrlTemplate = store.BaseUrl + "?from={0}&size={1}&tags=language:{3}";
                return store;
            }
            if (typeof(BestBetEntity) == typeof(T))
            {
                return (IStore<T>) ServiceLocator.Current.GetInstance<BestBetStore>();
            }
            return null;
        }
    }
}
