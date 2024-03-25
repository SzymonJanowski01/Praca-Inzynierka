using LeagueOfLegendsScenarioCreator.Models;
using LeagueOfLegendsScenarioCreator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    /// <summary>
    /// Class responsible for presenting scenario to user
    /// </summary>
    public class ScenarioPresenterViewModel : ViewModelBase
    {
        [Reactive] public MainWindowViewModel? MainWindowContent { get; set; }
        [Reactive] public ObservableCollection<PhaseProjector> BluePhases { get; set; }
        [Reactive] public ObservableCollection<PhaseProjector> RedPhases { get; set; }

        public ReactiveCommand<Unit, Unit> BackCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UserSettingsCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public ScenarioPresenterViewModel(MainWindowViewModel? mainViewModel)
        {
            MainWindowContent = mainViewModel;
            BluePhases = new ObservableCollection<PhaseProjector>(MatchChampionsNames(true));
            RedPhases = new ObservableCollection<PhaseProjector>(MatchChampionsNames(false));

            BackCommand = ReactiveCommand.Create(Back);
            UserSettingsCommand = ReactiveCommand.Create(UserSettings);
            LogoutCommand = ReactiveCommand.Create(Logout);
        }

        /// <summary>
        /// Matches champions names from server with images from local assets, and return them in <see cref="PhaseProjector"/> form.
        /// </summary>
        /// <param name="firstFive">Server returns 10 champions, they should be divided into two 5 champions each portion. The bool represents which portion should be returned.</param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="PhaseProjector"/> containing images and names of champions.
        /// </returns>
        public IEnumerable<PhaseProjector> MatchChampionsNames(bool firstFive)
        {
            IEnumerable<Phase> selectedPhases = firstFive ? MainWindowContent!.Scenario!.Phases!.Take(5) : MainWindowContent!.Scenario!.Phases!.Skip(5);

            return selectedPhases.Select(phase => new PhaseProjector(new Image("Splashes", phase!.MainCharacter!, "jpg"),
                                                                     new Image("Icons", phase!.FirstAlternaticeCharacter!, "png"),
                                                                     new Image("Icons", phase!.SecondAlternaticeCharacter!, "png"),
                                                                     phase.PhaseName!));
        }

        /// <summary>
        /// Returns to <see cref="ScenariosViewModel"/>.
        /// </summary>
        public async void Back()
        {
            MainWindowContent!.Scenario = null;
            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

            MainWindowContent!.ToScenarios();
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
