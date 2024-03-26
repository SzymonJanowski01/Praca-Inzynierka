using Avalonia.Platform;
using Avalonia;
using LeagueOfLegendsScenarioCreator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using System.IO;
using System.Drawing;

namespace ProjectUnitTests
{
    #region PhaseTests
    [TestClass]
    public class PhaseTests
    {
        [TestMethod]
        public void PhaseId_PropertyChanged_EventRaised()
        {
            // Arrange
            var phase = new Phase();
            bool propertyChangedRaised = false;

            phase.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Phase.PhaseId))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            phase.PhaseId = "NewPhaseId";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void PhaseName_PropertyChanged_EventRaised()
        {
            // Arrange
            var phase = new Phase();
            bool propertyChangedRaised = false;

            phase.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Phase.PhaseName))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            phase.PhaseName = "NewPhaseName";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void PhaseMainCharacter_PropertyChanged_EventRaised()
        {
            // Arrange
            var phase = new Phase();
            bool propertyChangedRaised = false;

            phase.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Phase.MainCharacter))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            phase.MainCharacter = "NewMainCharacter";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void PhaseFirstAlternativeCharacter_PropertyChanged_EventRaised()
        {
            // Arrange
            var phase = new Phase();
            bool propertyChangedRaised = false;

            phase.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Phase.FirstAlternaticeCharacter))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            phase.FirstAlternaticeCharacter = "NewFirstAlternaticeCharacter";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void PhaseSecondAlternativeCharacter_PropertyChanged_EventRaised()
        {
            // Arrange
            var phase = new Phase();
            bool propertyChangedRaised = false;

            phase.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Phase.SecondAlternaticeCharacter))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            phase.SecondAlternaticeCharacter = "NewSecondAlternaticeCharacter";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

    }
    #endregion
    #region ScenarioTests
    [TestClass]
    public class ScenarioTests
    {
        [TestMethod]
        public void ScenarioId_PropertyChanged_EventRaised()
        {
            // Arrange
            var scenario = new Scenario();
            bool propertyChangedRaised = false;

            scenario.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Scenario.ScenarioId))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            scenario.ScenarioId = "NewScenarioId";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void ScenarioName_PropertyChanged_EventRaised()
        {
            // Arrange
            var scenario = new Scenario();
            bool propertyChangedRaised = false;

            scenario.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Scenario.ScenarioName))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            scenario.ScenarioName = "NewScenarioName";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void ScenarioCratedAt_PropertyChanged_EventRaised()
        {
            // Arrange
            var scenario = new Scenario();
            bool propertyChangedRaised = false;

            scenario.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Scenario.CreatedAt))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            scenario.CreatedAt = "NewCreationTime";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void ScenarioLastModifiedAt_PropertyChanged_EventRaised()
        {
            // Arrange
            var scenario = new Scenario();
            bool propertyChangedRaised = false;

            scenario.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Scenario.LastModifiedAt))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            scenario.LastModifiedAt = "NewLastModificationTime";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void ScenarioPhases_CollectionModification_PropertyChangedEventRaised()
        {
            // Arrange
            var scenario = new Scenario();
            bool propertyChangedRaised = false;

            scenario.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Scenario.Phases))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            scenario.Phases = new ObservableCollection<Phase>();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void ScenarioPhases_CollectionAdding_CollectionChangedEventRaised()
        {
            // Arrange
            var scenario = new Scenario();
            bool collectionChangedRaised = false;

            scenario.Phases = new ObservableCollection<Phase>();
            scenario.Phases.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            scenario.Phases.Add(new Phase());

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void ScenarioPhases_CollectionClearing_CollectionChangedEventRaised()
        {
            // Arrange
            var scenario = new Scenario();
            bool collectionChangedRaised = false;

            scenario.Phases = new ObservableCollection<Phase>
            {
                new Phase(),
                new Phase(),
                new Phase()
            };

            scenario.Phases.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            scenario.Phases.Clear();

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void ScenarioPhases_CollectionRemoving_CollectionChangedEventRaised()
        {
            // Arrange
            var scenario = new Scenario();
            bool collectionChangedRaised = false;

            scenario.Phases = new ObservableCollection<Phase>
            {
                new Phase(),
                new Phase()
            };

            scenario.Phases.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            scenario.Phases.Remove(scenario.Phases.First());

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void Scenario_Serialization_Deserialiazation()
        {
            // Arrange
            var scenario = new Scenario
            {
                ScenarioId = "ScenarioId",
                ScenarioName = "ScenarioName",
                CreatedAt = "CreatedAt",
                LastModifiedAt = "LastModifiedAt",
                Phases = new ObservableCollection<Phase>
                {
                    new Phase
                    {
                        PhaseId = "PhaseId",
                        PhaseName = "PhaseName",
                        MainCharacter = "MainCharacter",
                        FirstAlternaticeCharacter = "FirstAlternaticeCharacter",
                        SecondAlternaticeCharacter = "SecondAlternaticeCharacter"
                    }
                }
            };

            // Act
            var serializedScenario = JsonSerializer.Serialize(scenario);
            var deserializedScenario = JsonSerializer.Deserialize<Scenario>(serializedScenario);

            // Assert
            Assert.AreEqual(scenario.ScenarioId, deserializedScenario!.ScenarioId);
            Assert.AreEqual(scenario.ScenarioName, deserializedScenario.ScenarioName);
            Assert.AreEqual(scenario.CreatedAt, deserializedScenario.CreatedAt);
            Assert.AreEqual(scenario.LastModifiedAt, deserializedScenario.LastModifiedAt);
            Assert.AreEqual(scenario.Phases!.Count, deserializedScenario.Phases!.Count);
            Assert.AreEqual(scenario.Phases.First().PhaseId, deserializedScenario.Phases.First().PhaseId);
            Assert.AreEqual(scenario.Phases.First().PhaseName, deserializedScenario.Phases.First().PhaseName);
            Assert.AreEqual(scenario.Phases.First().MainCharacter, deserializedScenario.Phases.First().MainCharacter);
            Assert.AreEqual(scenario.Phases.First().FirstAlternaticeCharacter, deserializedScenario.Phases.First().FirstAlternaticeCharacter);
            Assert.AreEqual(scenario.Phases.First().SecondAlternaticeCharacter, deserializedScenario.Phases.First().SecondAlternaticeCharacter);
        }
    }
    #endregion
    #region UserTests
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void UserId_PropertyChanged_EventRaised()
        {
            // Arrange
            var user = new User();
            bool propertyChangedRaised = false;

            user.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(User.UserId))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            user.UserId = "NewUserId";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void UserUsername_PropertyChanged_EventRaised()
        {
            // Arrange
            var user = new User();
            bool propertyChangedRaised = false;

            user.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(User.Username))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            user.Username = "NewUsername";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void UserPassword_PropertyChanged_EventRaised()
        {
            // Arrange
            var user = new User();
            bool propertyChangedRaised = false;

            user.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(User.Password))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            user.Password = "NewPassword";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void UserEmail_PropertyChanged_EventRaised()
        {
            // Arrange
            var user = new User();
            bool propertyChangedRaised = false;

            user.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(User.Email))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            user.Email = "NewEmail";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void UserScenarios_CollectionModification_PropertyChangedEventRaised()
        {
            // Arrange
            var user = new User();
            bool propertyChangedRaised = false;

            user.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(User.Scenarios))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            user.Scenarios = new ObservableCollection<Scenario>();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void UserScenarios_CollectionAdding_CollectionChangedEventRaised()
        {
            // Arrange
            var user = new User();
            bool collectionChangedRaised = false;

            user.Scenarios = new ObservableCollection<Scenario>();
            user.Scenarios.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            user.Scenarios.Add(new Scenario());

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void UserScenarios_CollectionClearing_CollectionChangedEventRaised()
        {
            // Arrange
            var user = new User();
            bool collectionChangedRaised = false;

            user.Scenarios = new ObservableCollection<Scenario>
            {
                new Scenario(),
                new Scenario(),
                new Scenario()
            };

            user.Scenarios.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            user.Scenarios.Clear();

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void UserScenarios_CollectionRemoving_CollectionChangedEventRaised()
        {
            // Arrange
            var user = new User();
            bool collectionChangedRaised = false;

            user.Scenarios = new ObservableCollection<Scenario>
            {
                new Scenario(),
                new Scenario()
            };

            user.Scenarios.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            user.Scenarios.Remove(user.Scenarios.First());

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void UserScenariosNames_CollectionModification_PropertyChangedEventRaised()
        {
            // Arrange
            var user = new User();
            bool propertyChangedRaised = false;

            user.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(User.ScenariosNames))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            user.ScenariosNames = new ObservableCollection<string>();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        public void UserScenariosNames_CollectionAdding_CollectionChangedEventRaised()
        {
            // Arrange
            var user = new User();
            bool collectionChangedRaised = false;

            user.ScenariosNames = new ObservableCollection<string>();
            user.ScenariosNames.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            user.ScenariosNames.Add("NewScenarioName");

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void UserScenariosNames_CollectionClearing_CollectionChangedEventRaised()
        {
            // Arrange
            var user = new User();
            bool collectionChangedRaised = false;

            user.ScenariosNames = new ObservableCollection<string>
            {
                "ScenarioName1",
                "ScenarioName2",
                "ScenarioName3"
            };

            user.ScenariosNames.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            user.ScenariosNames.Clear();

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void UserScenariosNames_CollectionRemoving_CollectionChangedEventRaised()
        {
            // Arrange
            var user = new User();
            bool collectionChangedRaised = false;

            user.ScenariosNames = new ObservableCollection<string>
            {
                "ScenarioName1",
                "ScenarioName2"
            };

            user.ScenariosNames.CollectionChanged += (sender, args) =>
            {
                collectionChangedRaised = true;
            };

            // Act
            user.ScenariosNames.Remove(user.ScenariosNames.First());

            // Assert
            Assert.IsTrue(collectionChangedRaised);
        }

        [TestMethod]
        public void User_Serialization_Deserliazation()
        {
            // Arrange
            var user = new User
            {
                UserId = "UserId",
                Username = "Username",
                Password = "Password",
                Email = "Email",
                Scenarios = new ObservableCollection<Scenario>
                {
                    new Scenario
                    {
                        ScenarioId = "ScenarioId",
                        ScenarioName = "ScenarioName",
                        CreatedAt = "CreatedAt",
                        LastModifiedAt = "LastModifiedAt",
                        Phases = new ObservableCollection<Phase>
                        {
                            new Phase
                            {
                                PhaseId = "PhaseId",
                                PhaseName = "PhaseName",
                                MainCharacter = "MainCharacter",
                                FirstAlternaticeCharacter = "FirstAlternaticeCharacter",
                                SecondAlternaticeCharacter = "SecondAlternaticeCharacter"
                            }
                        }
                    }
                },
                ScenariosNames = new ObservableCollection<string>
                {
                    "ScenarioName1",
                    "ScenarioName2",
                    "ScenarioName3"
                }
            };

            // Act
            var serializedUser = JsonSerializer.Serialize(user);
            var deserializedUser = JsonSerializer.Deserialize<User>(serializedUser);

            // Assert
            Assert.AreEqual(user.UserId, deserializedUser!.UserId);
            Assert.AreEqual(user.Username, deserializedUser.Username);
            Assert.AreEqual(user.Password, deserializedUser.Password);
            Assert.AreEqual(user.Email, deserializedUser.Email);
            Assert.AreEqual(user.Scenarios!.Count, deserializedUser.Scenarios!.Count);
            Assert.AreEqual(user.Scenarios.First().ScenarioId, deserializedUser.Scenarios.First().ScenarioId);
            Assert.AreEqual(user.Scenarios.First().ScenarioName, deserializedUser.Scenarios.First().ScenarioName);
            Assert.AreEqual(user.Scenarios.First().CreatedAt, deserializedUser.Scenarios.First().CreatedAt);
            Assert.AreEqual(user.Scenarios.First().LastModifiedAt, deserializedUser.Scenarios.First().LastModifiedAt);
            Assert.AreEqual(user.Scenarios.First().Phases!.Count, deserializedUser.Scenarios.First().Phases!.Count);
            Assert.AreEqual(user.Scenarios.First().Phases!.First().PhaseId, deserializedUser.Scenarios.First().Phases!.First().PhaseId);
            Assert.AreEqual(user.Scenarios.First().Phases!.First().PhaseName, deserializedUser.Scenarios.First().Phases!.First().PhaseName);
            Assert.AreEqual(user.Scenarios.First().Phases!.First().MainCharacter, deserializedUser.Scenarios.First().Phases!.First().MainCharacter);
        }
    }
    #endregion
    #region PhaseProjectorTests
    [TestClass]
    public class PhaseProjectorTests
    {
        // TODO: Add tests for PhaseProjector
    }
    #endregion
    #region ImageLoaderTests
    #endregion
}
