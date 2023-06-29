using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioMaker.Models
{
    public class Scenario : ReactiveObject
    {
        [Reactive]
        public string? ScenarioId { get; set; }
        [Reactive]
        public string? ScenarioName { get; set; }
        [Reactive]
        public string? CreatedAt { get; set; }
        [Reactive]
        public string? LastModifiedAt { get; set; }
        [Reactive]
        public ObservableCollection<Phase>? Phases { get; set; }
    }
}
