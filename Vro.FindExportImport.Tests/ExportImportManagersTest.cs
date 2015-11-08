using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vro.FindExportImport.Export;
using Vro.FindExportImport.Import;
using Vro.FindExportImport.Models;
using Xunit;

namespace Vro.FindExportImport.Tests
{
    public class ExportImportManagersTest
    {

        [Fact]
        public void ExportManagerLimitedExportersTest()
        {
            // Arrange
            var context = new ExportImportContext();

            context.SetupAutocompletes(2);
            context.SetupRelatedQueries(1);

            var exporters = new List<IExporter> {context.AutocompleteExporter, context.RelatedQueryExporter};
            var exportManager = new ExportManager(exporters, context.SiteIdentityLoaderMock.Object, context.Settings);
            MemoryStream stream = new MemoryStream();

            // Act
            exportManager.ExportToStream(new List<string> {context.AutocompleteExporter.EntityKey}, Helpers.AllSitesId, Helpers.AllLanguages, stream);
            
            // Assert
            var outputStream = new MemoryStream(stream.ToArray());
            var streamReader = new StreamReader(outputStream);
            var exportedData = streamReader.ReadToEnd();
            Assert.NotNull(exportedData);
            Assert.Equal("[{\"$type\":\"Vro.FindExportImport.Models.EntitySet`1[[Vro.FindExportImport.Models.IOptimizationEntity, Vro.FindExportImport]], Vro.FindExportImport\",\"Key\":\"AutocompleteEntity\"," +
                         "\"Entities\":[" +
                         "{\"$type\":\"Vro.FindExportImport.Models.AutocompleteEntity, Vro.FindExportImport\",\"priority\":0,\"query\":\"testAQuery0\",\"id\":\"testAId0\",\"tags\":[\"siteid:84bfaf5c52a349a0bc61a9ffb6983a66\",\"language:7d2da0a9fc754533b091fa6886a51c0d\"]}," +
                         "{\"$type\":\"Vro.FindExportImport.Models.AutocompleteEntity, Vro.FindExportImport\",\"priority\":1,\"query\":\"testAQuery1\",\"id\":\"testAId1\",\"tags\":[\"siteid:84bfaf5c52a349a0bc61a9ffb6983a66\",\"language:7d2da0a9fc754533b091fa6886a51c0d\"]}" +
                         "]}]", exportedData);
                         
        }

        [Fact]
        public void ExportManagerAllExportersTest()
        {
            // Arrange
            var context = new ExportImportContext();

            context.SetupAutocompletes(1);
            context.SetupRelatedQueries(2);

            var exporters = new List<IExporter> { context.AutocompleteExporter, context.RelatedQueryExporter };
            var exportManager = new ExportManager(exporters, context.SiteIdentityLoaderMock.Object, context.Settings);
            MemoryStream stream = new MemoryStream();

            // Act
            exportManager.ExportToStream(new List<string> { context.AutocompleteExporter.EntityKey, context.RelatedQueryExporter.EntityKey }, Helpers.AllSitesId, Helpers.AllLanguages, stream);

            // Assert
            var outputStream = new MemoryStream(stream.ToArray());
            var streamReader = new StreamReader(outputStream);
            var exportedData = streamReader.ReadToEnd();
            Assert.NotNull(exportedData);
            Assert.Equal("[{\"$type\":\"Vro.FindExportImport.Models.EntitySet`1[[Vro.FindExportImport.Models.IOptimizationEntity, Vro.FindExportImport]], Vro.FindExportImport\"," +
                         "\"Key\":\"AutocompleteEntity\",\"Entities\":[" +
                         "{\"$type\":\"Vro.FindExportImport.Models.AutocompleteEntity, Vro.FindExportImport\",\"priority\":0,\"query\":\"testAQuery0\",\"id\":\"testAId0\",\"tags\":[\"siteid:84bfaf5c52a349a0bc61a9ffb6983a66\",\"language:7d2da0a9fc754533b091fa6886a51c0d\"]}]}," +
                         "{\"$type\":\"Vro.FindExportImport.Models.EntitySet`1[[Vro.FindExportImport.Models.IOptimizationEntity, Vro.FindExportImport]], Vro.FindExportImport\"," +
                         "\"Key\":\"RelatedQueryEntity\",\"Entities\":[" +
                         "{\"$type\":\"Vro.FindExportImport.Models.RelatedQueryEntity, Vro.FindExportImport\",\"priority\":0,\"query\":\"testRQQuery0\",\"suggestion\":\"testRQSuggestion0\",\"id\":\"testRQId0\",\"tags\":[\"siteid:84bfaf5c52a349a0bc61a9ffb6983a66\",\"language:7d2da0a9fc754533b091fa6886a51c0d\"]}," +
                         "{\"$type\":\"Vro.FindExportImport.Models.RelatedQueryEntity, Vro.FindExportImport\",\"priority\":1,\"query\":\"testRQQuery1\",\"suggestion\":\"testRQSuggestion1\",\"id\":\"testRQId1\",\"tags\":[\"siteid:84bfaf5c52a349a0bc61a9ffb6983a66\",\"language:7d2da0a9fc754533b091fa6886a51c0d\"]}" +
                         "]}]", exportedData);

        }

        [Fact]
        public void ExportImportTest()
        {
            // Arrange
            var exportContext = new ExportImportContext();

            var autocompletesCount = 25;
            var relatedQueriesCount = 49;

            exportContext.SetupAutocompletes(autocompletesCount);
            exportContext.SetupRelatedQueries(relatedQueriesCount);

            var exporters = new List<IExporter> { exportContext.AutocompleteExporter, exportContext.RelatedQueryExporter };
            var exportManager = new ExportManager(exporters, exportContext.SiteIdentityLoaderMock.Object, exportContext.Settings);
            

            var importContext = new ExportImportContext();
            importContext.SetupAutocompletes(0);
            importContext.SetupRelatedQueries(0);
            var importers = new List<IImporter> { importContext.AutocompleteImporter, importContext.RelatedQueryImporter };
            var importManager = new ImportManager(importers);

            // Act
            MemoryStream exportStream = new MemoryStream();
            exportManager.ExportToStream(new List<string> { exportContext.AutocompleteExporter.EntityKey, exportContext.RelatedQueryExporter.EntityKey }, Helpers.AllSitesId, Helpers.AllLanguages, exportStream);
            var importStream = new MemoryStream(exportStream.ToArray());
            importManager.ImportFromStream("MySite", importStream);
            
            // Assert   
            Assert.Equal(autocompletesCount, importContext.Autocompletes.Count);
            Assert.Equal(relatedQueriesCount, importContext.RelatedQueries.Count);

            for (int i = 0; i < autocompletesCount; i++)
            {
                Assert.Equal(exportContext.Autocompletes[i].Id, importContext.Autocompletes[i].Id); 
                Assert.Equal(exportContext.Autocompletes[i].Query, importContext.Autocompletes[i].Query); 
                Assert.Equal(exportContext.Autocompletes[i].Priority, importContext.Autocompletes[i].Priority); 
                Assert.NotEqual(exportContext.Autocompletes[i].Tags, importContext.Autocompletes[i].Tags); 
                Assert.Equal("siteid:MySite", importContext.Autocompletes[i].Tags.First());
            }

            for (int i = 0; i < relatedQueriesCount; i++)
            {
                Assert.Equal(exportContext.RelatedQueries[i].Id, importContext.RelatedQueries[i].Id);
                Assert.Equal(exportContext.RelatedQueries[i].Query, importContext.RelatedQueries[i].Query);
                Assert.Equal(exportContext.RelatedQueries[i].Priority, importContext.RelatedQueries[i].Priority);
                Assert.Equal(exportContext.RelatedQueries[i].Suggestion, importContext.RelatedQueries[i].Suggestion);
                Assert.NotEqual(exportContext.RelatedQueries[i].Tags, importContext.RelatedQueries[i].Tags);
                Assert.Equal("siteid:MySite", importContext.RelatedQueries[i].Tags.First());
            }
        }
    }
}
