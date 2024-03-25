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
    /// <summary>
    /// Class responsible for registering user
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        [Reactive] public MainWindowViewModel? MainWindowContent { get; set; }

        [Reactive] public string? Username { get; set; }

        [Reactive] public string? Email { get; set; }

        [Reactive] public string? Password { get; set; }

        [Reactive] public string? RegisterIncorrectData { get; set; }

        [Reactive] public bool RegisterLock { get; set; }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; private set; }

        public RegisterViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            RegisterCommand = ReactiveCommand.Create(Register);
            RegisterIncorrectData = string.Empty;
            RegisterLock = false;
        }

        /// <summary>
        /// Method responsible for registering user, if username or email is already taken, it will display message.
        /// </summary>
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
                IncorrectData(1500, "Service unavaible");
            }
            catch (UserConflictException)
            {
                IncorrectData(1500, "E-mail or username already taken!");
            }
            catch (Exception ex) 
            {
                IncorrectData(1500, $"{ex.Message}");
            }
            
        }

        /// <summary>
        /// Helper method for displaying message about inccorect register attempt for a short time.
        /// </summary>
        /// <param name="delay">How long the message will be shown.</param>
        /// <param name="message">Content of the message to show.</param>
        public async void IncorrectData(int delay, string message)
        {
            await Task.Delay(delay);
            RegisterIncorrectData = message;
            RegisterLock = false;
        }
    }
}
