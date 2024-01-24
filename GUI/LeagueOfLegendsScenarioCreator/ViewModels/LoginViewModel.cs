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

                var user = await ServerConnection.GetUser(id!);
                MainWindowContent!.User = user;
                MainWindowContent!.User.ScenariosNames = await ServerConnection.GetUserScenariosNames(MainWindowContent!.User!.UserId!);
                MainWindowContent!.User.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

                MainWindowContent!.ToScenarios();
            }
            catch (NotFoundException)
            {
                IncorrectData(1500, "User with provided username/email does not exist");
            }
            catch (UnauthorizedException)
            {
                IncorrectData(1500, "Wrong password!");
            }
            catch (ServiceUnavailableException)
            {
                IncorrectData(1500, "Service unavaible");
            }
            catch (Exception ex)
            {
                IncorrectData(1500, $"{ex.Message}");
            }
        }

        public async void IncorrectData(int delay, string message)
        {
            await Task.Delay(delay);
            LoginIncorrectData = message;
            LoginLock = false;
        }
    }
}
