using System.Collections.Generic;

namespace Vro.FindExportImport.Models
{
    public interface IOptimizationEntity
    {
        string Id { get; set; }
        List<string> Tags { get; set; }
    }
}