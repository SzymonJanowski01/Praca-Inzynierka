using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.Models
{
    public class Phase : ReactiveObject
    {
        [JsonPropertyName("phase_id")]
        [Reactive]
        public string? PhaseId { get; set; }

        [JsonPropertyName("name")]
        [Reactive]
        public string? PhaseName { get; set;}

        [JsonPropertyName("main_character")]
        [Reactive]
        public string? MainCharacter { get; set;}

        [JsonPropertyName("first_alternative_character")]
        [Reactive]
        public string? FirstAlternaticeCharacter { get; set; }

        [JsonPropertyName("second_alternative_character")]
        [Reactive]
        public string? SecondAlternaticeCharacter { get; set;}

    }
}
