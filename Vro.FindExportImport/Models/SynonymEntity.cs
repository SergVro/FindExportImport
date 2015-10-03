using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vro.FindExportImport.Models
{
    public class SynonymEntity : IOptimizationEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        [JsonProperty(PropertyName = "phrase")]
        public string Phrase { get; set; }

        [JsonProperty(PropertyName = "synonym")]
        public string Synonym { get; set; }

        [JsonProperty(PropertyName = "bidirectional")]
        public bool Bidirectional { get; set; }

    }
}
