using LeagueOfLegendsScenarioCreator.CustomExceptions;
using LeagueOfLegendsScenarioCreator.Models;
using LeagueOfLegendsScenarioCreator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    public class ScenariosViewModel : ViewModelBase
    {
        [Reactive]
        public MainWindowViewModel? MainWindowContent { get; set; }

        [Reactive]
        public Scenario? SelectedItem { get; set; }

        [Reactive]
        public string NameInput { get; set; }

        [Reactive]
        public string? Filter { get; set; }

        [Reactive]
        public int? CurrentPage { get; set; }

        [Reactive]
        public int? TotalPages { get; set; }

        [Reactive]
        public bool ScenarioCreationLock { get; set; }

        public ReactiveCommand<Unit, Unit> AddScenarioCommand { get; private set; }

        public ReactiveCommand<string, Unit> ChangePageCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> OpenScenarioCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> DeleteScenarioCommand { get; private set; }

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
            OpenScenarioCommand = ReactiveCommand.Create(OpenScenario);
            DeleteScenarioCommand = ReactiveCommand.Create(DeleteScenario);
            LogoutCommand = ReactiveCommand.Create(Logout);
        }

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

        public async void ChangePage(string parameter)
        {

            CurrentPage = parameter switch
            {
                "p" when CurrentPage > 1 => CurrentPage - 1,
                "n" when CurrentPage < TotalPages => CurrentPage + 1,
                _ when int.TryParse(parameter, out int pageNumber) && pageNumber >= 1 && pageNumber <= TotalPages => pageNumber,
                _ => CurrentPage
            };

            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, (CurrentPage * 5) - 5, null, string.IsNullOrEmpty(Filter) ? null : Filter);
        }

        public void OpenScenario()
        {
            MainWindowContent!.Scenario = SelectedItem;

            MainWindowContent!.ToScenario();
        }

        public async void DeleteScenario()
        {
            try
            {
                await ServerConnection.DeleteScenario(SelectedItem!.ScenarioId!);

                MainWindowContent!.User!.ScenariosNames = await ServerConnection.GetUserScenariosNames(MainWindowContent!.User!.UserId!);
                MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

                MainWindowContent!.ToScenarios();
            }
            catch(NotFoundException)
            {

            }
            catch (Exception)
            {

            }
        }

        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
