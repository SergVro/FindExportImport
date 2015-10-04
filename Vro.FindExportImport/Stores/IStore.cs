using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Stores
{
    public interface IStore<T> where T : IOptimizationEntity
    {
        T Get(string id);
        ListResult<T> List(int from, int size);
        string Create(T entity);
        void Update(T entity);
        void Delete(string id);
    }
}