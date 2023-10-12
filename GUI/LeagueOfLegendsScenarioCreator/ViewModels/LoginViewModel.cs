using LeagueOfLegendsScenarioCreator.CustomExceptions;
using LeagueOfLegendsScenarioCreator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
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

        [Reactive]
        public string? LoginIncorrectData { get; set; }

        [Reactive]
        public bool LoginLock { get; set; }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; private set; }

        public LoginViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            LoginCommand = ReactiveCommand.Create(Login);
            LoginIncorrectData = string.Empty;
            LoginLock = false;
        }

        private async void Login()
        {
            LoginIncorrectData = string.Empty;

            try
            {
                LoginLock = true;

                var id = await ServerConnection.CheckCredentials(UsernameOrEmail!, Password!);

                if (id != "NotFound" && id != "Unauthorized")
                {
                    var user = await ServerConnection.GetUser(id!);
                    MainWindowContent!.User = user;
                    MainWindowContent!.User.ScenariosNames = await ServerConnection.GetUserScenariosNames(MainWindowContent!.User!.UserId!);
                    MainWindowContent!.User.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

                    MainWindowContent!.ToScenarios();
                }
                else if (id == "Unauthorized")
                {
                    await Task.Delay(1500);
                    LoginIncorrectData = "Wrong password!";
                    LoginLock = false;
                }
                else
                {
                    await Task.Delay(1500);
                    LoginIncorrectData = "User with provided username/email does not exist";
                    LoginLock = false;
                }
            }
            catch (ServiceUnavailableException)
            {
                await Task.Delay(1500);
                LoginIncorrectData = "Service unavaible";
                LoginLock = false;
            }
            catch (Exception ex)
            {
                await Task.Delay(1500);
                LoginIncorrectData = $"{ex}";
                LoginLock = false;
            }
        }
    }
}
