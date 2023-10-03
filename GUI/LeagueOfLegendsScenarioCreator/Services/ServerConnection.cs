using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicData.Diagnostics;
using LeagueOfLegendsScenarioCreator.Models;

namespace LeagueOfLegendsScenarioCreator.Services
{
    public static class ServerConnection
    {
        private readonly static string _baseAddress = "http://127.0.0.1:8000";

        private static readonly HttpClient sharedClient = new() { BaseAddress = new Uri(_baseAddress) };

        #region User
        public static async Task<User> GetUser(string userId)
        {
            using HttpResponseMessage response = await sharedClient.GetAsync($"/api/user/get-user/{userId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            User? user = JsonSerializer.Deserialize<User>(json);
            return user!;
        }

        public static async Task<User?> CreateUser(string username, string email, string password)
        {
            using StringContent json = new(
                JsonSerializer.Serialize(new
                {
                    username,
                    email,
                    password
                }), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await sharedClient.PostAsync("/api/user/create-user", json);
            
            if (response.StatusCode != System.Net.HttpStatusCode.Created) 
            {
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    return null;
                }
                else
                {
                    throw new Exception($"Unexpected response status code: {response.StatusCode}");
                }
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            User? user = JsonSerializer.Deserialize<User> (jsonResponse);
            return user;
        }

        public static async Task<string?> CheckCredentials(string usernameOrEmail, string password)
        {
            using StringContent json = new(
                JsonSerializer.Serialize(new
                {
                    username_or_email = usernameOrEmail,
                    password
                }), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await sharedClient.PostAsync($"/api/user/check-credentials", json);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "NotFound";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return "Unauthorized";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    return "Server not available";
                }
                else
                {
                    throw new Exception($"Unexpected response status code: {response.StatusCode}");
                }
            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                string? userId = JsonSerializer.Deserialize<string?>(jsonResponse);
                return userId;
            }        
        }

        public static async Task<User?> UpdateUser(string userId, string? username, string? email, string? password)
        {
            using StringContent json = new(
                JsonSerializer.Serialize(new
                {
                    username,
                    email,
                    password
                }), Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await sharedClient.PutAsync($"/api/user/update-user/{userId}", json);
            
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return null;
                }
                else
                {
                    throw new Exception($"Unexpected response status code: {response.StatusCode}");
                }
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            User? user = JsonSerializer.Deserialize<User>(jsonResponse);
            return user!;
        }

        public static async Task DeleteUser(string userId)
        {
            using HttpResponseMessage response = await sharedClient.DeleteAsync($"/api/user/delete-user/{userId}");
            response.EnsureSuccessStatusCode();
        }
        #endregion
        #region Scenario
        public static async Task<ObservableCollection<Scenario>> GetUserScenarios(string userId, int? skip, int? limit, string? filter)
        {
            string requestUrl = $"/api/scenario/get-user-scenarios/{userId}";

            if (skip.HasValue || limit.HasValue || !string.IsNullOrEmpty(filter))
            {
                var sb = new StringBuilder(requestUrl + "?");

                sb.Append(!string.IsNullOrEmpty(filter) ? $"filter={Uri.EscapeDataString(filter)}&" : "");
                sb.Append(skip.HasValue ? $"skip={skip}&" : "");
                sb.Append(limit.HasValue ? $"limit={limit}&" : "");

                requestUrl = sb.ToString().TrimEnd('&');
            }

            using HttpResponseMessage response = await sharedClient.GetAsync(requestUrl);

            var json = await response.Content.ReadAsStringAsync();
            ObservableCollection<Scenario>? scenarios = json.Contains("No scenarios associated with the user.") ? null : JsonSerializer.Deserialize<ObservableCollection<Scenario>>(json);
            return scenarios!;
        }

        public static async Task<ObservableCollection<string>> GetUserScenariosNames(string userId)
        {
            using HttpResponseMessage response = await sharedClient.GetAsync($"/api/scenario/get-user-scenarios-names/{userId}");

            var json = await response.Content.ReadAsStringAsync();
            ObservableCollection<string>? scenarios = json.Contains("No scenarios associated with the user.") ? null : JsonSerializer.Deserialize<ObservableCollection<string>>(json);
            return scenarios!;
        }

        public static async Task CreateScenario(string userId, string name)
        {
            using StringContent json = new(
                JsonSerializer.Serialize(new
                {
                    name
                }), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await sharedClient.PostAsync($"/api/scenario/create-scenario/{userId}", json);
            
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new Exception($"Unexpected response status code: {response.StatusCode}");
            }
        }

        public static async Task<Scenario?> UpdateScenario(string scenarioId, string name)
        {
            using StringContent json = new(
                JsonSerializer.Serialize(new
                {
                    name
                }), Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await sharedClient.PutAsync($"/api/scenario/update-scenario/{scenarioId}", json);
            
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) 
                {
                    return null;
                }
                else
                {
                    throw new Exception($"Unexpected response status code: {response.StatusCode}");
                }
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Scenario? scenario = JsonSerializer.Deserialize<Scenario>(jsonResponse);
            return scenario!;
        }

        public static async Task DeleteScenario(string scenarioId)
        {
            using HttpResponseMessage response = await sharedClient.DeleteAsync($"/api/scenario/delete-scenario/{scenarioId}");
            response.EnsureSuccessStatusCode();
        }
        #endregion
        #region Phase
        public static async Task<ObservableCollection<Phase>?> GetScenarioPhases(string scenarioId)
        {
            using HttpResponseMessage response = await sharedClient.GetAsync($"/api/phase/get-scenario-phases/{scenarioId}");
            
            if (response.StatusCode != System.Net.HttpStatusCode.OK) 
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw new Exception($"Unexpected response status code: {response.StatusCode}");
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            ObservableCollection<Phase>? phases = JsonSerializer.Deserialize<ObservableCollection<Phase>>(json);
            return phases!;
        }

        public static async Task<ObservableCollection<string>> CreateEmptyPhases(string scenarioId)
        {
            using HttpResponseMessage response = await sharedClient.PostAsync($"/api/phase/create-empty-phases/{scenarioId}", null);
            
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new Exception($"Unexpected response status code: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            ObservableCollection<string>? phasesIds = JsonSerializer.Deserialize<ObservableCollection<string>>(jsonResponse);
            return phasesIds!;
        }

        public static async Task UpdateScenarioPhases (string scenarioId, List<Phase> phases)
        {
            List<object> jsonPhases = new();

            foreach (var phase in phases)
            {
                var attributes = new Dictionary<string, string?>
                {
                    { "main_character", string.IsNullOrEmpty(phase.MainCharacter) ? null : phase.MainCharacter },
                    { "first_alternative_character", string.IsNullOrEmpty(phase.FirstAlternaticeCharacter) ? null : phase.FirstAlternaticeCharacter},
                    { "second_alternative_character", string.IsNullOrEmpty(phase.SecondAlternaticeCharacter) ? null : phase.SecondAlternaticeCharacter}
                };

                int phaseNumber = phase.PhaseName!.StartsWith("B") 
                    ? int.Parse(phase.PhaseName[1..]) - 1
                    : int.Parse(phase.PhaseName[1..]) + 4;

                var jsonPhase = new
                {
                    phase_number = phaseNumber,
                    attributes
                };

                jsonPhases.Add(jsonPhase);
            }

            using StringContent json = new(
                JsonSerializer.Serialize(new
                {
                    jsonPhases
                }), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await sharedClient.PutAsync($"/api/phase/update-scenario-phases/{scenarioId}", json);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
