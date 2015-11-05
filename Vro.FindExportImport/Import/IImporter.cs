using System.Collections.Generic;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Import
{
    public interface IImporter
    {
        string EntityKey { get; set; }
        string Import(string siteId, List<IOptimizationEntity> entities);
    }
}