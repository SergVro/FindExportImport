using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vro.FindExportImport.Models
{
    public interface IOptimizationEntity
    {
        string Id { get; set; }
        List<string> Tags { get; set; }
    }
}
