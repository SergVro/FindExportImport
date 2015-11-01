using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Core;
using Moq;
using Vro.FindExportImport.Models;
using Vro.FindExportImport.Stores;
using Xunit;

namespace Vro.FindExportImport.Tests
{
    public class BestBetStoreTest
    {

        [Fact]
        public void GetById()
        {
            // Arrange
            var context = new BestBetStoreTestContext();

            context.ResponseMessage.StatusCode = HttpStatusCode.OK;
            context.ResponseMessage.Content = new StringContent(@"
            {
	            'status': 'ok',
                'item': {
                    'id': 'testId',
                    'phrases': 'planning',
                    'target_description': 'Alloy Plan (6)',
                    'target_type': 'PageBestBetSelector',
                    'target_key': '6',
                    'tags': ['siteid:84BFAF5C52A349A0BC61A9FFB6983A66',
                    'language:7D2DA0A9FC754533B091FA6886A51C0D'],
		            'date_added': '2015-11-01T18:38:58.092388+01:00',
		            'best_bet_has_own_style': true,
		            'best_bet_target_title': 'Alloy Plan for planning',
		            'best_bet_target_description': '',
		            'best_bet_target_url': 'http://mysite/alloy-plan/',
		            'best_bet_target_document_title': 'Alloy Plan',
		            'best_bet_target_document_description': 'Alloy Plan, online project management Project management has never been easier!'
                }
            }");
            context.MockController.Setup(c => c.Get("testId")).Returns(context.ResponseMessage);
            context.MockContent.Setup(c => c.Name).Returns("Alloy Plan");
            context.MockContentRepository.Setup(r => r.Get<IContent>(new ContentReference(6))).Returns(context.MockContent.Object);

            var bestBetStore = context.BestBetStore;

            // Act
            var bestBet = bestBetStore.Get("testId");
            
            // Assert
            Assert.NotNull(bestBet);
            Assert.Equal("testId", bestBet.Id);
            Assert.Equal("planning", bestBet.Phrase);
            Assert.Equal("6", bestBet.TargetKey);
            Assert.Equal("PageBestBetSelector", bestBet.TargetType);
            Assert.Equal("Alloy Plan for planning", bestBet.BestBetTargetTitle);
            Assert.Equal("Alloy Plan", bestBet.TargetName);
        }

        [Fact]
        public void List()
        {
            // Arrange
            var context = new BestBetStoreTestContext();

            context.ResponseMessage.StatusCode = HttpStatusCode.OK;
            context.ResponseMessage.Content = new StringContent(@"
            {
	            'status': 'ok',
	            'total': 2,
	            'hits': [
	            {
		            'id': 'testId',
		            'phrases': 'planning',
		            'target_description': 'Alloy Plan (6)',
		            'target_type': 'PageBestBetSelector',
		            'target_key': '6',
		            'tags': ['siteid:84BFAF5C52A349A0BC61A9FFB6983A66',
		            'language:7D2DA0A9FC754533B091FA6886A51C0D'],
		            'date_added': '2015-11-01T18:38:58.092388+01:00',
		            'best_bet_has_own_style': true,
		            'best_bet_target_title': 'Alloy Plan for planning',
		            'best_bet_target_description': '',
		            'best_bet_target_url': 'http://mysite/alloy-plan/',
		            'best_bet_target_document_title': 'Alloy Plan',
		            'best_bet_target_document_description': 'Alloy Plan, online project management Project management has never been easier! 
                        Project management has never been easier! Use Alloy Meet with Alloy Plan to get the whole team involved in the creation'
	            },
	            {
		            'id': 'testId2',
		            'phrases': 'plan',
		            'target_description': 'Dagens Nyheter',
		            'target_type': 'ExternalUrlBestBetSelector',
		            'target_key': 'http://www.dn.se',
		            'tags': ['siteid:84BFAF5C52A349A0BC61A9FFB6983A66',
		            'language:7D2DA0A9FC754533B091FA6886A51C0D'],
		            'date_added': '2015-10-31T21:09:08.87',
		            'best_bet_has_own_style': true,
		            'best_bet_target_title': 'news',
		            'best_bet_target_description': null,
		            'best_bet_target_url': 'http://www.dn.se',
		            'best_bet_target_document_title': null,
		            'best_bet_target_document_description': null
	            },

	            ]
            }
            ");
            context.MockController.Setup(c => c.GetList(0, 10, "siteid:84BFAF5C52A349A0BC61A9FFB6983A66,language:7D2DA0A9FC754533B091FA6886A51C0D"))
                .Returns(context.ResponseMessage);
            context.MockContent.Setup(c => c.Name).Returns("Alloy Plan");
            context.MockContentRepository.Setup(r => r.Get<IContent>(new ContentReference(6))).Returns(context.MockContent.Object);
            var bestBetStore = context.BestBetStore;

            // Act
            var listResult = bestBetStore.List("84BFAF5C52A349A0BC61A9FFB6983A66", "7D2DA0A9FC754533B091FA6886A51C0D", 0, 10);

            // Assert
            Assert.NotNull(listResult);
            Assert.Equal(2, listResult.Total);
            Assert.Equal(2, listResult.Hits.Count);
            var bestBetEntity1 = listResult.Hits.First();
            Assert.Equal("testId", bestBetEntity1.Id);
            Assert.Equal("planning", bestBetEntity1.Phrase);
        }
    }
}
