﻿using System;
using System.Linq;

namespace AntPlus
{
    /// <summary>
    /// ANT channel sharing enumeration. This is obtained from the transmission type in the channel ID.
    /// </summary>
    public enum ChannelSharing
    {
        /// <summary>The reserved</summary>
        Reserved = 0,
        /// <summary>
        /// The independent channel
        /// </summary>
        IndependentChannel = 1,
        /// <summary>
        /// The shared channel one byte address
        /// </summary>
        SharedChannelOneByteAddress = 2,
        /// <summary>
        /// The shared channel two byte address
        /// </summary>
        SharedChannelTwoByteAddress = 3,
    }

    public enum MessageId

    {
        BroadcastData = 0x4E,
        AcknowledgedData = 0x4F,
        BurstData = 0x50,
        ExtBroadcastData = 0x5D,
        ExtAcknowledgedData = 0x5E,
        ExtBurstData = 0x5F
    }

    /// <summary>The channel ID is comprised of device number, device type, and transmission type.</summary>
    public readonly struct ChannelId
    {
        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        public uint Id { get; }
        /// <summary>Gets the type of the device.</summary>
        /// <value>The type of the device.</value>
        public byte DeviceType => (byte)(Id >> 16 & 0x0000007F);
        /// <summary>Gets the device number.</summary>
        /// <value>The device number.</value>
        public uint DeviceNumber => (Id & 0x0000FFFF) + (Id >> 12 & 0x000F0000);
        /// <summary>Gets a value indicating whether this instance has the pairing bit set.</summary>
        /// <value>
        ///   <c>true</c> if this instance has pairing bit set; otherwise, <c>false</c>.</value>
        public bool IsPairingBitSet => (Id & 0x00800000) == 0x00800000;
        /// <summary>Gets the type of the transmission.</summary>
        /// <value>The type of the transmission.</value>
        public ChannelSharing TransmissionType => (ChannelSharing)(Id >> 24 & 0x00000003);
        /// <summary>Gets a value indicating whether global data pages are used.</summary>
        /// <value>
        ///   <c>true</c> if global data pages are used; otherwise, <c>false</c>.</value>
        public bool AreGlobalDataPagesUsed => (Id & 0x04000000) == 0x04000000;

        /// <summary>Initializes a new instance of the <see cref="ChannelId" /> struct.</summary>
        /// <param name="channelId">The channel identifier.</param>
        public ChannelId(uint channelId)
        {
            Id = channelId;
        }
    }

    /// <summary>
    /// Base class for all ANT devices.
    /// </summary>
    public abstract class AntDevice
    {
        protected bool isFirstDataMessage = true;     // used for accumulated values


        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        public ChannelId ChannelId { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="AntDevice" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        protected AntDevice(ChannelId channelId)
        {
            ChannelId = channelId;
        }

        /// <summary>Parses the specified data page.</summary>
        /// <param name="payload">The data page.</param>
        public abstract void Parse(byte[] dataPage);

        /// <summary>
        /// Calculates the delta of the current and previous values. Rollover is accounted for and a positive integer is always returned.
        /// Add the returned value to the accumulated value in the derived class. The last value is updated with the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="lastValue">The last value.</param>
        /// <returns>Positive delta of the current and previous values.</returns>
        protected int CalculateDelta(byte currentValue, ref byte lastValue)
        {
            if (isFirstDataMessage)
            {
                lastValue = currentValue;
                return 0;
            }

            int delta = currentValue - lastValue;
            if (lastValue > currentValue)
            {
                // rollover
                delta += 256;
            }

            lastValue = currentValue;
            return delta;
        }

        /// <summary>
        /// Calculates the delta of the current and previous values. Rollover is accounted for and a positive integer is always returned.
        /// Add the returned value to the accumulated value in the derived class. The last value is updated with the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="lastValue">The last value.</param>
        /// <returns>Positive delta of the current and previous values.</returns>
        protected int CalculateDelta(ushort currentValue, ref ushort lastValue)
        {
            if (isFirstDataMessage)
            {
                lastValue = currentValue;
                return 0;
            }

            int delta = currentValue - lastValue;
            if (lastValue > currentValue)
            {
                // rollover
                delta += 0x10000;
            }

            lastValue = currentValue;
            return delta;
        }

        /// <summary>Requests the data page.</summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="channelNumber">The channel number.</param>
        /// <param name="transmissionResponse">The transmission response.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="slaveSerialNumber">The slave serial number.</param>
        /// <param name="decriptor1">The decriptor1.</param>
        /// <param name="descriptor2">The descriptor2.</param>
        public void RequestDataPage(byte pageNumber, byte channelNumber, byte transmissionResponse = 0x04, CommandType commandType = CommandType.DataPage, ushort slaveSerialNumber = 0xFFFF, byte decriptor1 = 0xFF, byte descriptor2 = 0xFF)
        {
            byte[] msg = new byte[] { (byte)CommonDataPageType.RequestDataPage, 0, 0, decriptor1, descriptor2, transmissionResponse, pageNumber, (byte)commandType };
            BitConverter.GetBytes(slaveSerialNumber).CopyTo(msg, 1);
            SendExtendedAcknowledgedMessage(channelNumber, msg);
        }

        /// <summary>Sends the extended acknowledged message.</summary>
        /// <param name="channelNumber">The channel number.</param>
        /// <param name="message">The message.</param>
        public void SendExtendedAcknowledgedMessage(byte channelNumber, byte[] message)
        {
            byte[] msg = new byte[] { 13, (byte)MessageId.ExtAcknowledgedData, channelNumber };
            msg = msg.Concat(BitConverter.GetBytes(ChannelId.Id)).Concat(message).ToArray();
            SendMessage(msg);
        }

        public byte[] Message { get; set; }
        private void SendMessage(byte[] msg)
        {
            Message = msg;
        }
    }
}
