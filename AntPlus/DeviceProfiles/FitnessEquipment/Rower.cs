﻿using System;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    public class Rower
    {
        private bool isFirstDataMessage = true;
        private byte prevStroke;

        public int StrokeCount { get; private set; }
        public byte Cadence { get; private set; }
        public int InstantaneousPower { get; private set; }

        public event EventHandler RowerChanged;

        public void Parse(byte[] dataPage)
        {
            if (isFirstDataMessage)
            {
                isFirstDataMessage = false;
                prevStroke = dataPage[3];
            }
            else
            {
                StrokeCount += Utils.CalculateDelta(dataPage[3], ref prevStroke);
            }
            Cadence = dataPage[4];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
            RowerChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
