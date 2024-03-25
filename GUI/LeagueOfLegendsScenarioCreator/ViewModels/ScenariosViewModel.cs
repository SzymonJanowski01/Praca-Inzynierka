using LeagueOfLegendsScenarioCreator.CustomExceptions;
using LeagueOfLegendsScenarioCreator.Models;
using LeagueOfLegendsScenarioCreator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    /// <summary>
    /// Class responsible for providing user with list of scenarios, and managing them.
    /// </summary>
    public class ScenariosViewModel : ViewModelBase
    {
        [Reactive] public MainWindowViewModel? MainWindowContent { get; set; }
        [Reactive] public Scenario? SelectedItem { get; set; }
        [Reactive] public string NameInput { get; set; }
        [Reactive] public string? Filter { get; set; }
        [Reactive] public int? CurrentPage { get; set; }
        [Reactive] public int? TotalPages { get; set; }
        [Reactive] public bool ScenarioCreationLock { get; set; }

        public ReactiveCommand<Unit, Unit> AddScenarioCommand { get; private set; }
        public ReactiveCommand<string, Unit> ChangePageCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> OpenScenarioPresenterCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> OpenScenarioEditorCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeleteScenarioCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UserSettingsCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public ScenariosViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            Filter = string.Empty;
            CurrentPage = 1;
            TotalPages = MainWindowContent!.User!.ScenariosNames! != null ? (int)Math.Ceiling((double)MainWindowContent!.User!.ScenariosNames!.Count / 5) : 1;
            NameInput = string.Empty;
            ScenarioCreationLock = false;
            SelectedItem = null;

            AddScenarioCommand = ReactiveCommand.Create(AddScenario);
            ChangePageCommand = ReactiveCommand.Create<string>(ChangePage);
            OpenScenarioPresenterCommand = ReactiveCommand.Create(OpenScenarioPresenter);
            OpenScenarioEditorCommand = ReactiveCommand.Create(OpenScenarioEditor);
            DeleteScenarioCommand = ReactiveCommand.Create(DeleteScenario);
            UserSettingsCommand = ReactiveCommand.Create(UserSettings);
            LogoutCommand = ReactiveCommand.Create(Logout);
        }

        /// <summary>
        /// Creates new scenario, and adds it to user's scenarios list.
        /// Also creates empty phases for scenario.
        /// Blocks user from creating multiple scenarios until the process of creating one is finished.
        /// </summary>
        public async void AddScenario()
        {
            ScenarioCreationLock = true;

            var scenario = await ServerConnection.CreateScenario(MainWindowContent!.User!.UserId!, NameInput);
            await ServerConnection.CreateEmptyPhases(scenario!.ScenarioId!);

            MainWindowContent!.User!.ScenariosNames = await ServerConnection.GetUserScenariosNames(MainWindowContent!.User!.UserId!);
            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

            MainWindowContent!.ToScenarios();
            ScenarioCreationLock = false;
        }

        /// <summary>
        /// Changes page of scenarios list.
        /// Used also for filtering scenarios list.
        /// </summary>
        /// <param name="parameter"> <c>p</c> for previous page, <c>n</c> for next page.</param>
        public async void ChangePage(string parameter)
        {

            CurrentPage = parameter switch
            {
                "p" when CurrentPage > 1 => CurrentPage - 1,
                "n" when CurrentPage < TotalPages => CurrentPage + 1,
                _ => CurrentPage
            };

            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, (CurrentPage * 5) - 5, null, string.IsNullOrEmpty(Filter) ? null : Filter);
            TotalPages = string.IsNullOrEmpty(Filter) ? (int)Math.Ceiling((double)MainWindowContent!.User!.ScenariosNames!.Count / 5) : (int)Math.Ceiling((double)MainWindowContent!.User!.Scenarios!.Count / 5);
        }

        /// <summary>
        /// Redirects to <see cref="ScenarioPresenterViewModel"/>.
        /// </summary>
        public async void OpenScenarioPresenter()
        {
            MainWindowContent!.Scenario = SelectedItem;
            MainWindowContent!.Scenario!.Phases = await ServerConnection.GetScenarioPhases(SelectedItem!.ScenarioId!);

            MainWindowContent!.ToScenarioPresenter();
        }

        /// <summary>
        /// Redirects to <see cref="ScenarioEditorViewModel"/>.
        /// </summary>
        public async void OpenScenarioEditor()
        {
            MainWindowContent!.Scenario = SelectedItem;
            MainWindowContent!.Scenario!.Phases = await ServerConnection.GetScenarioPhases(SelectedItem!.ScenarioId!);

            MainWindowContent!.ToScenarioEditor();
        }

        // TODO: exception handling and empty list handling
        /// <summary>
        /// Deletes selected scenario from user's scenarios list and also from server.
        /// </summary>
        public async void DeleteScenario()
        {
            try
            {
                await ServerConnection.DeleteScenario(SelectedItem!.ScenarioId!);

                MainWindowContent!.User!.ScenariosNames!.Remove(SelectedItem!.ScenarioName!);
                MainWindowContent!.User!.Scenarios!.Remove(SelectedItem!);

                MainWindowContent!.ToScenarios();
            }
            catch(NotFoundException)
            {

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Redirects to <see cref="UserSettingsViewModel"/>.
        /// </summary>
        public void UserSettings()
        {
            MainWindowContent!.ToUserSettings();
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
