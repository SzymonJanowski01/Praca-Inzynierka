using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    public class UserSettingsViewModel : ViewModelBase
    {
        [Reactive]
        public MainWindowViewModel? MainWindowContent { get; set; }

        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public UserSettingsViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;

            LogoutCommand = ReactiveCommand.Create(Logout);
        }

        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
