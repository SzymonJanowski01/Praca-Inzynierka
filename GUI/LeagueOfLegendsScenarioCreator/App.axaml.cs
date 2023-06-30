using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LeagueOfLegendsScenarioCreator.ViewModels;
using LeagueOfLegendsScenarioCreator.Views;
using LeagueOfLegendsScenarioCreator.Services;
using System.IO;
using System.Threading.Tasks;
using System;

namespace LeagueOfLegendsScenarioCreator
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            var _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOLScenarioCreator");

            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);
            Task.Run(() => LocalDatabase.CreateTables());

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}