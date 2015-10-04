using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public class StoreFactory
    {
        public static IStore<T> GetStore<T>() where T: IOptimizationEntity
        {
            if (typeof(AutocompleteEntity) == typeof(T))
            {
                return new IndexStore<T>("_autocomplete");
            }
            if (typeof (RelatedQueryEntity) == typeof(T))
            {
                return new IndexStore<T>("_didyoumean");
            }
            if (typeof (SynonymEntity) == typeof(T))
            {
                var store = new IndexStore<T>("_admin/synonym");
                store.ListUrlTemplate = store.BaseUrl + "?from={0}&size={1}";
                return store;
            }
            if (typeof(BestBetEntity) == typeof(T))
            {
                return (IStore<T>) new BestBetStore();
            }
            return null;
        }
    }
}
