using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Import
{
    [ServiceConfiguration(typeof(IImporter))]
    public class AutocompleteImporter : ImporterBase<AutocompleteEntity>
    {
        public AutocompleteImporter() : base("_autocomplete/")
        {
        }
    }
}
