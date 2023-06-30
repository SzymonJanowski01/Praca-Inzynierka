using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfLegendsScenarioCreator.Models;

namespace LeagueOfLegendsScenarioCreator.Services
{
    public static class ServerConnection
    {
        private readonly static string _baseAddress = "127.0.0.1";

        private static readonly HttpClient sharedClient = new() { BaseAddress = new Uri(_baseAddress) };

        #region User
        public static async Task<User> GetUser(string UserID)
        {
            using HttpResponseMessage response = await sharedClient.GetAsync($"/api/user/get-user/{UserID}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            User? user = JsonSerializer.Deserialize<User>(json);
            return user!;
        }

        public static async Task CreateUser(string username, string email, string password)
        {
            using StringContent json = new(

                JsonSerializer.Serialize(new
                {
                    username,
                    email,
                    password
                }), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await sharedClient.PostAsync("/api/user/create-user", json);
            response.EnsureSuccessStatusCode();
        }
        #endregion
        #region Scenario
        #endregion
        #region Phase
        #endregion
    }
}
