using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.CustomExceptions
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException() { }

        public ServiceUnavailableException(string message) : base(message) { }

        public ServiceUnavailableException(string message, Exception innerException) : base(message, innerException) { }
    }
}
