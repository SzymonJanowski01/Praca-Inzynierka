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

        [Reactive]
        public string? RegisterIncorrectData { get; set; }

        [Reactive]
        public bool RegisterLock { get; set; }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; private set; }

        public RegisterViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            RegisterCommand = ReactiveCommand.Create(Register);
            RegisterIncorrectData = string.Empty;
            RegisterLock = false;
        }

        private async void Register()
        {
            RegisterIncorrectData = string.Empty;

            try
            {
                RegisterLock = true;

                var user = await ServerConnection.CreateUser(Username!, Email!, Password!);
                MainWindowContent!.User = user;
                MainWindowContent!.User!.ScenariosNames = await ServerConnection.GetUserScenariosNames(MainWindowContent!.User!.UserId!);
                MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

                MainWindowContent!.ToScenarios();
            }

            catch (ServiceUnavailableException) 
            {
                await Task.Delay(1500);
                RegisterIncorrectData = "Service unavaible";
                RegisterLock = false;
            }
            catch (UserConflictException)
            {
                await Task.Delay(1500);
                RegisterIncorrectData = "E-mail or username already taken!";
                RegisterLock = false;
            }
            catch (Exception ex) 
            {
                await Task.Delay(1500);
                RegisterIncorrectData = $"{ex}";
                RegisterLock = false;
            }
            
        }
    }
}
