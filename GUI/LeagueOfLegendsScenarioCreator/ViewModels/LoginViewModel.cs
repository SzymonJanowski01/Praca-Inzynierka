using LeagueOfLegendsScenarioCreator.CustomExceptions;
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
    public class LoginViewModel : ViewModelBase
    {
        [Reactive]
        public MainWindowViewModel? MainWindowContent { get; set; }

        [Reactive]
        public string? UsernameOrEmail { get; set; }

        [Reactive]
        public string? Password { get; set; }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; private set; }

        public LoginViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            LoginCommand = ReactiveCommand.Create(Login);
        }

        private async void Login()
        {
            try
            {
                var id = await ServerConnection.CheckCredentials(UsernameOrEmail!, Password!);

                if (id != "NotFound" && id != "Unauthorized")
                {
                    var user = await ServerConnection.GetUser(id!);
                    MainWindowContent!.User = user;
                    MainWindowContent!.User.ScenariosNames = await ServerConnection.GetUserScenariosNames(MainWindowContent!.User!.UserId!);
                    MainWindowContent!.User.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

                    MainWindowContent!.ToScenarios();
                }
            }
            catch (ServiceUnavailableException)
            {

            }
            catch (Exception)
            {

            }
            
        }
    }
}
