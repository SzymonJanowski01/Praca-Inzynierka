using LeagueOfLegendsScenarioCreator.CustomExceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUnitTests
{
    [TestClass]
    public class NotFoundExceptionTests
    {
        [TestMethod]
        public void Constructor_DefaultConstructor_MessageIsNull()
        {
            // Arrange
            var exception = new NotFoundException();

            // Act
            var message = exception.Message;
            Debug.WriteLine(message);

            // Assert
            Assert.AreEqual(message, "Exception of type 'LeagueOfLegendsScenarioCreator.CustomExceptions.NotFoundException' was thrown.");
        }

        [TestMethod]
        public void Constructor_MessageConstructor_MessageIsSet()
        {
            // Arrange
            var errorMessage = "NotFound";
            var exception = new NotFoundException(errorMessage);

            // Act
            var message = exception.Message;

            // Assert
            Assert.AreEqual(errorMessage, message);
        }

        [TestMethod]
        public void Constructor_InnerExceptionConstructor_MessageAndInnerExceptionAreSet()
        {
            // Arrange
            var errorMessage = "NotFound";
            var innerException = new Exception("Inner exception");
            var exception = new NotFoundException(errorMessage, innerException);

            // Act
            var message = exception.Message;
            var innerEx = exception.InnerException;

            // Assert
            Assert.AreEqual(errorMessage, message);
            Assert.AreEqual(innerException, innerEx);
        }
    }

    [TestClass]
    public class ServiceUnavailableExceptionTests
    {
        [TestMethod]
        public void Constructor_DefaultConstructor_MessageIsNull()
        {
            // Arrange
            var exception = new ServiceUnavailableException();

            // Act
            var message = exception.Message;

            // Assert
            Assert.AreEqual(message, "Exception of type 'LeagueOfLegendsScenarioCreator.CustomExceptions.ServiceUnavailableException' was thrown.");
        }

        [TestMethod]
        public void Constructor_MessageConstructor_MessageIsSet()
        {
            // Arrange
            var errorMessage = "Service is unavailable";
            var exception = new ServiceUnavailableException(errorMessage);

            // Act
            var message = exception.Message;

            // Assert
            Assert.AreEqual(errorMessage, message);
        }

        [TestMethod]
        public void Constructor_InnerExceptionConstructor_MessageAndInnerExceptionAreSet()
        {
            // Arrange
            var errorMessage = "Service is unavailable";
            var innerException = new Exception("Inner exception");
            var exception = new ServiceUnavailableException(errorMessage, innerException);

            // Act
            var message = exception.Message;
            var innerEx = exception.InnerException;

            // Assert
            Assert.AreEqual(errorMessage, message);
            Assert.AreEqual(innerException, innerEx);
        }
    }

    [TestClass]
    public class UserConflictExceptionTests
    {
        [TestMethod]
        public void Constructor_DefaultConstructor_MessageIsNull()
        {
            // Arrange
            var exception = new UserConflictException();

            // Act
            var message = exception.Message;
            
            // Assert
            Assert.AreEqual(message, "Exception of type 'LeagueOfLegendsScenarioCreator.CustomExceptions.UserConflictException' was thrown.");
        }

        [TestMethod]
        public void Constructor_MessageConstructor_MessageIsSet()
        {
            // Arrange
            var errorMessage = "UserConflict";
            var exception = new UserConflictException(errorMessage);

            // Act
            var message = exception.Message;

            // Assert
            Assert.AreEqual(errorMessage, message);
        }

        [TestMethod]
        public void Constructor_InnerExceptionConstructor_MessageAndInnerExceptionAreSet()
        {
            // Arrange
            var errorMessage = "UserConflict";
            var innerException = new Exception("Inner exception");
            var exception = new UserConflictException(errorMessage, innerException);

            // Act
            var message = exception.Message;
            var innerEx = exception.InnerException;

            // Assert
            Assert.AreEqual(errorMessage, message);
            Assert.AreEqual(innerException, innerEx);
        }
    }

    [TestClass]
    public class UnauthorizedExceptionTests
    {
        [TestMethod]
        public void Constructor_DefaultConstructor_MessageIsNull()
        {
            // Arrange
            var exception = new UnauthorizedException();

            // Act
            var message = exception.Message;

            // Assert
            Assert.AreEqual(message, "Exception of type 'LeagueOfLegendsScenarioCreator.CustomExceptions.UnauthorizedException' was thrown.");
        }

        [TestMethod]
        public void Constructor_MessageConstructor_MessageIsSet()
        {
            // Arrange
            var errorMessage = "Unauthorized";
            var exception = new UnauthorizedException(errorMessage);

            // Act
            var message = exception.Message;

            // Assert
            Assert.AreEqual(errorMessage, message);
        }

        [TestMethod]
        public void Constructor_InnerExceptionConstructor_MessageAndInnerExceptionAreSet()
        {
            // Arrange
            var errorMessage = "Unauthorized";
            var innerException = new Exception("Inner exception");
            var exception = new UnauthorizedException(errorMessage, innerException);

            // Act
            var message = exception.Message;
            var innerEx = exception.InnerException;

            // Assert
            Assert.AreEqual(errorMessage, message);
            Assert.AreEqual(innerException, innerEx);
        }
    }
}
