using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.Models
{
    public class Scenario : ReactiveObject
    {
        [JsonPropertyName("scenario_id")]
        [Reactive]
        public string? ScenarioId { get; set; }

        [JsonPropertyName("name")]
        [Reactive]
        public string? ScenarioName { get; set; }

        [JsonPropertyName("created_at")]
        [Reactive]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("last_modified_at")]
        [Reactive]
        public string? LastModifiedAt { get; set; }

        [Reactive]
        public ObservableCollection<Phase>? Phases { get; set; }
    }
}
