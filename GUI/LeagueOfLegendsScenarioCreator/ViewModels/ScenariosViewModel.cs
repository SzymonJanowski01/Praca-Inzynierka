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

        public ReactiveCommand<Unit, Unit> AddScenarioCommand { get; private set; }

        public ReactiveCommand<string, Unit> ChangePageCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> OpenScenarioCommand { get; private set; }


        public ScenariosViewModel(MainWindowViewModel? mainWindowContent)
        {
            MainWindowContent = mainWindowContent;
            Filter = string.Empty;
            CurrentPage = 1;
            TotalPages = (int)Math.Ceiling((double)MainWindowContent!.User!.ScenariosNames!.Count / 10);
            NameInput = string.Empty;

            AddScenarioCommand = ReactiveCommand.Create(AddScenario);
            ChangePageCommand = ReactiveCommand.Create<string>(ChangePage);
            OpenScenarioCommand = ReactiveCommand.Create(OpenScenario);
        }

        public async void AddScenario()
        {
            await ServerConnection.CreateScenario(MainWindowContent!.User!.UserId!, NameInput);

            MainWindowContent!.User!.ScenariosNames = await ServerConnection.GetUserScenariosNames(MainWindowContent!.User!.UserId!);
            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

            MainWindowContent!.ToScenarios();
        }

        public async void ChangePage(string parameter)
        {

            CurrentPage = parameter switch
            {
                "p" => CurrentPage - 1,
                "n" => CurrentPage + 1,
                _ when int.TryParse(parameter, out int pageNumber) && pageNumber >= 1 && pageNumber <= TotalPages => pageNumber,
                _ => CurrentPage
            };

            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, (CurrentPage * 10) - 10, null, string.IsNullOrEmpty(Filter) ? null : Filter);
        }

        public void OpenScenario()
        {
            MainWindowContent!.Scenario!.ScenarioId = SelectedItem!.ScenarioId!;

            MainWindowContent!.ToScenarios();
        }
    }
}
