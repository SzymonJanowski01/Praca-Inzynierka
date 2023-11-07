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

        public ReactiveCommand<string, Unit> ChangeVisibilityCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public UserSettingsViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            DeletionConfirmation = false;

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
                var CheckPassword = await ServerConnection.CheckCredentials(MainWindowContent!.User!.Email!, Password!);

                if (CheckPassword != "Unauthorized")
                {
                    await ServerConnection.DeleteUser(CheckPassword!);
                    MainWindowContent!.WipeData();
                    MainWindowContent!.ToRegister();
                }
            }
            catch (ServiceUnavailableException)
            {

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
