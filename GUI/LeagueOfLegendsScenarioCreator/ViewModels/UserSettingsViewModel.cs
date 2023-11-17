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
        public bool DeletionConfirmation { get; set; }

        [Reactive]
        public string? Password { get; set; }
        [Reactive]
        public string? DeletionIncorrectData { get; set; }

        [Reactive]
        public bool DeletionLock { get; set; }

        public ReactiveCommand<string, Unit> ChangeVisibilityCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public UserSettingsViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            DeletionConfirmation = false;
            DeletionIncorrectData = string.Empty;
            DeletionLock = false;

            ChangeVisibilityCommand = ReactiveCommand.Create<string>(ChangeConfirmationVisibility);
            DeleteAccountCommand = ReactiveCommand.Create(DeleteAccount);
            CancelCommand = ReactiveCommand.Create(Cancel);
            LogoutCommand = ReactiveCommand.Create(Logout);
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

        public void Cancel()
        {
            MainWindowContent!.ToScenarios();
        }

        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
