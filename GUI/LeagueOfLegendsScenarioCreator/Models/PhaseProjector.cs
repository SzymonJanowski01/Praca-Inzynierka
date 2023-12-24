using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LeagueOfLegendsScenarioCreator.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.Models
{
    public class Image
    {
        public string Localisation { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public Image(string localisation, string name, string extension)
        {
            Localisation = localisation;
            Name = name;
            Extension = extension;
        }
    }

    public class PhaseProjector : ReactiveObject
    {
        public PhaseProjector(Image image1, Image image2, Image image3)
        {
            MCImage = LoadImage(image1.Localisation, image1.Name, image1.Extension);
            FirstAltCImage = LoadImage(image2.Localisation, image2.Name, image2.Extension);
            SecAltCImage = LoadImage(image3.Localisation, image3.Name, image3.Extension);
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