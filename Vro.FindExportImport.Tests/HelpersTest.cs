using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Vro.FindExportImport.Tests
{
    public class HelpersTest
    {
        [Fact]
        public void GetEntityNameAutocompleteTest()
        {
            // Act
            var name = Helpers.GetEntityName("AutocompleteEntity");
            
            // Assert
            Assert.Equal("Autocomplete", name);
        }

        [Fact]
        public void GetEntityNameRelatedQueryTest()
        {
            // Act
            var name = Helpers.GetEntityName("RelatedQueryEntity");

            // Assert
            Assert.Equal("Related Query", name);
        }

        [Fact]
        public void GetEntityNameBestBetTest()
        {
            // Act
            var name = Helpers.GetEntityName("BestBetEntity");

            // Assert
            Assert.Equal("Best Bet", name);
        }

        [Fact]
        public void GetEntityNameSynonymTest()
        {
            // Act
            var name = Helpers.GetEntityName("SynonymEntity");

            // Assert
            Assert.Equal("Synonym", name);
        }
    }
}
