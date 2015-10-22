using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public interface IStore<T> where T : IOptimizationEntity
    {
        T Get(string id);
        ListResult<T> List(string siteId, string language, int @from, int size);
        string Create(T entity);
        void Delete(string id);
    }
}