using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class SynonymExporter : BaseExporter
    {
        public SynonymExporter() : base(EntityType.Synonym, "_admin/synonym?from={0}&size={1}")
        {
        }
    }
}
