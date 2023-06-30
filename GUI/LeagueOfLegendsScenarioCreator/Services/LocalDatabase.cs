using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.Services
{
    public static class LocalDatabase
    {
        private static readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOLScenarioCreator");
        private static readonly string _path = Path.Combine(_directory, "database.sqlite");

        private static SqliteConnection GetConnection() => new($"Filename={_path}");

        private static async Task CreateTable(string command)
        {
            using SqliteConnection database = GetConnection();
            await database.OpenAsync();

            SqliteCommand create = new(command, database);
            try
            {
                await create.ExecuteReaderAsync();
            }
            catch (SqliteException) { }
        }

        public static async Task CreateTables()
        {
            string Settings =
                @"CREATE TABLE IF NOT EXISTS Settings(
                    SettingsId INTEGER PRIMARY KEY AUTOINCREMENT, 
					Theme NVARCHAR(255)
				)";

            await CreateTable(Settings);
        }
    }
}
