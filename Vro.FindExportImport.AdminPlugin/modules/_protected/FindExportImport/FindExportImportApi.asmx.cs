using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace Vro.FindExportImport.AdminPlugin.modules._protected.FindExportImport
{

    [WebService]
    [ScriptService]
    public class FindExportImportApi : System.Web.Services.WebService
    {

        [WebMethod]
        public List<OptimizationCount> GetCounts(string siteId, string language)
        {
            var exportManager = new ExportManager();
            return exportManager.Exporters.Select(e => new OptimizationCount
            {
                EntityKey = e.EntityKey,
                Count = e.GetTotalCount(siteId, language)
            }).ToList();
        }
    }

    public class OptimizationCount
    {
        public string EntityKey { get; set; }
        public int Count { get; set; }
    }
}
