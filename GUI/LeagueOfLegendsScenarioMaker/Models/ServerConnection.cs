using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioMaker.Models
{
    public static class ServerConnection
    {
        private readonly static string _baseAddress = "127.0.0.1";

        private static readonly HttpClient sharedClient = new() { BaseAddress = new Uri(_baseAddress) };

        #region User
        #endregion
        #region Scenario
        #endregion
        #region Phase
        #endregion
    }
}
