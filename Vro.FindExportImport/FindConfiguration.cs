using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vro.FindExportImport
{
    public class FindConfiguration 
    {
        public string ServiceUrl { get; set; }
        public string DefaultIndex { get; set; }
        public int? DefaultRequestTimeout { get; set; }

        public FindConfiguration()
        {
        }

        public FindConfiguration(EPiServer.Find.Configuration configuration)
        {
            ServiceUrl = configuration.ServiceUrl;
            DefaultIndex = configuration.DefaultIndex;
            DefaultRequestTimeout = configuration.DefaultRequestTimeout;
        }
    }
}
