using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using StructureMap;

namespace Vro.FindExportImport
{
    [ServiceConfiguration(typeof(FindConfiguration))]
    public class FindConfiguration 
    {
        public string ServiceUrl { get; set; }
        public string DefaultIndex { get; set; }
        public int? DefaultRequestTimeout { get; set; }

        [DefaultConstructor]
        public FindConfiguration(): this(Configuration.GetConfiguration())
        {
        }

        public FindConfiguration(Configuration configuration)
        {
            ServiceUrl = configuration.ServiceUrl;
            DefaultIndex = configuration.DefaultIndex;
            DefaultRequestTimeout = configuration.DefaultRequestTimeout;
        }

        public FindConfiguration(string serviceUrl, string defaultIndex, int? defaultRequestTimeout)
        {
            ServiceUrl = serviceUrl;
            DefaultIndex = defaultIndex;
            DefaultRequestTimeout = defaultRequestTimeout;
        }
    }
}
