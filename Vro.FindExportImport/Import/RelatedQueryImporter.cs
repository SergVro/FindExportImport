using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Import
{
    [ServiceConfiguration(typeof(IImporter))]
    public class RelatedQueryImporter : ImporterBase<RelatedQueryEntity>
    {
        public RelatedQueryImporter() : base(StoreFactory.GetStore<RelatedQueryEntity>())
        {
        }
    }
}
