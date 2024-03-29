﻿using EPiServer.ServiceLocation;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;

namespace Vro.FindExportImport.Export
{
    [ServiceConfiguration(typeof (IExporter))]
    public class RelatedQueryExporter : ExporterBase<RelatedQueryEntity>
    {
        public RelatedQueryExporter(IStoreFactory storeFactory) : base(storeFactory.GetStore<RelatedQueryEntity>(), "#manage/optimization/relatedqueries")
        {
        }
    }
}