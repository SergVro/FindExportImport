﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof(IExporter))]
    public class SynonymExporter : ExporterBase<SynonymEntity>
    {
        public SynonymExporter() : base("_admin/synonym?from={0}&size={1}")
        {
        }
    }
}
