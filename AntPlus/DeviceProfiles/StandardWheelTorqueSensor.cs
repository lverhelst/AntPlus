﻿using AntRadioInterface;
using System.Linq;

namespace AntPlus.DeviceProfiles
{
    public class StandardWheelTorqueSensor : TorqueSensor
    {
        public StandardWheelTorqueSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);

            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;
        }
    }
}
