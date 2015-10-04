using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.UI;
using EPiServer.Find.UI.Helpers;
using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class RelatedQueryExporter : ExporterBase<RelatedQueryEntity>
    {
        public RelatedQueryExporter() : base(StoreFactory.GetStore<RelatedQueryEntity>())
        {
        }
    }
}
