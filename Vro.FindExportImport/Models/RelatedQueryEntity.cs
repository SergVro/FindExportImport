using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vro.FindExportImport.Models
{
    public class RelatedQueryEntity : IOptimizationEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; set; }

        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

        [JsonProperty(PropertyName = "suggestion")]
        public string Suggestion { get; set; }
    }
}
