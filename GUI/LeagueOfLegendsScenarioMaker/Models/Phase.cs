using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioMaker.Models
{
    public class Phase : ReactiveObject
    {
        [Reactive]
        public string? PhaseId { get; set; }
        [Reactive]
        public string? PhaseName { get; set;}
        [Reactive]
        public string? MainCharacter { get; set;}
        [Reactive]
        public string? FirstAlternaticeCharacter { get; set; }
        [Reactive]
        public string? SecondAlternaticeCharacter { get; set;}
    }
}
