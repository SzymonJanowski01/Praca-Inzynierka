using LeagueOfLegendsScenarioMaker.Models;
using ReactiveUI.Fody.Helpers;

namespace LeagueOfLegendsScenarioMaker.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        [Reactive]
        public ViewModelBase? Content { get; set; }
        [Reactive]
        public User? User { get; set; }

        public MainWindowViewModel() { }
    }
}