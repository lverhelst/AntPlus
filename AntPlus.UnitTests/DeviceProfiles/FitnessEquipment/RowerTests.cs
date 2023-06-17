﻿using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Rower;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class RowerTests
    {
        [TestMethod]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var rower = new Rower();
            byte[] dataPage = new byte[] { 24, 0xFF, 0xFF, 0, 128, 0, 0x80, 0 };

            // Act
            rower.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(rower.Cadence == 128);
            Assert.IsTrue(rower.InstantaneousPower == 32768);
        }

        [TestMethod]
        public void Parse_StrokeCount_Matches()
        {
            // Arrange
            var rower = new Rower();
            byte[] dataPage = new byte[] { 24, 0xFF, 0xFF, 255, 0, 0, 0, 0 };
            rower.Parse(
                dataPage);
            dataPage[3] = 19;

            // Act
            rower.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(rower.StrokeCount == 20);
        }

        [TestMethod]
        [DataRow(new byte[] { 24, 0xFF, 0xFF, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [DataRow(new byte[] { 24, 0xFF, 0xFF, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrokeCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var rower = new Rower();

            // Act
            rower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(capabilities, rower.Capabilities);
        }
    }
}
