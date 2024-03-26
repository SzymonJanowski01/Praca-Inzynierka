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
        [Reactive] public MainWindowViewModel? MainWindowContent { get; set; }
        [Reactive] public int? UniformGridHeight { get; set; }
        [Reactive] public string? Filter { get; set; }
        [Reactive] public Dictionary<int, string>? OriginalPhaseChampions { get; set; }
        [Reactive] public Dictionary<int, string>? NewPhaseChampions { get; set; }
        [Reactive] public ObservableCollection<ImageLoader> RequestedChampions { get; set; }
        [Reactive] public ObservableCollection<PhaseProjector> BluePhases { get; set; }
        [Reactive] public ObservableCollection<PhaseProjector> RedPhases { get; set; }
        [Reactive] public Bitmap? SelectedImage { get; set; }
        [Reactive] public string? SelectedChampionName { get; set; }
        [Reactive] public ObservableCollection<string> PhaseChampionsNames { get; set; }
        [Reactive] public Dictionary<string, ObservableCollection<string>> Recommendations { get; set; }
        [Reactive] public ObservableCollection<PhaseProjector> LPLChampions { get; set; }
        [Reactive] public ObservableCollection<PhaseProjector> LCKChampions { get; set; }
        [Reactive] public ObservableCollection<PhaseProjector> LECChampions { get; set; }
        [Reactive] public ObservableCollection<PhaseProjector> LCSChampions { get; set; }

        public ReactiveCommand<ImageLoader, Unit> SelectChampionImageCommand { get; private set; }
        public ReactiveCommand<PhaseProjector, Unit> SelectMCCommand { get; private set; }
        public ReactiveCommand<PhaseProjector, Unit> SelectFACommand { get; private set; }
        public ReactiveCommand<PhaseProjector, Unit> SelectSACommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAndExitCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UserSettingsCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

        public ScenarioEditorViewModel(MainWindowViewModel? mainViewModel)
        {
            MainWindowContent = mainViewModel;
            SelectedImage = null;
            Filter = string.Empty;
            OriginalPhaseChampions = GetOriginalPhaseChampions();
            NewPhaseChampions = new Dictionary<int, string>();

            BluePhases = new ObservableCollection<PhaseProjector>(MatchChampionsNames(true));
            RedPhases = new ObservableCollection<PhaseProjector>(MatchChampionsNames(false));
            RequestedChampions = new ObservableCollection<ImageLoader>(GetChampionsImages()!);
            PhaseChampionsNames = GetPhaseChampionsNames()!;
            Recommendations = new Dictionary<string, ObservableCollection<string>>();

            LPLChampions = new ObservableCollection<PhaseProjector>();
            LCKChampions = new ObservableCollection<PhaseProjector>();
            LECChampions = new ObservableCollection<PhaseProjector>();
            LCSChampions = new ObservableCollection<PhaseProjector>();

            UniformGridHeight = 75 * ((RequestedChampions.Count + 5) / 6);

            SelectChampionImageCommand = ReactiveCommand.Create<ImageLoader>(SelectChampionImage);
            SelectMCCommand = ReactiveCommand.Create<PhaseProjector>(SelectMC);
            SelectFACommand = ReactiveCommand.Create<PhaseProjector>(SelectFA);
            SelectSACommand = ReactiveCommand.Create<PhaseProjector>(SelectSA);
            SaveCommand = ReactiveCommand.Create(Save);
            SaveAndExitCommand = ReactiveCommand.Create(SaveAndExit);
            ExitCommand = ReactiveCommand.Create(Exit);
            UserSettingsCommand = ReactiveCommand.Create(UserSettings);
            LogoutCommand = ReactiveCommand.Create(Logout);

        }

        public Dictionary<int, string> GetOriginalPhaseChampions()
        {
            var originalPhaseChampions = new Dictionary<int, string>();

            var index = 1;
            foreach(var phase in MainWindowContent!.Scenario!.Phases!)
            {
                originalPhaseChampions.Add(index, phase.MainCharacter!);
                index++;
                originalPhaseChampions.Add(index, phase.FirstAlternaticeCharacter!);
                index++;
                originalPhaseChampions.Add(index, phase.SecondAlternaticeCharacter!);
                index++;
            }

            return originalPhaseChampions;
        }

        public ObservableCollection<string> GetPhaseChampionsNames()
        {
            var phaseChampionsNames = new ObservableCollection<string>();
            foreach (var phase in MainWindowContent!.Scenario!.Phases!)
            {
                phaseChampionsNames.Add(phase.MainCharacter!);
            }

            return phaseChampionsNames;
        }

        public IEnumerable<PhaseProjector> MatchChampionsNames(bool firstFive)
        {
            IEnumerable<Phase> selectedPhases = firstFive ? MainWindowContent!.Scenario!.Phases!.Take(5) : MainWindowContent!.Scenario!.Phases!.Skip(5);

            return selectedPhases.Select(phase => new PhaseProjector(new Image("Icons", phase!.MainCharacter!, "png"),
                                                                     new Image("Icons", phase!.FirstAlternaticeCharacter!, "png"),
                                                                     new Image("Icons", phase!.SecondAlternaticeCharacter!, "png"),
                                                                     phase.PhaseName!));
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
             "Sivir", "Skarner", "Smolder", "Sona", "Soraka",
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
            RequestedChampions = GetChampionsImages()!;
            UniformGridHeight = 75 * ((RequestedChampions.Count + 5) / 6);
        }

        public void SelectChampionImage(ImageLoader champion)
        {
            SelectedImage = champion.ChampionImage;
            SelectedChampionName = champion.Name;
        }
        
        public async void GetRecommendations(string TargetPosition)
        {
            Recommendations = await ServerConnection.GetRecommendations(MainWindowContent!.User!.UserId!, GetPhaseChampionsNames()!, TargetPosition);

            LPLChampions.Clear();
            LCKChampions.Clear();
            LECChampions.Clear();
            LCSChampions.Clear();

            foreach (var champion in Recommendations["cn"])
            {
                LPLChampions.Add(new PhaseProjector(new Image("Splashes", champion, "jpg"), null, null, "LPL"));
            }
            foreach (var champion in Recommendations["kr"])
            {
                LCKChampions.Add(new PhaseProjector(new Image("Splashes", champion, "jpg"), null, null, "LCK"));
            }
            foreach (var champion in Recommendations["eu"])
            {
                LECChampions.Add(new PhaseProjector(new Image("Splashes", champion, "jpg"), null, null, "LEC"));
            }
            foreach (var champion in Recommendations["na"])
            {
                LCSChampions.Add(new PhaseProjector(new Image("Splashes", champion, "jpg"), null, null, "LCS"));
            }
        }

        public static string ExpandPhaseName(string phaseName)
        {
            if (phaseName[0] == 'B')
            {
                return $"{phaseName[0]}lue_Champion_{phaseName[1]}";
            }
            else
            {
                return $"{phaseName[0]}ed_Champion_{phaseName[1]}";
            }
            
        }

        public void SelectMC(PhaseProjector phase)
        {
            var phaseName = phase.PhaseName;
            var full_name = ExpandPhaseName(phaseName);

            if (SelectedImage == null || SelectedChampionName == null)
            {
                GetRecommendations(full_name);

                return;
            }
            
            ReplaceChampion(phaseName, "MainCharacter");

            GetRecommendations(full_name);
        }

        public void SelectFA(PhaseProjector phase)
        {
            var phaseName = phase.PhaseName;
            var full_name = ExpandPhaseName(phaseName);

            if (SelectedImage == null || SelectedChampionName == null)
            {
                GetRecommendations(full_name);

                return;
            }

            ReplaceChampion(phaseName, "FirstAlternative");

            GetRecommendations(full_name);
        }

        public void SelectSA(PhaseProjector phase)
        {
            var phaseName = phase.PhaseName;
            var full_name = ExpandPhaseName(phaseName);

            if (SelectedImage == null || SelectedChampionName == null)
            {
                GetRecommendations(full_name);

                return;
            }

            ReplaceChampion(phaseName, "SecondAlternative");

            GetRecommendations(full_name);
        }

        public void ReplaceChampion(string phaseName, string champion)
        {
            if (phaseName[0] == 'B')
            {
                var phase = BluePhases.Where(phase => phase.PhaseName == phaseName).FirstOrDefault();
                if (champion == "MainCharacter")
                {
                    phase!.MCImage = SelectedImage;
                    var index = 1 + (BluePhases.IndexOf(phase) * 3);
                    RecordChange(index, SelectedChampionName!);
                }
                else if (champion == "FirstAlternative")
                {
                    phase!.FirstAltCImage = SelectedImage;
                    var index = 2 + (BluePhases.IndexOf(phase) * 3);
                    RecordChange(index, SelectedChampionName!);
                }
                else if (champion == "SecondAlternative")
                {
                    phase!.SecAltCImage = SelectedImage;
                    var index = 3 + (BluePhases.IndexOf(phase) * 3);
                    RecordChange(index, SelectedChampionName!);
                }
            }
            else
            {
                var phase = RedPhases.Where(phase => phase.PhaseName == phaseName).FirstOrDefault();
                if (champion == "MainCharacter")
                {
                    phase!.MCImage = SelectedImage;
                    var index = 16 + (RedPhases.IndexOf(phase) * 3);
                    RecordChange(index, SelectedChampionName!);
                }
                else if (champion == "FirstAlternative")
                {
                    phase!.FirstAltCImage = SelectedImage;
                    var index = 17 + (RedPhases.IndexOf(phase) * 3);
                    RecordChange(index, SelectedChampionName!);
                }
                else if (champion == "SecondAlternative")
                {
                    phase!.SecAltCImage = SelectedImage;
                    var index = 18 + (RedPhases.IndexOf(phase) * 3);
                    RecordChange(index, SelectedChampionName!);
                }
            }   
        }

        public void RecordChange(int index, string championName)
        {
            if (NewPhaseChampions!.ContainsKey(index))
            {
                NewPhaseChampions[index] = championName;
            }
            else
            {
                NewPhaseChampions!.Add(index, championName);
            }
        }

        public async void Save()
        {
            if (NewPhaseChampions!.Count == 0)
            {
                return;
            }
            else
            {
                bool changesMatchOriginal = NewPhaseChampions.All(pair =>
                {
                    if (OriginalPhaseChampions!.TryGetValue(pair.Key, out string? originalChampion))
                    {
                        return pair.Value == originalChampion;
                    }
                    else
                    {
                        return false;
                    }
                });

                if (changesMatchOriginal)
                {
                    return;
                }
                else
                {
                    try
                    {
                        var result = await ServerConnection.UpdateScenarioPhases(MainWindowContent!.Scenario!.ScenarioId!, NewPhaseChampions!);
                        MainWindowContent!.Scenario!.Phases = await ServerConnection.GetScenarioPhases(MainWindowContent!.Scenario!.ScenarioId!);
                        NewPhaseChampions = new Dictionary<int, string>();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{ex.Message}");
                    }
                }      
            }
        }

        public void SaveAndExit()
        {
            Save();
            Exit();
        }

        public async void Exit()
        {
            if (NewPhaseChampions!.Count == 0)
            {
                MainWindowContent!.Scenario = null;
                MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

                MainWindowContent!.ToScenarios();
            }
            else
            {
                bool changesMatchOriginal = NewPhaseChampions.All(pair =>
                {
                    if (OriginalPhaseChampions!.TryGetValue(pair.Key, out string? originalChampion))
                    {
                        return pair.Value == originalChampion;
                    }
                    else
                    {
                        return false;
                    }
                });

                if (changesMatchOriginal)
                {
                    MainWindowContent!.Scenario = null;
                    MainWindowContent!.User!.Scenarios = await ServerConnection.GetUserScenarios(MainWindowContent!.User!.UserId!, null, null, null);

                    MainWindowContent!.ToScenarios();
                }
                else
                {
                    
                }
            }
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
