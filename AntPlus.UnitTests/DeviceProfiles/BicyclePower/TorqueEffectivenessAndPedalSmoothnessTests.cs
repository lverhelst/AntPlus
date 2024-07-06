﻿using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class TorqueEffectivenessAndPedalSmoothnessTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<BicyclePower>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<BicyclePower>>();
        }

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            byte[] page = new byte[8] { (byte)DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object) as StandardPowerSensor;
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(200, 100)]
        [DataRow(0xFF, double.NaN)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedTorqueEffectiveness(int value, double expPct)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, (byte)value, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPct, sensor.TorqueEffectiveness.LeftTorqueEffectiveness);
            Assert.AreEqual(expPct, sensor.TorqueEffectiveness.RightTorqueEffectiveness);
        }

        [TestMethod]
        [DataRow(0, 0, 0, false)]
        [DataRow(200, 200, 100, false)]
        [DataRow(0xFF, 0xFF, double.NaN, false)]
        [DataRow(100, 0xFE, 50, true)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedPedalSmoothness(int left, int right, double expPct, bool expCombined)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, 0xFF, 0xFF, (byte)left, (byte)right, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPct, sensor.TorqueEffectiveness.LeftPedalSmoothness);
            Assert.AreEqual(expPct, sensor.TorqueEffectiveness.RightPedalSmoothness);
            Assert.AreEqual(expCombined, sensor.TorqueEffectiveness.CombinedPedalSmoothness);
        }
    }
}
