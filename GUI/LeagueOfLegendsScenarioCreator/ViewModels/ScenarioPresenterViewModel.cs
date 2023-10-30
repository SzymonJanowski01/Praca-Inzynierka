using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    public class ScenarioPresenterViewModel : ViewModelBase
    {
        [Reactive]
        public MainWindowViewModel? MainWindowContent { get; set; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }
        public ScenarioPresenterViewModel(MainWindowViewModel? mainViewModel)
        {
            MainWindowContent = mainViewModel;

            BackCommand = ReactiveCommand.Create(Back);
            LogoutCommand = ReactiveCommand.Create(Logout);
        }

        public void Back()
        {
            MainWindowContent!.Scenario = null;

            MainWindowContent!.ToScenarios();
        }

        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
