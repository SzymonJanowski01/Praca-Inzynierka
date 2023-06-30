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
        public string? Username { get; set; }
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

        }
    }
}
