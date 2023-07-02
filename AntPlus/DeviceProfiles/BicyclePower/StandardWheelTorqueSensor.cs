﻿using Microsoft.Extensions.Logging;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The standard wheel torque sensor.
    /// </summary>
    /// <remarks>
    /// Set the wheel circumference if the default value is incorrect. The calculations rely
    /// on the wheel circumference.
    /// </remarks>
    /// <seealso cref="TorqueSensor" />
    public class StandardWheelTorqueSensor : TorqueSensor
    {
        /// <summary>
        /// Wheel circumference in meters. The default is 2.2 meters.
        /// </summary>
        public double WheelCircumference { get; set; } = 2.2;
        /// <summary>
        /// Average speed in kilometers per hour.
        /// </summary>
        public double AverageSpeed { get; private set; }
        /// <summary>
        /// Accumulated distance in meters.
        /// </summary>
        public double AccumulatedDistance { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="StandardWheelTorqueSensor" /> class.</summary>
        /// <param name="bp">The bp.</param>
        /// <param name="logger">Logger to use.</param>
        public StandardWheelTorqueSensor(BicyclePower bp, ILogger logger) : base(bp, logger)
        {
        }

        /// <inheritdoc/>
        public override void ParseTorque(byte[] dataPage)
        {
            base.ParseTorque(dataPage);
            if (deltaEventCount != 0)
            {
                AverageSpeed = Utils.ComputeAvgSpeed(WheelCircumference, deltaEventCount, deltaPeriod);
                AccumulatedDistance += Utils.ComputeDeltaDistance(WheelCircumference, deltaTicks);
                RaisePropertyChange(nameof(AverageSpeed));
                RaisePropertyChange(nameof(AccumulatedDistance));
            }
        }
    }
}
