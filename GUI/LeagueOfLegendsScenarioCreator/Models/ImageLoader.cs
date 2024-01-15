using Avalonia;
using Avalonia.Platform;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;

namespace LeagueOfLegendsScenarioCreator.Models
{
    public class ImageLoader : ReactiveObject
    {
        [Reactive]
        public Bitmap? ChampionImage { get; set; }

        public ImageLoader(string name)
        {
            ChampionImage = LoadImage("Icons", name, "png");
        }

        public static Bitmap LoadImage(string type, string name, string extension)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            string uri = $"avares://LeagueOfLegendsScenarioCreator/Assets/Champions-{type}/{name}.{extension}";

            return new(assets?.Open(new Uri(uri)));
        }
    }
}
