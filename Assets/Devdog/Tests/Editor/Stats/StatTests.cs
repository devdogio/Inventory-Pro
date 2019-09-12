using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventoryPro;
using NUnit.Framework;

namespace Devdog.DevdogInternal.Tests
{
    public class StatTests
    {
        public FakeStatDefinition statDef { get; set; }
        public FakeStatDefinition statDefBlank { get; set; }


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
                autoProgressLevels = true,
                levels = new StatLevel[]
                {
                    new StatLevel() { maxValue = 100f, experienceRequired = 0f },
                    new StatLevel() { maxValue = 200f, experienceRequired = 200f },
                    new StatLevel() { maxValue = 300f, experienceRequired = 500f },
                    new StatLevel() { maxValue = 400f, experienceRequired = 1000f },
                    new StatLevel() { maxValue = 500f, experienceRequired = 3000f },
                },
            };

            statDefBlank = new FakeStatDefinition()
            {
                statName = "Blank",
                category = "Default",
                baseValue = 0,
                maxValue = 100,
                startLevel = 0,
                autoProgressLevels = true,
                levels = new StatLevel[]
                {

                },
            };
        }



        [Test]
        public void InitializingStatShouldLoadValuesFromDef()
        {
            // Assign
            var stat = new Stat(statDef);

            // Act

            // Assert
            Assert.AreEqual(stat.currentValue, statDef.baseValue);
            Assert.AreEqual(stat.currentMaxValue, stat.currentLevel.maxValue);
            Assert.AreEqual(2, stat.currentLevelIndex);
        }


        [Test]
        public void SetLevelOutOfBoundsShouldClamp()
        {
            // Assign
            var stat = new Stat(statDef);

            // Act
            stat.SetLevel(10, true);
            
            // Assert
            Assert.AreEqual(statDef.levels.Length - 1, stat.currentLevelIndex);
            Assert.AreEqual(statDef.levels[4], stat.currentLevel);
        }

        [Test]
        public void SetMaxValueShouldLeaveCurrentValue()
        {
            // Assign
            var stat = new Stat(statDef);

            // Act
            stat.SetCurrentValueRaw(10f);
            var currentValue = stat.currentValue;

            stat.SetMaxValueRaw(300f, false);
            var currentValue2 = stat.currentValue;

            // Assert
            Assert.AreEqual(10f, currentValue);
            Assert.AreEqual(10f, currentValue2);
        }

        [Test]
        public void SetMaxValueShouldLeaveCurrentValue2()
        {
            // Assign
            var stat = new Stat(statDef);

            // Act
            stat.SetCurrentValueRaw(10f);
            var currentValue = stat.currentValue;

            stat.SetMaxValueRaw(300f, false);
            var currentValue2 = stat.currentValue;

            // Assert
            Assert.AreEqual(10f, currentValue);
            Assert.AreEqual(10f, currentValue2);
        }


        [Test]
        public void SetMaxValueShouldLeaveCurrentValue3()
        {
            // Assign
            var stat = new Stat(statDef);

            // Act
            stat.SetMaxValueRaw(100f, false, false);
            stat.SetFactorMax(1.0f, false, false);
            stat.SetFactor(1.0f, false);
            stat.SetCurrentValueRaw(10f, true);

            // Assert
            Assert.AreEqual(10f, stat.currentValue);
        }

        [Test]
        public void SettingStatLevelShouldFireEvent()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCalledCount = 0;
            IStat eventStatValue = null;

            // Act
            stat.OnLevelChanged += characterStat =>
            {
                eventCalledCount++;
                eventStatValue = characterStat;
            };
            stat.SetLevel(3, true);

            // Assert
            Assert.AreEqual(1, eventCalledCount);
            Assert.AreEqual(stat, eventStatValue);
        }


        [Test]
        public void SettingStatLevelShouldNotFireEvent()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCalledCount = 0;
            IStat eventStatValue = null;

            // Act
            stat.OnLevelChanged += characterStat =>
            {
                eventCalledCount++;
                eventStatValue = characterStat;
            };
            stat.SetLevel(2, false);

            // Assert
            Assert.AreEqual(0, eventCalledCount);
            Assert.AreEqual(statDef.levels[2], stat.currentLevel);
            Assert.IsNull(eventStatValue);
        }


        [Test]
        public void SettingMaxValueChanges()
        {
            // Assign
            var stat = new Stat(statDefBlank);

            // Act
            stat.ChangeMaxValueRaw(10f, false);

            // Assert
            Assert.AreEqual(110f, stat.currentMaxValue);
            Assert.AreEqual(110f, stat.currentMaxValueRaw);
            Assert.AreEqual(0f, stat.currentValue);
            Assert.AreEqual(0f, stat.currentValueRaw);
        }

        [Test]
        public void SettingMaxValueAndCurrentChanges()
        {
            // Assign
            var stat = new Stat(statDefBlank);

            // Act
            stat.ChangeMaxValueRaw(10f, true);

            // Assert
            Assert.AreEqual(110f, stat.currentMaxValue);
            Assert.AreEqual(110f, stat.currentMaxValueRaw);
            Assert.AreEqual(10f, stat.currentValue);
            Assert.AreEqual(10f, stat.currentValueRaw);
        }

        [Test]
        public void ChangingValueFiresEvent()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCalledCount = 0;
            IStat eventStatValue = null;

            // Act
            stat.OnValueChanged += characterStat =>
            {
                eventCalledCount++;
                eventStatValue = characterStat;
            };
            stat.ChangeCurrentValueRaw(10f);

            // Assert
            Assert.AreEqual(20f, stat.currentValue);
            Assert.AreEqual(1, eventCalledCount);
            Assert.AreEqual(stat, eventStatValue);
        }


        [Test]
        public void ChangingValueDoesNotFiresEvent()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCalledCount = 0;
            IStat eventStatValue = null;

            // Act
            stat.OnValueChanged += characterStat =>
            {
                eventCalledCount++;
                eventStatValue = characterStat;
            };
            stat.ChangeCurrentValueRaw(10f, false);

            // Assert
            Assert.AreEqual(20f, stat.currentValue);
            Assert.AreEqual(0, eventCalledCount);
            Assert.IsNull(eventStatValue);
        }



        [Test]
        public void SettingFactorCalculatesValueProperly()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCount = 0;

            // Act
            stat.OnValueChanged += characterStat => eventCount++;
            stat.ChangeFactor(0.1f);

            // Assert
            Assert.AreEqual(11f, stat.currentValue);
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void SettingMaxFactorCalculatesValueProperly()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCount = 0;

            // Act
            stat.OnValueChanged += characterStat => eventCount++;
            stat.SetFactorMax(1.1f, false);

            // Assert
            Assert.AreEqual(330f, stat.currentMaxValue);
            Assert.AreEqual(1, eventCount);
        }


        [Test]
        public void SettingMaxFactorIncreasesCurrent()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCount = 0;

            // Act
            stat.OnValueChanged += characterStat => eventCount++;
            stat.SetFactorMax(1.1f, true);

            // Assert
            Assert.AreEqual(40f, stat.currentValue);
            Assert.AreEqual(330f, stat.currentMaxValue);
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void ResettingStat()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCount = 0;

            // Act
            stat.OnValueChanged += characterStat => eventCount++;
            stat.SetCurrentValueRaw(50f);
            stat.SetMaxValueRaw(120f, false);
            stat.Reset();

            // Assert
            Assert.AreEqual(10f, stat.currentValue);
            Assert.AreEqual(300f, stat.currentMaxValue);
            Assert.AreEqual(3, eventCount);
        }


        [Test]
        public void SetMaxValueRawAndIncreaseCurrentValue()
        {
            // Assign
            var stat = new Stat(statDef);
            int eventCount = 0;

            // Act
            stat.OnValueChanged += characterStat => eventCount++;
            stat.ChangeMaxValueRaw(10f, true);

            // Assert
            Assert.AreEqual(20f, stat.currentValue);
            Assert.AreEqual(310f, stat.currentMaxValue);
            Assert.AreEqual(1, eventCount);
        }


        [Test]
        public void IncreasingExperienceEnoughWillAutoIncreaseLevel()
        {
            // Assign
            var stat = new Stat(statDef);
            int onExperienceChangedEventCount = 0;
            int onLevelChangedEventCount = 0;

            // Act
            stat.OnExperienceChanged += stat1 =>
            {
                onExperienceChangedEventCount++;
            };
            stat.OnLevelChanged += stat1 =>
            {
                onLevelChangedEventCount++;
            };
            stat.SetExperience(1100f);

            // Assert
            Assert.AreEqual(3, stat.currentLevelIndex);
            Assert.AreEqual(1, onExperienceChangedEventCount);
            Assert.AreEqual(1, onLevelChangedEventCount);
        }

        [Test]
        public void DecreasingExperienceEnoughWillAutoDecreaseLevel()
        {
            // Assign
            var stat = new Stat(statDef);
            int onExperienceChangedEventCount = 0;
            int onLevelChangedEventCount = 0;

            // Act
            stat.OnExperienceChanged += stat1 =>
            {
                onExperienceChangedEventCount++;
            };
            stat.OnLevelChanged += stat1 =>
            {
                onLevelChangedEventCount++;
            };
            stat.SetExperience(1100f);
            stat.SetExperience(210f);

            // Assert
            Assert.AreEqual(1, stat.currentLevelIndex);
            Assert.AreEqual(2, onExperienceChangedEventCount);
            Assert.AreEqual(2, onLevelChangedEventCount);
        }

        [Test]
        public void IncreaseMultipleLevelsInASingleCall()
        {
            // Assign
            var stat = new Stat(statDef);
            int onExperienceChangedEventCount = 0;
            int onLevelChangedEventCount = 0;

            // Act
            stat.OnExperienceChanged += stat1 =>
            {
                onExperienceChangedEventCount++;
            };
            stat.OnLevelChanged += stat1 =>
            {
                onLevelChangedEventCount++;
            };
            stat.SetExperience(99999f);

            // Assert
            Assert.AreEqual(4, stat.currentLevelIndex);
            Assert.AreEqual(1, onExperienceChangedEventCount);
            Assert.AreEqual(1, onLevelChangedEventCount);
        }
    }
}
