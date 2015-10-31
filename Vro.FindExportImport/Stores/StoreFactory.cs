using EPiServer;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public class StoreFactory
    {
        private static FindConfiguration _configuration;

        public static FindConfiguration Config
        {
            get { return _configuration ?? (_configuration = new FindConfiguration(Configuration.GetConfiguration())); }
            set { _configuration = value; }
        }

        public static IStore<T> GetStore<T>() where T: IOptimizationEntity
        {
            if (typeof(AutocompleteEntity) == typeof(T))
            {
                return new IndexStore<T>("_autocomplete", Config);
            }
            if (typeof (RelatedQueryEntity) == typeof(T))
            {
                return new IndexStore<T>("_didyoumean", Config);
            }
            if (typeof (SynonymEntity) == typeof(T))
            {
                var store = new IndexStore<T>("_admin/synonym", Config);
                store.ListUrlTemplate = store.BaseUrl + "?from={0}&size={1}&tags=language:{3}";
                return store;
            }
            if (typeof(BestBetEntity) == typeof(T))
            {
                return (IStore<T>) new BestBetStore(new BestBetControllerDefaultFactory(), ServiceLocator.Current.GetInstance<IContentRepository>());
            }
            return null;
        }
    }
}
