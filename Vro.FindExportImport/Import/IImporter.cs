using System.Collections.Generic;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Import
{
    public interface IImporter
    {
        string EntityKey { get; set; }
        string Import(List<IOptimizationEntity> entities);
        void DeleteAll();
    }
}