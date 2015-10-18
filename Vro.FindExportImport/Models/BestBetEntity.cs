using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vro.FindExportImport.Models
{
    public class BestBetEntity : IOptimizationEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        [JsonProperty(PropertyName = "phrases")]
        public string Phrase { get; set; }

        [JsonProperty(PropertyName = "best_bet_target_title")]
        public string BestBetTargetTitle { get; set; }

        [JsonProperty(PropertyName = "best_bet_target_description")]
        public string BestBetTargetDescription { get; set; }

        [JsonProperty(PropertyName = "target_type")]
        public string TargetType { get; set; }

        [JsonProperty(PropertyName = "target_key")]
        public string TargetKey { get; set; }

        [JsonProperty(PropertyName = "best_bet_has_own_style")]
        public bool BestBetHasOwnStyle { get; set; }


        [JsonProperty(PropertyName = "target_name")]
        public string TargetName { get; set; }

    }
}
