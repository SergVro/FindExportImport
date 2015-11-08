using System.Collections.Generic;
using System.Linq;
using EPiServer.Find.Api;
using EPiServer.Find.Framework.Statistics;
using Moq;
using Vro.FindExportImport.Export;
using Vro.FindExportImport.Import;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;
using Xunit;

namespace Vro.FindExportImport.Tests
{
    public class ExportImportContext
    {
        public List<AutocompleteEntity> Autocompletes { get; private set; }
        public AutocompleteExporter AutocompleteExporter { get; }
        public AutocompleteImporter AutocompleteImporter { get; } 
        public Mock<IStore<AutocompleteEntity>> AutocompleteStoreMock { get; }

        public List<RelatedQueryEntity> RelatedQueries { get; private set; }
        public RelatedQueryExporter RelatedQueryExporter { get; }
        public RelatedQueryImporter RelatedQueryImporter { get; }
        public Mock<IStore<RelatedQueryEntity>> RelatedQueryStoreMock { get; }

        public Mock<IStoreFactory> StoreFactoryMock { get; }
        public Mock<ISiteIdentityLoader> SiteIdentityLoaderMock { get; }
        public Settings Settings { get; set; }

        public ExportImportContext()
        {
            SiteIdentityLoaderMock = new Mock<ISiteIdentityLoader>();
            Settings = new Settings();

            AutocompleteStoreMock = new Mock<IStore<AutocompleteEntity>>();
            RelatedQueryStoreMock = new Mock<IStore<RelatedQueryEntity>>();

            StoreFactoryMock = new Mock<IStoreFactory>();
            StoreFactoryMock.Setup(f => f.GetStore<AutocompleteEntity>()).Returns(AutocompleteStoreMock.Object);
            StoreFactoryMock.Setup(f => f.GetStore<RelatedQueryEntity>()).Returns(RelatedQueryStoreMock.Object);

            AutocompleteExporter = new AutocompleteExporter(StoreFactoryMock.Object);
            AutocompleteImporter = new AutocompleteImporter(StoreFactoryMock.Object);

            RelatedQueryExporter = new RelatedQueryExporter(StoreFactoryMock.Object);
            RelatedQueryImporter = new RelatedQueryImporter(StoreFactoryMock.Object);

            
        }

        public void SetupAutocompletes(int count)
        {
            Autocompletes = new List<AutocompleteEntity>();
            for (int i = 0; i < count; i++)
            {
                var autocompleteEntity = new AutocompleteEntity
                {
                    Id = "testAId"+i,
                    Priority = i,
                    Query = "testAQuery"+i,
                    Tags = new List<string> { "siteid:" + Helpers.AllSitesId, "language:" + Helpers.AllLanguages }
                };
                Autocompletes.Add(autocompleteEntity);
            }
            AutocompleteStoreMock.Setup(s => s.Get(It.IsAny<string>())).Returns<string>(id => Autocompletes.FirstOrDefault(a => a.Id.Equals(id)));

            AutocompleteStoreMock.Setup(s => s.Create(It.IsAny<AutocompleteEntity>()))
                .Callback<AutocompleteEntity>(e => Autocompletes.Add(e));
            AutocompleteStoreMock.Setup(s => s.Delete(It.IsAny<string>()))
                .Callback<string>(id => Autocompletes.RemoveAll(a => a.Id.Equals(id)));

            AutocompleteStoreMock.Setup(s => s.List(Helpers.AllSitesId, Helpers.AllLanguages, It.IsAny<int>(), It.IsAny<int>()))
                .Returns<string, string, int,int>((site, language, from, size) => new ListResult<AutocompleteEntity>
                {
                    Status = "ok",
                    Total = Autocompletes.Count,
                    Hits = Autocompletes.Skip(from).Take(size).ToList()
                });

        }

        public void SetupRelatedQueries(int count)
        {
            RelatedQueries = new List<RelatedQueryEntity>();
            for (int i = 0; i < count; i++)
            {
                var relatedQueryEntity = new RelatedQueryEntity
                {
                    Id = "testRQId" + i,
                    Priority = i,
                    Query = "testRQQuery" + i,
                    Suggestion = "testRQSuggestion" + i,
                    Tags = new List<string> { "siteid:" + Helpers.AllSitesId, "language:" + Helpers.AllLanguages }
                };
                RelatedQueries.Add(relatedQueryEntity);
            }
            RelatedQueryStoreMock.Setup(s => s.Get(It.IsAny<string>())).Returns<string>(id => RelatedQueries.FirstOrDefault(a => a.Id.Equals(id)));

            RelatedQueryStoreMock.Setup(s => s.Create(It.IsAny<RelatedQueryEntity>()))
                .Callback<RelatedQueryEntity>(e => RelatedQueries.Add(e));
            RelatedQueryStoreMock.Setup(s => s.Delete(It.IsAny<string>()))
                .Callback<string>(id => RelatedQueries.RemoveAll(a => a.Id.Equals(id)));

            RelatedQueryStoreMock.Setup(s => s.List(Helpers.AllSitesId, Helpers.AllLanguages, It.IsAny<int>(), It.IsAny<int>()))
                .Returns<string, string, int, int>((site, language, from, size) => new ListResult<RelatedQueryEntity>
                {
                    Status = "ok",
                    Total = RelatedQueries.Count,
                    Hits = RelatedQueries.Skip(from).Take(size).ToList()
                });
        }
    }

    public class ExportImportContextTests
    {
        [Fact]
        public void TestSetupAutocompleteExportImportContext()
        {
            var context = new ExportImportContext();
            context.SetupAutocompletes(3);

            Assert.Equal(3, context.AutocompleteStoreMock.Object.List(Helpers.AllSitesId, Helpers.AllLanguages, 0, 20).Total);
            Assert.NotNull(context.AutocompleteStoreMock.Object.Get("testAId1"));

            var res = context.AutocompleteStoreMock.Object.List(Helpers.AllSitesId, Helpers.AllLanguages, 2, 1);
            Assert.Equal(3, res.Total);
            Assert.Equal(1, res.Hits.Count);
            Assert.Equal("testAId2", res.Hits.First().Id);

            context.AutocompleteStoreMock.Object.Delete("testAId1");
            Assert.Equal(2, context.AutocompleteStoreMock.Object.List(Helpers.AllSitesId, Helpers.AllLanguages, 0, 20).Total);
            Assert.Null(context.AutocompleteStoreMock.Object.Get("testAId1"));

            context.AutocompleteStoreMock.Object.Create(new AutocompleteEntity
            {
                Id = "myNewAId",
                Priority = 999,
                Query = "myNewQuery"
            });
            Assert.Equal(3, context.AutocompleteStoreMock.Object.List(Helpers.AllSitesId, Helpers.AllLanguages, 0, 20).Total);
            Assert.NotNull(context.AutocompleteStoreMock.Object.Get("myNewAId"));


        }

        [Fact]
        public void TestSetupRelatedQueriesExportImportContext()
        {
            var context = new ExportImportContext();

            context.SetupRelatedQueries(2);

            Assert.Equal(2, context.RelatedQueryStoreMock.Object.List(Helpers.AllSitesId, Helpers.AllLanguages, 0, 20).Total);
            Assert.NotNull(context.RelatedQueryStoreMock.Object.Get("testRQId1"));

            var resRq = context.RelatedQueryStoreMock.Object.List(Helpers.AllSitesId, Helpers.AllLanguages, 1, 1);
            Assert.Equal(2, resRq.Total);
            Assert.Equal(1, resRq.Hits.Count);
            Assert.Equal("testRQId1", resRq.Hits.First().Id);

            context.RelatedQueryStoreMock.Object.Delete("testRQId1");
            Assert.Equal(1, context.RelatedQueryStoreMock.Object.List(Helpers.AllSitesId, Helpers.AllLanguages, 0, 20).Total);
            Assert.Null(context.RelatedQueryStoreMock.Object.Get("testRQId1"));

            context.RelatedQueryStoreMock.Object.Create(new RelatedQueryEntity
            {
                Id = "myNewRQId",
                Priority = 777,
                Query = "myNewQuery",
                Suggestion = "myNewSuggestion"
            });
            Assert.Equal(2, context.RelatedQueryStoreMock.Object.List(Helpers.AllSitesId, Helpers.AllLanguages, 0, 20).Total);
            Assert.NotNull(context.RelatedQueryStoreMock.Object.Get("myNewRQId"));
        }
    }
}