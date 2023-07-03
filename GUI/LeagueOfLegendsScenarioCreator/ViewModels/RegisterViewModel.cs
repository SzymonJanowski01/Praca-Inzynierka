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
    public class RegisterViewModel : ViewModelBase
    {
        [Reactive]
        public MainWindowViewModel? MainWindowContent { get; set; }

        [Reactive]
        public string? Username { get; set; }

        [Reactive]
        public string? Email { get; set; }

        [Reactive]
        public string? Password { get; set; }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; private set; }

        public RegisterViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            RegisterCommand = ReactiveCommand.Create(Register);
        }

        private async void Register()
        {
            var user = await ServerConnection.CreateUser(Username!, Email!, Password!);
            MainWindowContent!.User = user;
            MainWindowContent!.User!.ScenariosNames = await ServerConnection.GetUserScenariosNames(MainWindowContent!.User!.UserId!);
            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

            MainWindowContent!.ToScenarios();
        }
    }
}
