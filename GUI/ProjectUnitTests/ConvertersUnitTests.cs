using LeagueOfLegendsScenarioCreator.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUnitTests
{
    [TestClass]
    public class EmailFormatConverterTests
    {
        [TestMethod]
        public void Convert_ValidEmail_ReturnsTrue()
        {
            // Arrange
            var converter = new EmailFormatConverter();
            var email = "test@example.com";

            // Act
            var result = converter.Convert(email, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.IsTrue((bool)result);
        }

        [TestMethod]
        public void Convert_InvalidEmail_ReturnsFalse()
        {
            // Arrange
            var converter = new EmailFormatConverter();
            var email = "invalid_email";

            // Act
            var result = converter.Convert(email, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.IsFalse((bool)result);
        }

        [TestMethod]
        public void ConvertBack_ReturnsTrue()
        {
            // Arrange
            var converter = new EmailFormatConverter();
            var value = "";

            // Act
            var result = converter.ConvertBack(value, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.IsTrue((bool)result);
        }
    }
}
