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
    public class User : ReactiveObject
    {
        [Reactive]
        public string? UserId { get; set; }
        [Reactive]
        public string? Username { get; set; }
        [Reactive]
        public string? Email { get; set;}
        [Reactive]
        public string? Password { get; set;}
        [Reactive]
        public ObservableCollection<Scenario>? Scenarios { get; set; }
    }
}
