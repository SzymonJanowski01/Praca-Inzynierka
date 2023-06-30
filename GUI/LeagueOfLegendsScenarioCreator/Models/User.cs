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
    public class User : ReactiveObject
    {
        [JsonPropertyName("user_id")]
        [Reactive]
        public string? UserId { get; set; }

        [JsonPropertyName("username")]
        [Reactive]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        [Reactive]
        public string? Email { get; set; }

        [JsonPropertyName("password")]
        [Reactive]
        public string? Password { get; set; }

        [Reactive]
        public ObservableCollection<string>? ScenariosNames { get; set; }

        [Reactive]
        public ObservableCollection<Scenario>? Scenarios { get; set; }
    }
}
