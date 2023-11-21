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
        [Reactive]
        public MainWindowViewModel? MainWindowContent { get; set; }

        [Reactive]
        public string? UsernameUpdate { get; set; }

        [Reactive]
        public string? EmailUpdate { get; set; }

        [Reactive]
        public string? PasswordUpdate { get; set; }

        [Reactive]
        public string? ConfirmPasswordUpdate { get; set; }
        
        [Reactive]
        public string? UpdateMessage { get; set; }

        [Reactive]
        public string? InformationBorderColor { get; set; }

        [Reactive]
        public bool IsButtonEnabled { get; set; }

        [Reactive]
        public bool DeletionConfirmation { get; set; }

        [Reactive]
        public string? Password { get; set; }

        [Reactive]
        public string? DeletionIncorrectData { get; set; }

        [Reactive]
        public bool DeletionLock { get; set; }

        [Reactive]
        public bool BackConfirmation { get; set; }

        public ReactiveCommand<Unit, Unit> UpdateCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAndExitCommand { get; private set; }
        public ReactiveCommand<string, Unit> ChangeVisibilityCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public UserSettingsViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
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

        public async void UpdateUser()
        {
            try
            {
                if (string.IsNullOrEmpty(PasswordUpdate) && string.IsNullOrEmpty(ConfirmPasswordUpdate))
                {
                    if (string.IsNullOrEmpty(UsernameUpdate) && string.IsNullOrEmpty(EmailUpdate))
                    {
                        InformationBorderColor = "Red";
                        UpdateMessage = "At least one setting must be changed!";
                        await Task.Delay(1500);
                        UpdateMessage = string.Empty;
                    }
                    else
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
                    }
                }
                else if (!string.IsNullOrEmpty(PasswordUpdate) && !string.IsNullOrEmpty(ConfirmPasswordUpdate))
                {
                    if (PasswordUpdate == ConfirmPasswordUpdate)
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
                    }
                    else
                    {
                        InformationBorderColor = "Red";
                        UpdateMessage = "Password fields do not match each other!";
                        await Task.Delay(1500);
                        UpdateMessage = string.Empty;
                    }
                }
                else
                {
                    InformationBorderColor = "Red";
                    UpdateMessage = "When updating password both password and confirm password fields must be filled!";
                    await Task.Delay(2000);
                    UpdateMessage = string.Empty;
                }
            }
            catch (UserConflictException ex)
            {
                InformationBorderColor = "Red";
                UpdateMessage = $"{ex.Message}";
                await Task.Delay(1500);
                UpdateMessage = string.Empty;
            }
            catch (ServiceUnavailableException)
            {
                InformationBorderColor = "Red";
                UpdateMessage = "Service is currently unavailable";
                await Task.Delay(1500);
                UpdateMessage = string.Empty;
            }
            catch (Exception ex)
            {
                InformationBorderColor = "Red";
                UpdateMessage = $"{ex.Message}";
                await Task.Delay(1500);
                UpdateMessage = string.Empty;
            }
        }

        public void SaveAndExit()
        {
            UpdateUser();
            Back();
        }

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

        public async void DeleteAccount()
        {
            try
            {
                DeletionLock = true;

                var CheckPassword = await ServerConnection.CheckCredentials(MainWindowContent!.User!.Email!, Password!);

                if (CheckPassword != "Unauthorized")
                {
                    await ServerConnection.DeleteUser(CheckPassword!);
                    MainWindowContent!.WipeData();
                    MainWindowContent!.ToRegister();
                }
                else
                {
                    DeletionIncorrectData = "Wrong password provided";
                    await Task.Delay(1500);
                    DeletionIncorrectData = string.Empty;
                    DeletionLock = false;
                }
            }
            catch (ServiceUnavailableException)
            {
                DeletionIncorrectData = "Service unavaible";
                await Task.Delay(1500);
                DeletionIncorrectData = string.Empty;
                DeletionLock = false;
            }
        }

        public async void Back()
        {
            if (BackConfirmation)
            {
                MainWindowContent!.ToScenarios();
            }
            else
            {
                if (!string.IsNullOrEmpty(UsernameUpdate) || !string.IsNullOrEmpty(EmailUpdate) || !string.IsNullOrEmpty(PasswordUpdate) || !string.IsNullOrEmpty(ConfirmPasswordUpdate))
                {
                    InformationBorderColor = "Red";
                    UpdateMessage = $"You have unsaved changes! Press once again to dismiss this message.";
                    await Task.Delay(1500);
                    UpdateMessage = string.Empty;

                    BackConfirmation = true;
                }
                else
                {
                    MainWindowContent!.ToScenarios();
                }
            }
        }

        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
