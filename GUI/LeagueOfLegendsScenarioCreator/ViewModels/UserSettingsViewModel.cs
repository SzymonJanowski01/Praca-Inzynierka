using LeagueOfLegendsScenarioCreator.CustomExceptions;
using LeagueOfLegendsScenarioCreator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    public class UserSettingsViewModel : ViewModelBase
    {
        [Reactive] public MainWindowViewModel? MainWindowContent { get; set; }

        [Reactive] public string? UsernameUpdate { get; set; }

        [Reactive] public string? EmailUpdate { get; set; }

        [Reactive] public string? PasswordUpdate { get; set; }

        [Reactive] public string? ConfirmPasswordUpdate { get; set; }

        [Reactive] public int ScenariosNumbers { get; set; }
        
        [Reactive] public string? UpdateMessage { get; set; }

        [Reactive] public string? InformationBorderColor { get; set; }

        [Reactive] public bool IsButtonEnabled { get; set; }

        [Reactive] public bool DeletionConfirmation { get; set; }

        [Reactive] public string? Password { get; set; }

        [Reactive] public string? DeletionIncorrectData { get; set; }

        [Reactive] public bool DeletionLock { get; set; }

        [Reactive] public bool BackConfirmation { get; set; }

        public ReactiveCommand<Unit, Unit> UpdateCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAndExitCommand { get; private set; }
        public ReactiveCommand<string, Unit> ChangeVisibilityCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public UserSettingsViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            ScenariosNumbers = MainWindowContent!.User!.ScenariosNames!.Count;
            InformationBorderColor = "Red";
            DeletionConfirmation = false;
            DeletionIncorrectData = string.Empty;
            DeletionLock = false;
            BackConfirmation = false;

            UpdateCommand = ReactiveCommand.Create(UpdateUser);
            SaveAndExitCommand = ReactiveCommand.Create(SaveAndExit);
            ChangeVisibilityCommand = ReactiveCommand.Create<string>(ChangeConfirmationVisibility);
            DeleteAccountCommand = ReactiveCommand.Create(DeleteAccount);
            BackCommand = ReactiveCommand.Create(Back);
            LogoutCommand = ReactiveCommand.Create(Logout);
        }

        /// <summary>
        /// Function responsible for updating user data.
        /// At least one setting must be changed, if password is being updated, both password and confirm password fields must be filled.
        /// </summary>
        public void UpdateUser()
        {
            if (string.IsNullOrEmpty(PasswordUpdate) && string.IsNullOrEmpty(ConfirmPasswordUpdate))
            {
                if (string.IsNullOrEmpty(UsernameUpdate) && string.IsNullOrEmpty(EmailUpdate))
                {
                    IncorrectUpdate("At least one setting must be changed!", 1500);
                }
                else
                {
                    CorrectUpdate();
                }
            }
            else if (!string.IsNullOrEmpty(PasswordUpdate) && !string.IsNullOrEmpty(ConfirmPasswordUpdate))
            {
                if (PasswordUpdate == ConfirmPasswordUpdate)
                {
                    CorrectUpdate();
                }
                else
                {
                    IncorrectUpdate("Password fields do not match each other!", 1500);
                }
            }
            else
            {
                IncorrectUpdate("When updating password both password and confirm password fields must be filled!", 2500);
            } 
        }

        /// <summary>
        /// Helper function for <see cref="UpdateUser"/> function. Commits changes to server and shows message to user.
        /// </summary>
        public async void CorrectUpdate()
        {
            try
            {
                InformationBorderColor = "Green";
                var tempScenariosNames = MainWindowContent!.User!.ScenariosNames;
                var tempScenarios = MainWindowContent!.User!.Scenarios;
                var user = await ServerConnection.UpdateUser(MainWindowContent!.User!.UserId!, UsernameUpdate, EmailUpdate, PasswordUpdate);
                MainWindowContent!.User = user;
                MainWindowContent!.User!.ScenariosNames = tempScenariosNames;
                MainWindowContent!.User!.Scenarios = tempScenarios;

                UpdateMessage = "Successfully updated!";
                await Task.Delay(1500);
                UpdateMessage = string.Empty;

                UsernameUpdate = string.Empty;
                EmailUpdate = string.Empty;
                PasswordUpdate = string.Empty;
                ConfirmPasswordUpdate = string.Empty;
            }
            catch (UserConflictException ex)
            {
                IncorrectUpdate($"{ex.Message}", 1500);
            }
            catch (ServiceUnavailableException)
            {
                IncorrectUpdate("Service is currently unavailable", 1500);
            }
            catch (Exception ex)
            {
                IncorrectUpdate($"{ex.Message}", 1500);
            }
        }

        /// <summary>
        /// Helper function for <see cref="UpdateUser"/> function. Shows message of inccorect update attempt to user.
        /// </summary>
        /// <param name="message">Content of the message to show.</param>
        /// <param name="delay">How long the message will be shown.</param>
        public async void IncorrectUpdate(string message, int delay)
        {
            InformationBorderColor = "Red";
            UpdateMessage = message;
            await Task.Delay(delay);
            UpdateMessage = string.Empty;
        }

        /// <summary>
        /// Updates user data and if the update is correct returns to <see cref="ScenariosViewModel"/>.
        /// </summary>
        public async void SaveAndExit()
        {
            UpdateUser();
            await Task.Delay(500);
            if (InformationBorderColor == "Green")
            {
                Back();
            }
        }
        
        /// <summary>
        /// Helperf function to show/hide deletion confirmation message.
        /// </summary>
        /// <param name="parameter"></param>
        public void ChangeConfirmationVisibility(string parameter)
        {
            if (parameter == "t") 
            {
                DeletionConfirmation = true;
            }
            else
            {
                DeletionConfirmation = false;
            }
        }

        /// <summary>
        /// Deletes user account, shows message to user if the deletion was not successful.
        /// </summary>
        public async void DeleteAccount()
        {
            try
            {
                DeletionLock = true;

                var CheckPassword = await ServerConnection.CheckCredentials(MainWindowContent!.User!.Email!, Password!);

                await ServerConnection.DeleteUser(CheckPassword!);
                MainWindowContent!.WipeData();
                MainWindowContent!.ToRegister();
            }
            catch (NotFoundException)
            {
                DeletionIncorrectData = "User with provided username/email does not exist";
                await Task.Delay(1500);
                DeletionIncorrectData = string.Empty;
                DeletionLock = false;
            }
            catch (UnauthorizedException)
            {
                DeletionIncorrectData = "Wrong password!";
                await Task.Delay(1500);
                DeletionIncorrectData = string.Empty;
                DeletionLock = false;
            }
            catch (ServiceUnavailableException)
            {
                DeletionIncorrectData = "Service unavaible";
                await Task.Delay(1500);
                DeletionIncorrectData = string.Empty;
                DeletionLock = false;
            }
        }

        /// <summary>
        /// If there are unsaved changes, shows message to user, if not returns to <see cref="ScenariosViewModel"/>.
        /// </summary>
        public void Back()
        {
            if (BackConfirmation)
            {
                MainWindowContent!.ToScenarios();
            }
            else
            {
                if (!string.IsNullOrEmpty(UsernameUpdate) || !string.IsNullOrEmpty(EmailUpdate) || !string.IsNullOrEmpty(PasswordUpdate) || !string.IsNullOrEmpty(ConfirmPasswordUpdate))
                {
                    IncorrectUpdate("You have unsaved changes! Press once again to dismiss this message.", 1500);

                    BackConfirmation = true;
                }
                else
                {
                    MainWindowContent!.ToScenarios();
                }
            }
        }

        /// <summary>
        /// Logs out user.
        /// </summary>
        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
