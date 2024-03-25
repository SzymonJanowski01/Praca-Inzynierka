using LeagueOfLegendsScenarioCreator.Models;
using LeagueOfLegendsScenarioCreator.Services;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    /// <summary>
    /// Class responsible for controlling which ViewModel is in use, and globally storing user and scenario data.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        [Reactive] public ViewModelBase? Content { get; set; }
        [Reactive] public User? User { get; set; }
        [Reactive] public Scenario? Scenario { get; set; }

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

        public void ToScenarioPresenter()
        {
            Content = new ScenarioPresenterViewModel(this);
        }

        public void ToScenarioEditor()
        {
            Content = new ScenarioEditorViewModel(this);
        }

        public void ToUserSettings()
        {
            Content = new UserSettingsViewModel(this);
        }

        public void WipeData()
        {
            User = null;
            Scenario = null;
        }

        public void LogOut()
        {
            WipeData();
            ToLogin();
        }
    }
}
