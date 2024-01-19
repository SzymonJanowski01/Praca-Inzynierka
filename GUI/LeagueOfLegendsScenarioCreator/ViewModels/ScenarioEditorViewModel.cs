using Avalonia.Media.Imaging;
using DynamicData;
using LeagueOfLegendsScenarioCreator.Models;
using LeagueOfLegendsScenarioCreator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.ViewModels
{
    public class ScenarioEditorViewModel : ViewModelBase
    {
        [Reactive]
        public MainWindowViewModel? MainWindowContent { get; set; }

        [Reactive]
        public int? UniformGridHeight { get; set; }

        [Reactive]
        public string? Filter { get; set; }

        [Reactive]
        public ObservableCollection<ImageLoader> RequestedChampions { get; set; }

        [Reactive]
        public ObservableCollection<PhaseProjector> BluePhases { get; set; }

        [Reactive]
        public ObservableCollection<PhaseProjector> RedPhases { get; set; }

        [Reactive]
        public Bitmap? SelectedImage { get; set; }


        public ReactiveCommand<Bitmap, Unit> SelectChampionImageCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UserSettingsCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public ScenarioEditorViewModel(MainWindowViewModel? mainViewModel)
        {
            MainWindowContent = mainViewModel;
            SelectedImage = null;
            Filter = string.Empty;
            UniformGridHeight = 72 * 28;

            BluePhases = new ObservableCollection<PhaseProjector>(MatchChampionsNames(true));
            RedPhases = new ObservableCollection<PhaseProjector>(MatchChampionsNames(false));
            RequestedChampions = new ObservableCollection<ImageLoader>(GetChampionsImages()!);

            SelectChampionImageCommand = ReactiveCommand.Create<Bitmap>(SelectChampionImage);
            CancelCommand = ReactiveCommand.Create(Cancel);
            UserSettingsCommand = ReactiveCommand.Create(UserSettings);
            LogoutCommand = ReactiveCommand.Create(Logout);

        }

        public IEnumerable<PhaseProjector> MatchChampionsNames(bool firstFive)
        {
            IEnumerable<Phase> selectedPhases = firstFive ? MainWindowContent!.Scenario!.Phases!.Take(5) : MainWindowContent!.Scenario!.Phases!.Skip(5);

            return selectedPhases.Select(phase => new PhaseProjector(new Image("Icons", phase!.MainCharacter!, "png"),
                                                                     new Image("Icons", phase!.FirstAlternaticeCharacter!, "png"),
                                                                     new Image("Icons", phase!.SecondAlternaticeCharacter!, "png")));
        }


        public ObservableCollection<ImageLoader?> GetChampionsImages()
        {
            var championsImages = new ObservableCollection<ImageLoader>();

            List<string> championsNames = new()
            {
             "None",
             "Aatrox", "Ahri", "Akali", "Akshan",
             "Alistar", "Amumu", "Anivia", "Annie",
             "Aphelios", "Ashe", "AurelionSol", "Azir",
             "Bard", "BelVeth", "Blitzcrank", "Brand",
             "Braum", "Briar", "Caitlyn", "Camille",
             "Cassiopeia", "ChoGath", "Corki", "Darius",
             "Diana", "DrMundo", "Draven", "Ekko",
             "Elise", "Evelynn", "Ezreal", "Fiddlesticks",
             "Fiora", "Fizz", "Galio", "Gangplank",
             "Garen", "Gnar", "Gragas", "Graves",
             "Gwen", "Hecarim", "Heimerdinger", "Hwei",
             "Illaoi", "Irelia", "Ivern", "Janna",
             "JarvanIV", "Jax", "Jayce", "Jhin",
             "Jinx", "KSante", "KaiSa", "Kalista",
             "Karma", "Karthus", "Kassadin", "Katarina",
             "Kayle", "Kayn", "Kennen", "KhaZix",
             "Kindred", "Kled", "KogMaw", "LeBlanc",
             "LeeSin", "Leona", "Lillia", "Lissandra",
             "Lucian", "Lulu", "Lux", "Malphite",
             "Malzahar", "Maokai", "MasterYi", "Milio",
             "MissFortune", "Mordekaiser", "Morgana", "Naafiri",
             "Nami", "Nasus", "Nautilus", "Neeko",
             "Nidalee", "Nilah", "Nocturne", "Nunu",
             "Olaf", "Orianna", "Ornn", "Pantheon",
             "Poppy", "Pyke", "Qiyana", "Quinn",
             "Rakan", "Rammus", "RekSai", "Rell",
             "RenataGlasc", "Renekton", "Rengar", "Riven",
             "Rumble", "Ryze", "Samira", "Sejuani",
             "Senna", "Seraphine", "Sett", "Shaco",
             "Shen", "Shyvana", "Singed", "Sion",
             "Sivir", "Skarner", "Sona", "Soraka",
             "Swain", "Sylas", "Syndra", "TahmKench",
             "Taliyah", "Talon", "Taric", "Teemo",
             "Thresh", "Tristana", "Trundle", "Tryndamere",
             "TwistedFate", "Twitch", "Udyr", "Urgot",
             "Varus", "Vayne", "Veigar", "VelKoz",
             "Vex", "Vi", "Viego", "Viktor",
             "Vladimir", "Volibear", "Warwick", "Wukong",
             "Xayah", "Xerath", "XinZhao", "Yasuo",
             "Yone", "Yorick", "Yuumi", "Zac",
             "Zed", "Zeri", "Ziggs", "Zilean",
             "Zoe", "Zyra"
            };

            foreach (var championName in championsNames)
            {
                if(!string.IsNullOrEmpty(Filter) && !championName.Contains(Filter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                championsImages.Add(new ImageLoader(championName));
            }

            return championsImages!;
        }

        public void SearchChampion()
        {
            RequestedChampions = new ObservableCollection<ImageLoader>(GetChampionsImages()!);
            UniformGridHeight = 72 * (RequestedChampions.Count / 6);
        }

        public void SelectChampionImage(Bitmap image)
        {
            SelectedImage = image;
            Debug.WriteLine(SelectedImage);
            Debug.WriteLine("Im here");
        }

        public async void Cancel()
        {
            MainWindowContent!.Scenario = null;
            MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

            MainWindowContent!.ToScenarios();
        }

        public void UserSettings()
        {
            MainWindowContent!.ToUserSettings();
        }

        public void Logout()
        {
            MainWindowContent!.LogOut();
        }
    }
}
