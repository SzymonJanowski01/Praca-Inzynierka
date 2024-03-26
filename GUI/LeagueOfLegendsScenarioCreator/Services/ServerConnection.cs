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
using LeagueOfLegendsScenarioCreator.CustomExceptions;
using LeagueOfLegendsScenarioCreator.Models;

namespace LeagueOfLegendsScenarioCreator.Services
{
    /// <summary>
    /// Class for handling communication with the server.
    /// </summary>
    public static class ServerConnection
    {
        private readonly static string _baseAddress = "http://127.0.0.1:8000";

        private static readonly HttpClient sharedClient = new() { BaseAddress = new Uri(_baseAddress) };

        #region User
        /// <summary>
        /// Get user from the server.
        /// </summary>
        /// <param name="userId">ID of the user to get.</param>
        /// <returns>
        /// <see cref="User"/> object.
        /// </returns>
        public static async Task<User> GetUser(string userId)
        {
            using HttpResponseMessage response = await sharedClient.GetAsync($"/api/user/get-user/{userId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            User? user = JsonSerializer.Deserialize<User>(json);
            return user!;
        }

        /// <summary>
        /// Create user on the server.
        /// </summary>
        /// <param name="username">Username of the user.</param>
        /// <param name="email">Email of the user.</param>
        /// <param name="password">Password of the user.</param>
        /// <returns>
        /// <see cref="User"/> object.
        /// </returns>
        /// <exception cref="UserConflictException">
        /// Thrown when the username or email is already taken.
        /// </exception>
        /// <exception cref="ServiceUnavailableException">
        /// Thrown when the server is unavailable.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the server returns an unexpected status code.
        /// </exception>
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
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new UserConflictException();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    throw new ServiceUnavailableException();
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

        /// <summary>
        /// Check if the user with the given credentials exists on the server.
        /// </summary>
        /// <param name="usernameOrEmail">Username or email of the user.</param>
        /// <param name="password">Password of the user.</param>
        /// <returns>
        /// <see cref="string"/> with the ID of the user if the user exists.
        /// </returns>
        /// <exception cref="ServiceUnavailableException"></exception>
        /// <exception cref="Exception"></exception>
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
                    throw new NotFoundException();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    throw new ServiceUnavailableException();
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

        /// <summary>
        /// Update user on the server.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="username">New username (optional).</param>
        /// <param name="email">New email (optional).</param>
        /// <param name="password">New password (optional).</param>
        /// <returns>
        /// <see cref="User"/> object with updated fields.
        /// </returns>
        /// <exception cref="UserConflictException">
        /// Thrown when the username or email is already taken.
        /// </exception>
        /// <exception cref="ServiceUnavailableException">
        /// Thrown when the server is unavailable.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the server returns an unexpected status code.
        /// </exception>
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
                    var jsonErrorResponse = await response.Content.ReadAsStringAsync();
                    var errorDetail = JsonDocument.Parse(jsonErrorResponse).RootElement.GetProperty("detail");
                    throw new UserConflictException($"{errorDetail}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    throw new ServiceUnavailableException();
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
        /// <summary>
        /// Delete user from the server.
        /// </summary>
        /// <param name="userId">ID of the user to delete</param>
        /// <returns></returns>
        public static async Task DeleteUser(string userId)
        {
            using HttpResponseMessage response = await sharedClient.DeleteAsync($"/api/user/delete-user/{userId}");
            response.EnsureSuccessStatusCode();
        }
        #endregion
        #region Scenario
        /// <summary>
        /// Get user scenarios from the server.
        /// </summary>
        /// <param name="userId">ID of the user to get scenarios for.</param>
        /// <param name="skip">How many scenarios to omit.</param>
        /// <param name="limit">How many scenarios to get.</param>
        /// <param name="filter"> <paramref name="string"/> that must be included in the returned scenarios names.</param>
        /// <returns>
        /// <see cref="ObservableCollection{T}"/> of <see cref="Scenario"/> objects.
        /// </returns>
        public static async Task<ObservableCollection<Scenario>> GetUserScenarios(string userId, int? skip, int? limit, string? filter)
        {
            string requestUrl = $"/api/scenario/get-user-scenarios/{userId}";

            if (skip.HasValue || limit.HasValue || !string.IsNullOrEmpty(filter))
            {
                var sb = new StringBuilder(requestUrl + "?");

                sb.Append(!string.IsNullOrEmpty(filter) ? $"filter_param={Uri.EscapeDataString(filter)}&" : "");
                sb.Append(skip.HasValue ? $"skip={skip}&" : "");
                sb.Append(limit.HasValue ? $"limit={limit}&" : "");

                requestUrl = sb.ToString().TrimEnd('&');
            }

            using HttpResponseMessage response = await sharedClient.GetAsync(requestUrl);

            var json = await response.Content.ReadAsStringAsync();
            ObservableCollection<Scenario>? scenarios = json.Contains("No scenarios associated with the user.") ? null : JsonSerializer.Deserialize<ObservableCollection<Scenario>>(json);
            return scenarios!;
        }

        /// <summary>
        /// Get scenario names from the server.
        /// </summary>
        /// <param name="userId">ID of the user to get scenarios names for.</param>
        /// <returns>
        /// <see cref="ObservableCollection{T}"/> of scenarios names in a <paramref name="string"/> format.
        /// </returns>
        public static async Task<ObservableCollection<string>> GetUserScenariosNames(string userId)
        {
            using HttpResponseMessage response = await sharedClient.GetAsync($"/api/scenario/get-user-scenarios-names/{userId}");

            var json = await response.Content.ReadAsStringAsync();
            ObservableCollection<string>? scenarios = json.Contains("No scenarios associated with the user.") ? null : JsonSerializer.Deserialize<ObservableCollection<string>>(json);
            return scenarios!;
        }

        /// <summary>
        /// Create scenario on the server.
        /// </summary>
        /// <param name="userId">ID of the user to create scenario for.</param>
        /// <param name="name">Name of the scenario to create.</param>
        /// <returns>
        /// Newly created <see cref="Scenario"/> object.
        /// </returns>
        /// <exception cref="Exception"></exception>
        public static async Task<Scenario?> CreateScenario(string userId, string name)
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

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Scenario? scenario = JsonSerializer.Deserialize<Scenario>(jsonResponse);
            return scenario!;
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
                    throw new NotFoundException();
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

        /// <summary>
        /// Delete scenario from the server.
        /// </summary>
        /// <param name="scenarioId">ID of the scenario to delete.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the scenario is not found.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the server returns an unexpected status code.
        /// </exception>
        public static async Task DeleteScenario(string scenarioId)
        {
            using HttpResponseMessage response = await sharedClient.DeleteAsync($"/api/scenario/delete-scenario/{scenarioId}");
            
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new NotFoundException();
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        #endregion
        #region Phase
        /// <summary>
        /// Get scenario phases from the server.
        /// </summary>
        /// <param name="scenarioId">ID of the scenario to get phases for.</param>
        /// <returns>
        /// <see cref="ObservableCollection{T}"/> of <see cref="Phase"/> objects.
        /// </returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the scenario is not found.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the server returns an unexpected status code.
        /// </exception>
        public static async Task<ObservableCollection<Phase>?> GetScenarioPhases(string scenarioId)
        {
            using HttpResponseMessage response = await sharedClient.GetAsync($"/api/phase/get-scenario-phases/{scenarioId}");
            
            if (response.StatusCode != System.Net.HttpStatusCode.OK) 
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new NotFoundException();
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

        /// <summary>
        /// Create empty phases after scenario creation and commit them to the server.
        /// </summary>
        /// <param name="scenarioId">ID of the scenario to create phases for.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// Thrown when the server returns an unexpected status code.
        /// </exception>
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

        /// <summary>
        /// Get updated phases from ViewModel and send them to the server.
        /// </summary>
        /// <param name="scenarioId">ID of the scenario to perform update on.</param>
        /// <param name="newPhases">Dictionary of indexed changes for the scenario.</param>
        /// <returns>
        /// <paramref name="true"/> for a successful update, <paramref name="false"/> if the request have wrong data.
        /// </returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the scenario is not found.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the server returns an unexpected status code.
        /// </exception>
        public static async Task<bool?> UpdateScenarioPhases (string scenarioId, Dictionary<int, string> newPhases)
        {
            List<object> jsonPhases = new();
            List<int> indexesUsed = new();

            var attributes = new Dictionary<string, string>();
            var storedPhaseIndex = 0;

            foreach (var phase in newPhases)
            {
                // Divide the newPhases dict by 3 to get phaseNumber as single phase can contain up to 3 indexed attributes.
                int phaseNumber = (int)Math.Ceiling((double)phase.Key / 3) - 1;

                // If the phase is the first one, add its attribute to dict and continue.
                if (indexesUsed.Count == 0)
                {
                    indexesUsed.Add(phaseNumber);

                    attributes = MatchKey(attributes!, phase);

                    // If there are no more phases, create a json object and add it to the list.
                    if (phase.Key == newPhases.Keys.Last())
                    {
                        var jsonPhase = new
                        {
                            phase_index = phaseNumber,
                            attributes
                        };

                        jsonPhases.Add(jsonPhase);
                    }

                    storedPhaseIndex = phaseNumber;
                }
                // If the phaseNumber is the same as the previous one, add its content to attributes and continue.
                else if (indexesUsed.Contains(phaseNumber))
                {
                    attributes = MatchKey(attributes!, phase);

                    // If there are no more phases, create a json object and add it to the list.
                    if (phase.Key == newPhases.Keys.Last())
                    {
                        var jsonPhase = new
                        {
                            phase_index = storedPhaseIndex,
                            attributes
                        };

                        jsonPhases.Add(jsonPhase);
                    }
                }
                // If the phaseNumber is different from the previous one, create a json object for the previous phase, and start the creation of new json object.
                else
                {
                    var jsonPhase = new
                    {
                        phase_index = storedPhaseIndex,
                        attributes
                    };

                    jsonPhases.Add(jsonPhase);

                    attributes = new Dictionary<string, string>();
                    indexesUsed.Add(phaseNumber);

                    attributes = MatchKey(attributes!, phase);

                    storedPhaseIndex = phaseNumber;

                    // If there are no more phases, create a json object and add it to the list.
                    if (phase.Key == newPhases.Keys.Last())
                    {
                        var jsonPhaseNew = new
                        {
                            phase_index = storedPhaseIndex,
                            attributes
                        };

                        jsonPhases.Add(jsonPhaseNew);
                    }
                }
            }

            using StringContent json = new(
                JsonSerializer.Serialize(jsonPhases),
                Encoding.UTF8,
                "application/json");

            using HttpResponseMessage response = await sharedClient.PutAsync($"/api/phase/update-scenario-phases/{scenarioId}", json);
            
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) 
                {
                    throw new NotFoundException();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return false;
                }
                else
                {
                    throw new Exception($"Unexpected response status code: {response.StatusCode}");
                }
            }

            return true;
        }
        #endregion

        /// <summary>
        /// Helper method to add correct key-value pair to attribute list.
        /// </summary>
        /// <param name="attr">Old dictionary to undergo addition.</param>
        /// <param name="phase">Phase to get key-value pair from.</param>
        /// <returns>
        /// <see cref="Dictionary{TKey, TValue}"/> with added key-value pair.
        /// </returns>
        private static Dictionary<string, string> MatchKey(Dictionary<string, string?> attr, KeyValuePair<int, string> phase)
        {
            switch (phase.Key % 3)
            {
                case 0:
                    attr.Add("second_alternative_character", phase.Value);
                    break;
                case 1:
                    attr.Add("main_character", phase.Value);
                    break;
                case 2:
                    attr.Add("first_alternative_character", phase.Value);
                    break;
            }

            return attr!;
        }

        #region Recommendations
        /// <summary>
        /// Method to get recommendations from the server.
        /// </summary>
        /// <param name="userId">ID of the user to prevent end-point spamm from unregistered sources.</param>
        /// <param name="championsList">List of champions to provide context for recommendation.</param>
        /// <param name="TargetPosition">Position to get recommendation for.</param>
        /// <returns>
        /// <see cref="Dictionary{TKey, TValue}"/> with added key-value pair 
        /// (Key being the region of the <see cref="string"/> type, value is <see cref="ObservableCollection{T}"/> of <see cref="int"/> typed values).
        /// </returns>
        /// <exception cref="Exception"></exception>
        public static async Task<Dictionary<string, ObservableCollection<string>>> GetRecommendations(string userId, ObservableCollection<string> championsList, string TargetPosition)
        {
            using StringContent json = new(
                JsonSerializer.Serialize(championsList), 
                Encoding.UTF8, 
                "application/json");
            using HttpResponseMessage response = await sharedClient.PostAsync($"/api/phase/recommendations/{userId}?target_position={TargetPosition}", json);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected response status code: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var recommendations = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<string>>>(jsonResponse);
            return recommendations!;
        }
        #endregion
    }
}
