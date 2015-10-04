using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class SynonymExporter : ExporterBase<SynonymEntity>
    {
        public SynonymExporter(): base(StoreFactory.GetStore<SynonymEntity>())
        {
        }
    }
}
