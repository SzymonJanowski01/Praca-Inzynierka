using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LeagueOfLegendsScenarioCreator.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.Models
{
    public class PhaseProjector : ReactiveObject
    {
        public PhaseProjector(string MCName, string FAName, string SAName)
        {
            MCImage = LoadImage("Splashes", MCName, "jpg");
            FirstAltCImage = LoadImage("Icons", FAName, "png");
            SecAltCImage = LoadImage("Icons", SAName, "png");
        }

        [Reactive]
        public Bitmap? MCImage { get; set; }

        [Reactive]
        public Bitmap? FirstAltCImage { get; set; }

        [Reactive]
        public Bitmap? SecAltCImage { get; set; }

        private static Bitmap LoadImage(string type, string name, string extension)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            string uri = $"avares://LeagueOfLegendsScenarioCreator/Assets/Champions-{type}/{name}.{extension}";

            return new(assets?.Open(new Uri(uri)));
        }
    }
}