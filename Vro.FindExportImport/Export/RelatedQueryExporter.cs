using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.UI;
using EPiServer.Find.UI.Helpers;
using EPiServer.ServiceLocation;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class RelatedQueryExporter : BaseExporter
    {
        public RelatedQueryExporter() : base(EntityType.RelatedQuery, "_didyoumean/list?from={0}&size={1}")
        {
        }
    }
}
