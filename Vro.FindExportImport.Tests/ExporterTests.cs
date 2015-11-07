﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.Connection;
using Moq;
using Newtonsoft.Json;
using Vro.FindExportImport.Export;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;
using Xunit;

namespace Vro.FindExportImport.Tests
{
    public class ExporterTests
    {
        public const string AllSitesId = "84bfaf5c52a349a0bc61a9ffb6983a66";
        public const string AllLanguages = "7d2da0a9fc754533b091fa6886a51c0d";

        [Fact]
        public void AutocompleteExporterWriteToStream()
        {
            // Arrange
            string requestUrl = "";
            HttpVerbs? httpVerb = null;
            var context = new IndexStoreTestContext();
            var textWriter = new StringWriter();
            var writer = new JsonTextWriter(textWriter);
            var exporter = new AutocompleteExporter(context.StoreFactory);
            ((IndexStore<AutocompleteEntity>) exporter.Store).RequestFactory = context.GetMockRequestFactory(
                @"{ 
                    'total':2,
                    'status': 'ok',
                    'hits': [{
                        'id':'testId',
                        'query':'testQuery',
                        'type':'editorial',
                        'priority':536870912,
                        'tags':['siteid: 84bfaf5c52a349a0bc61a9ffb6983a66','language: 7d2da0a9fc754533b091fa6886a51c0d'],
                        'timestamp':'2015 - 10 - 30T17: 28:02Z'
                        }, {
                        'id':'testId2',
                        'query':'testQuery2',
                        'type':'editorial',
                        'priority':123456,
                        'tags':['siteid: 84bfaf5c52a349a0bc61a9ffb6983a66','language: 7d2da0a9fc754533b091fa6886a51c0d'],
                        'timestamp':'2015 - 10 - 30T17: 20:02Z'
                        }]
                }",
                (url, verbs, timeout) => {
                                             requestUrl = url;
                                             httpVerb = verbs;
                }).Object;
            
            // Act
            exporter.WriteToStream(AllSitesId, AllLanguages, writer);

            // Assert
            var exportResultsString = textWriter.ToString();
            Assert.NotNull(exportResultsString);

            var exportResults = exporter.DefaultSerializer.Deserialize<EntitySet<IOptimizationEntity>>(
                new JsonTextReader(new StringReader(exportResultsString)));
            Assert.Equal(typeof(AutocompleteEntity).Name, exportResults.Key);        
            Assert.Equal(2, exportResults.Entities.Count);
            Assert.IsAssignableFrom(typeof(AutocompleteEntity), exportResults.Entities.First());

            var exportedEntity1 = exportResults.Entities.First() as AutocompleteEntity;
            Assert.NotNull(exportedEntity1);
            Assert.Equal("testId", exportedEntity1.Id);
            Assert.Equal("testQuery", exportedEntity1.Query);
            Assert.Equal(536870912, exportedEntity1.Priority);
        }

        [Fact]
        public void AutocompleteExporterDeleteAll()
        {
            // Arrange
            var storeMock = new Mock<IStore<AutocompleteEntity>>();
            
            var listResult = new ListResult<AutocompleteEntity>();
            listResult.Status = "ok";
            listResult.Total = 2;
            listResult.Hits = new List<AutocompleteEntity>
            {
                new AutocompleteEntity {Id = "testId1"},
                new AutocompleteEntity {Id = "testId2"}
            };

            storeMock.Setup(s => s.List(AllSitesId, AllLanguages, 0, It.IsAny<int>())).Returns(listResult);
            var storeFactoryMock = new Mock<IStoreFactory>();
            storeFactoryMock.Setup(f => f.GetStore<AutocompleteEntity>()).Returns(storeMock.Object);
            var exporter = new AutocompleteExporter(storeFactoryMock.Object);

            // Act
            exporter.DeleteAll(AllSitesId, AllLanguages);
            
            // Assert
            storeMock.Verify(s => s.Delete(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void AutocompleteExporterGetTotal()
        {
            // Arrange
            var storeMock = new Mock<IStore<AutocompleteEntity>>();

            var listResult = new ListResult<AutocompleteEntity>();
            listResult.Status = "ok";
            listResult.Total = 2;
            listResult.Hits = new List<AutocompleteEntity>
            {
                new AutocompleteEntity {Id = "testId1"},
                new AutocompleteEntity {Id = "testId2"}
            };

            storeMock.Setup(s => s.List(AllSitesId, AllLanguages, 0, It.IsAny<int>())).Returns(listResult);
            var storeFactoryMock = new Mock<IStoreFactory>();
            storeFactoryMock.Setup(f => f.GetStore<AutocompleteEntity>()).Returns(storeMock.Object);
            var exporter = new AutocompleteExporter(storeFactoryMock.Object);

            // Act
            var count = exporter.GetTotalCount(AllSitesId, AllLanguages);

            // Assert
            Assert.Equal(2, count);
        }


    }
}
