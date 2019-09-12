using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventoryPro;
using NUnit.Framework;

namespace Devdog.DevdogInternal.Tests
{
    public class StatsCollectionTests
    {
        public FakeStatDefinition statDef { get; set; }

        [SetUp]
        public void SetUp()
        {
            statDef = new FakeStatDefinition()
            {
                statName = "Health",
                category = "Default",
                baseValue = 10,
                maxValue = 100,
                startLevel = 2,
                levels = new StatLevel[]
                {
                    new StatLevel() { maxValue = 100f },
                    new StatLevel() { maxValue = 200f },
                    new StatLevel() { maxValue = 300f },
                    new StatLevel() { maxValue = 400f },
                    new StatLevel() { maxValue = 500f },
                },
            };
        }


        [Test]
        public void InitializingStatsCollectionShouldUseDataProviderToFill()
        {
            // Assign
            var statsCollection = new StatsCollection(new List<IStatsProvider>() { new FakeStatsProvider() });

            // Act
            statsCollection.Prepare();

            // Assert
            Assert.IsTrue(statsCollection.ContainsCategory("Default"));
            Assert.IsTrue(statsCollection.ContainsStat("Default", "Health"));
            Assert.IsNotNull(statsCollection.Get("Default", "Health"));
            Assert.IsNotNull(statsCollection.GetCategory("Default"));
        }


        [Test]
        public void FindingStatThatDoesntExistShouldReturnNull()
        {
            // Assign
            var statsCollection = new StatsCollection(new List<IStatsProvider>() { new FakeStatsProvider() });

            // Act
            statsCollection.Prepare();

            // Assert
            Assert.IsNull(statsCollection.Get("NonExistent", "Nope"));
            Assert.IsNull(statsCollection.GetCategory("NonExistent"));
        }



        [Test]
        public void AddedStatCanBeFound()
        {
            // Assign
            var statsCollection = new StatsCollection(new List<IStatsProvider>() { new FakeStatsProvider() });

            // Act
            statsCollection.Prepare();
            statsCollection.Add("Default", new Stat(new FakeStatDefinition() { category = "Default", statName = "Agility" }));
            statsCollection.Add("Default2", new Stat(new FakeStatDefinition() { category = "Default", statName = "Agility" }));

            // Assert
            Assert.IsNotNull(statsCollection.Get("Default", "Agility"));
            Assert.IsNotNull(statsCollection.GetCategory("Default"));
            Assert.IsNotNull(statsCollection.GetCategory("Default2"));
            Assert.IsNotNull(statsCollection.Get("Default2", "Agility"));
        }



        [Test]
        public void RemoveStat()
        {
            // Assign
            var statsCollection = new StatsCollection(new List<IStatsProvider>() { new FakeStatsProvider() });

            // Act
            statsCollection.Prepare();
            bool removed = statsCollection.Remove("Default", "Health");

            // Assert
            Assert.IsNull(statsCollection.Get("Default", "Health"));
            Assert.IsTrue(removed);
        }



        [Test]
        public void RemoveNonExistingStat()
        {
            // Assign
            var statsCollection = new StatsCollection(new List<IStatsProvider>() { new FakeStatsProvider() });

            // Act
            statsCollection.Prepare();
            bool removed = statsCollection.Remove("Default", "NonExistent");

            // Assert
            Assert.IsFalse(removed);
        }
    }
}
