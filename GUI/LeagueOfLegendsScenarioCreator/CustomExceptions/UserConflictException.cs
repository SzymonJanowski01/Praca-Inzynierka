using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.CustomExceptions
{
    public class UserConflictException : Exception
    {
        public UserConflictException() { }

        public UserConflictException(string message) : base(message) { }

        public UserConflictException(string message, Exception innerEx) : base(message, innerEx) { }
    }
}
