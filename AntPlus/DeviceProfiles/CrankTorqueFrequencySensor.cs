﻿using AntPlus;
using System;
using System.Linq;

namespace DeviceProfiles
{
    public partial class BicyclePower
    {
        public class CrankTorqueFrequencySensor
        {
            private enum CTFDefinedId
            {
                ZeroOffset = 1,
                Slope = 2,
                SerialNumber = 3,
                Ack = 0xAC
            }

            private readonly BicyclePower bp;

            private bool isFirtPage = true;
            private byte prevUpdateEventCount;
            private ushort prevTimeStamp;
            private ushort prevTorqueTicks;

            public ushort Offset { get; private set; } = 0;
            public ushort Slope { get; private set; }
            public double Cadence { get; private set; }
            public double Torque { get; private set; }
            public double Power { get; private set; }

            public CrankTorqueFrequencySensor(BicyclePower bp)
            {
                this.bp = bp;
            }

            public void Parse(byte[] page)
            {
                byte updateEventCount = page[1];

                // the data is in big endian order; this also flips the order of the fields
                byte[] data = page.Skip(2).Reverse().ToArray();
                ushort torqueTicks = BitConverter.ToUInt16(data, 0);
                ushort timeStamp = BitConverter.ToUInt16(data, 2);
                Slope = BitConverter.ToUInt16(data, 4);

                if (isFirtPage)
                {
                    isFirtPage = false;
                    prevTimeStamp = timeStamp;
                    prevTorqueTicks = torqueTicks;
                    return;
                }

                // calculations
                double elapsedTime = Utils.CalculateDelta(timeStamp, ref prevTimeStamp) * 0.0005;
                double cadencePeriod = elapsedTime / Utils.CalculateDelta(updateEventCount, ref prevUpdateEventCount);
                Cadence = Math.Round(60.0 / cadencePeriod, MidpointRounding.AwayFromZero);
                double torqueFreq = (1.0 / (elapsedTime / Utils.CalculateDelta(torqueTicks, ref prevTorqueTicks))) - Offset;
                Torque = torqueFreq / (Slope / 10.0);
                Power = Torque * Cadence * Math.PI / 30.0;
            }

            public void ParseCalibrationMessage(byte[] message)
            {
                switch ((CTFDefinedId)message[2])
                {
                    case CTFDefinedId.ZeroOffset:
                        Offset = BitConverter.ToUInt16(message.Skip(6).Reverse().ToArray(), 0);
                        break;
                    case CTFDefinedId.Ack:
                        switch ((CTFDefinedId)message[3])
                        {
                            case CTFDefinedId.ZeroOffset:
                                break;
                            case CTFDefinedId.Slope:
                                break;
                            case CTFDefinedId.SerialNumber:
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            public void SaveSlopeToFlash(ushort slope)
            {
                byte[] msg = new byte[] { (byte)DataPage.Calibration, 0x10, (byte)CTFDefinedId.Slope, 0xFF, 0xFF, 0xFF };
                msg = msg.Concat(BitConverter.GetBytes(slope).Reverse()).ToArray();
                bp.SendExtAcknowledgedMessage(msg);
            }

            public void SaveSerialNumberToFlash(ushort serialNumber)
            {
                byte[] msg = new byte[] { (byte)DataPage.Calibration, 0x10, (byte)CTFDefinedId.SerialNumber, 0xFF, 0xFF, 0xFF };
                msg = msg.Concat(BitConverter.GetBytes(serialNumber).Reverse()).ToArray();
                bp.SendExtAcknowledgedMessage(msg);
            }
        }
    }
}
