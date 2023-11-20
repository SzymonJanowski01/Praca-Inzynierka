using LeagueOfLegendsScenarioCreator.Services;
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
        public ReactiveCommand<Unit, Unit> UserSettingsCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }
        public ScenarioPresenterViewModel(MainWindowViewModel? mainViewModel)
        {
            MainWindowContent = mainViewModel;

            BackCommand = ReactiveCommand.Create(Back);
            UserSettingsCommand = ReactiveCommand.Create(UserSettings);
            LogoutCommand = ReactiveCommand.Create(Logout);
        }

        public async void Back()
        {
            MainWindowContent!.Scenario = null;
            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

            MainWindowContent!.ToScenarios();
        }

        public void UserSettings()
        {
            MainWindowContent!.ToUserSettings();
        }

        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
