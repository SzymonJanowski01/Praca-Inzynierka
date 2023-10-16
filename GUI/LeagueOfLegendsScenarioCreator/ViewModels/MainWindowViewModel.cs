using LeagueOfLegendsScenarioCreator.Models;
using LeagueOfLegendsScenarioCreator.Services;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        [Reactive]
        public ViewModelBase? Content { get; set; }

        [Reactive]
        public User? User { get; set; }

        [Reactive]
        public Scenario? Scenario { get; set; }

        public MainWindowViewModel() 
        {
            Content = new LoginViewModel(this);
            Task.Run(() => LocalDatabase.CreateTables());
        }

        public void ToLogin()
        {
            Content = new LoginViewModel(this);
        }

        public void ToRegister()
        {
            Content = new RegisterViewModel(this);
        }

        public void ToScenarios()
        {
            Content = new ScenariosViewModel(this);
        }

        public void ToScenario()
        {
            Content = new ScenarioViewModel(this);
        }

        public void LogOut()
        {
            User = null;
            Scenario = null;
            Content = new LoginViewModel(this);
        }
    }
}
