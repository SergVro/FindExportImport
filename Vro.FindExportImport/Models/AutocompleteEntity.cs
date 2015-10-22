using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vro.FindExportImport.Models
{
    public class AutocompleteEntity : IOptimizationEntity
    {
        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; set; }

        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }
    }
}