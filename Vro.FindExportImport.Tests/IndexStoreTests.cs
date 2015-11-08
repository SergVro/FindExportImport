using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using EPiServer.Find;
using EPiServer.Find.Connection;
using Moq;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;
using Xunit;

namespace Vro.FindExportImport.Tests
{

    public class IndexStoreTests
    {

        [Fact]
        public void AutocompleteStoreGetByIdTest()
        {
            // Arrange           
            string requestUrl = "";
            HttpVerbs? httpVerb = null;
            var context = new IndexStoreTestContext();
            var mockRequestFactory = context.GetMockRequestFactory(
                @"{ 
                    'id':'testId',
                    'query':'testQuery',
                    'type':'editorial',
                    'priority':536870912,
                    'tags':['siteid: 84bfaf5c52a349a0bc61a9ffb6983a66','language: 7d2da0a9fc754533b091fa6886a51c0d'],
                    'timestamp':'2015 - 10 - 30T17: 28:02Z'
                }", 
                ((url, verbs, timeout) => {
                    requestUrl = url;
                    httpVerb = verbs;
                }));
            var autocompleteStore = context.StoreFactory.GetStore<AutocompleteEntity>() as IndexStore<AutocompleteEntity>;
            autocompleteStore.RequestFactory = mockRequestFactory.Object;

            // Act
            var entity = autocompleteStore.Get("testId");

            // Assert
            Assert.Equal("http://myfindurl/myindex/_autocomplete/testId", requestUrl);
            Assert.Equal(HttpVerbs.Get, httpVerb.Value);
            Assert.NotNull(entity);
            Assert.Equal("testId", entity.Id);
            Assert.Equal("testQuery", entity.Query);
            Assert.Equal(536870912, entity.Priority);
            Assert.Equal(2, entity.Tags.Count);
        }

        [Fact]
        public void AutocompleteStoreListTest()
        {
            // Arrange           
            string requestUrl = "";
            HttpVerbs? httpVerb = null;
            var context = new IndexStoreTestContext();

            var mockRequestFactory = context.GetMockRequestFactory(
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
                ((url, verbs, timeout) => {
                    requestUrl = url;
                    httpVerb = verbs;
                }));
            var autocompleteStore = context.StoreFactory.GetStore<AutocompleteEntity>() as IndexStore<AutocompleteEntity>;
            autocompleteStore.RequestFactory = mockRequestFactory.Object;

            // Act
            var entityList = autocompleteStore.List("84bfaf5c52a349a0bc61a9ffb6983a66", "7d2da0a9fc754533b091fa6886a51c0d", 0, 5);

            // Assert
            Assert.Equal("http://myfindurl/myindex/_autocomplete/list?from=0&size=5&tags=siteid:84bfaf5c52a349a0bc61a9ffb6983a66,language:7d2da0a9fc754533b091fa6886a51c0d", requestUrl);
            Assert.Equal(HttpVerbs.Get, httpVerb.Value);
            Assert.NotNull(entityList);
            Assert.Equal(2, entityList.Hits.Count);
            Assert.Equal("testId", entityList.Hits[0].Id);
            Assert.Equal("testQuery", entityList.Hits[0].Query);
            Assert.Equal(536870912, entityList.Hits[0].Priority);
            Assert.Equal(2, entityList.Hits[0].Tags.Count);
        }

        [Fact]
        public void AutocompleteStoreDeleteTest()
        {
            // Arrange           
            string requestUrl = "";
            HttpVerbs? httpVerb = null;
            var context = new IndexStoreTestContext();

            var mockRequestFactory = context.GetMockRequestFactory(
                @"{'status':'ok','id':'testId'}",
                ((url, verbs, timeout) => {
                    requestUrl = url;
                    httpVerb = verbs;
                }));
            var autocompleteStore = context.StoreFactory.GetStore<AutocompleteEntity>() as IndexStore<AutocompleteEntity>;
            autocompleteStore.RequestFactory = mockRequestFactory.Object;

            // Act
            autocompleteStore.Delete("testId");

            // Assert
            Assert.Equal("http://myfindurl/myindex/_autocomplete/testId", requestUrl);
            Assert.Equal(HttpVerbs.Delete, httpVerb.Value);
        }

        [Fact]
        public void AutocompleteStoreCreateTest()
        {
            // Arrange           
            string requestUrl = "";
            HttpVerbs? httpVerb = null;
            var context = new IndexStoreTestContext();

            var mockRequestFactory = context.GetMockRequestFactory(
                @"{'status':'ok','id':'testAId'}",
                ((url, verbs, timeout) => {
                    requestUrl = url;
                    httpVerb = verbs;
                }));
            var autocompleteStore = context.StoreFactory.GetStore<AutocompleteEntity>() as IndexStore<AutocompleteEntity>;
            autocompleteStore.RequestFactory = mockRequestFactory.Object;

            var entity = new AutocompleteEntity
            {
                Id = "testAId",
                Priority = 223,
                Query = "testAQuery",
                Tags = new List<string>()
            };

            // Act
            var result = autocompleteStore.Create(entity);

            // Assert
            Assert.Equal("http://myfindurl/myindex/_autocomplete", requestUrl);
            Assert.Equal(HttpVerbs.Put, httpVerb.Value);
            Assert.Equal("testAId", result);
        }

        [Fact]
        public void AutocompleteStoreExceptionHandlingTest()
        {
            // Arrange
            var context = new IndexStoreTestContext();
            var response = new Mock<HttpWebResponse>();

            var expected = "{'error':'TestErrorResponse', 'status':'error'}";
            var expectedBytes = Encoding.UTF8.GetBytes(expected);
            var responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            response.Setup(r => r.GetResponseStream()).Returns(responseStream);
            var webException = new WebException("Test Error", null, WebExceptionStatus.UnknownError, response.Object);

            var mockRequestFactory = context.GetErrorRequestFactory(webException);
            var autocompleteStore = context.StoreFactory.GetStore<AutocompleteEntity>() as IndexStore<AutocompleteEntity>;
            autocompleteStore.RequestFactory = mockRequestFactory.Object;

            // Act
            var exception = Assert.Throws<ServiceException>(() => autocompleteStore.Delete("testId"));

            // Assert
            Assert.Equal("Test Error"+Environment.NewLine+"TestErrorResponse", exception.Message);

            
        }
    }
}
