﻿using System;

namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>
    /// Flags for configuring advanced bursting features.
    /// </summary>
    [Flags]
    public enum AdvancedBurstConfigFlags : uint
    {
        /// <summary>The frequency hop enable</summary>
        FrequencyHopEnable = 0x00000001,
    };

    /// <summary>
    /// Event groups for configuring Event Buffering
    /// </summary>
    [Flags]
    public enum EventBufferConfig : byte
    {
        /// <summary>The buffer low priority events</summary>
        BufferLowPriorityEvents = 0x00,
        /// <summary>The buffer all events</summary>
        BufferAllEvents = 0x01
    };

    /// <summary>
    /// Flags for configuring device ANT library
    /// </summary>
    [Flags]
    public enum LibConfigFlags
    {
        /// <summary>The radio configuration always</summary>
        RadioConfigAlways = 0x01,
        /// <summary>The message out inc time stamp</summary>
        MesgOutIncTimeStamp = 0x20,
        /// <summary>The message out inc rssi</summary>
        MesgOutIncRssi = 0x40,
        /// <summary>The message out inc device identifier</summary>
        MesgOutIncDeviceId = 0x80,
    }

    /// <summary>This interface defines common ANT radio configuration commands.</summary>
    public interface IAntConfiguration
    {
        /// <summary>Configures the advanced bursting.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="maxPacketLength">Maximum length of the packet.</param>
        /// <param name="requiredFields">The required fields.</param>
        /// <param name="optionalFields">The optional fields.</param>
        void ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields);
        /// <summary>Configures the advanced bursting.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="maxPacketLength">Maximum length of the packet.</param>
        /// <param name="requiredFields">The required fields.</param>
        /// <param name="optionalFields">The optional fields.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, uint responseWaitTime);
        /// <summary>Configures the extended advanced bursting.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="maxPacketLength">Maximum length of the packet.</param>
        /// <param name="requiredFields">The required fields.</param>
        /// <param name="optionalFields">The optional fields.</param>
        /// <param name="stallCount">The stall count.</param>
        /// <param name="retryCount">The retry count.</param>
        void ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount);
        /// <summary>Configures the extended advanced bursting.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="maxPacketLength">Maximum length of the packet.</param>
        /// <param name="requiredFields">The required fields.</param>
        /// <param name="optionalFields">The optional fields.</param>
        /// <param name="stallCount">The stall count.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime);
        /// <summary>Configures the advanced burst splitting.</summary>
        /// <param name="splitBursts">if set to <c>true</c> [split bursts].</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureAdvancedBurstSplitting(bool splitBursts);
        /// <summary>Configures the event buffer.</summary>
        /// <param name="config">The configuration.</param>
        /// <param name="size">The size.</param>
        /// <param name="time">The time.</param>
        void ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time);
        /// <summary>Configures the event buffer.</summary>
        /// <param name="config">The configuration.</param>
        /// <param name="size">The size.</param>
        /// <param name="time">The time.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time, uint responseWaitTime);
        /// <summary>FIT adjust pairing settings.</summary>
        /// <param name="searchLv">The search lv.</param>
        /// <param name="pairLv">The pair lv.</param>
        /// <param name="trackLv">The track lv.</param>
        /// <summary>Configures the event filter.</summary>
        /// <param name="eventFilter">The event filter.</param>
        void ConfigureEventFilter(ushort eventFilter);
        /// <summary>Configures the event filter.</summary>
        /// <param name="eventFilter">The event filter.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureEventFilter(ushort eventFilter, uint responseWaitTime);
        /// <summary>Configures the high duty search.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="suppressionCycles">The suppression cycles.</param>
        void ConfigureHighDutySearch(bool enable, byte suppressionCycles);
        /// <summary>Configures the high duty search.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="suppressionCycles">The suppression cycles.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureHighDutySearch(bool enable, byte suppressionCycles, uint responseWaitTime);
        /// <summary>Configures the user NVM.</summary>
        /// <param name="address">The address.</param>
        /// <param name="data">The data.</param>
        /// <param name="size">The size.</param>
        void ConfigureUserNvm(ushort address, byte[] data, byte size);
        /// <summary>Configures the user NVM.</summary>
        /// <param name="address">The address.</param>
        /// <param name="data">The data.</param>
        /// <param name="size">The size.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureUserNvm(ushort address, byte[] data, byte size, uint responseWaitTime);
        /// <summary>Crystal enable.</summary>
        void CrystalEnable();
        /// <summary>Crystal enable.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool CrystalEnable(uint responseWaitTime);
        /// <summary>Enables the led.</summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        void EnableLED(bool isEnabled);
        /// <summary>Enables the led.</summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool EnableLED(bool isEnabled, uint responseWaitTime);
        /// <summary>Enables Rx extended messages.</summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        void EnableRxExtendedMessages(bool isEnabled);
        /// <summary>Enables Rx extended messages.</summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool EnableRxExtendedMessages(bool isEnabled, uint responseWaitTime);
        /// <summary>Sets the library configuration.</summary>
        /// <param name="libConfigFlags">The library configuration flags.</param>
        void SetLibConfig(LibConfigFlags libConfigFlags);
        /// <summary>Sets the library configuration.</summary>
        /// <param name="libConfigFlags">The library configuration flags.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetLibConfig(LibConfigFlags libConfigFlags, uint responseWaitTime);
        /// <summary>Sets the network key.</summary>
        /// <param name="netNumber">The network number.</param>
        /// <param name="networkKey">The network key.</param>
        void SetNetworkKey(byte netNumber, byte[] networkKey);
        /// <summary>Sets the network key.</summary>
        /// <param name="netNumber">The network number.</param>
        /// <param name="networkKey">The network key.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime);
        /// <summary>Sets the transmit power for all channels.</summary>
        /// <param name="transmitPower">The transmit power.</param>
        void SetTransmitPowerForAllChannels(TransmitPower transmitPower);
        /// <summary>Sets the transmit power for all channels.</summary>
        /// <param name="transmitPower">The transmit power.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetTransmitPowerForAllChannels(TransmitPower transmitPower, uint responseWaitTime);
    }
}
