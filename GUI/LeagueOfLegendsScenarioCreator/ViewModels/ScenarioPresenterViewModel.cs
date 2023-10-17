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

        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }
        public ScenarioPresenterViewModel(MainWindowViewModel? mainViewModel)
        {
            MainWindowContent = mainViewModel;

            LogoutCommand = ReactiveCommand.Create(Logout);
        }

        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
