﻿using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SmallEarthTech.AntPlus.DeviceProfiles.Geocache
{
    /// <summary>
    /// This class supports geocaches. This is specified in ANT+ Managed Network Document – Geocache Device Profile.
    /// </summary>
    /// <remarks>
    /// Latitude and longitude coordinates are transmitted as 
    /// </remarks>
    /// <seealso cref="AntDevice" />
    public class Geocache : AntDevice
    {
        /// <summary>
        /// The geocache device class ID.
        /// </summary>
        public const byte DeviceClass = 19;

        /// <summary>
        /// Main data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>The trackable identifier</summary>
            TrackableId = 0x00,
            /// <summary>The PIN number</summary>
            PIN = 0x01,
            /// <summary>The authentication page</summary>
            AuthenticationPage = 0x20,
        }

        /// <summary>Data identifier of the geocache data page.</summary>
        private enum DataId
        {
            /// <summary>Latitude</summary>
            Latitude,
            /// <summary>Longitude</summary>
            Longitude,
            /// <summary>Hint or message</summary>
            Hint,
            /// <summary>The number of logged visits</summary>
            LoggedVisits = 4
        }

        private byte firstHintPage;
        private byte lastHintPage;
        private bool authRequested;
        private bool programmingGeocache;
        private byte loggedVisitsPage;
        private DateTime lastMessageTime;

        /// <summary>Gets the trackable identifier.</summary>
        public string TrackableId { get; private set; }
        /// <summary>Gets the programming PIN.</summary>
        public uint? ProgrammingPIN { get; private set; }
        /// <summary>Gets the total pages programmed.</summary>
        public byte? TotalPagesProgrammed { get; private set; }
        /// <summary>Gets the next stage latitude in semicircles(180/2^31). North is positive, south is negative.</summary>
        public int? NextStageLatitude { get; private set; }
        /// <summary>Gets the next stage longitude in semicircles(180/2^31). East is positive, west is negative.</summary>
        public int? NextStageLongitude { get; private set; }
        /// <summary>Gets a message from the geocache device, or a next stage hint.</summary>
        public string Hint { get; private set; } = string.Empty;
        /// <summary>Gets the number of visits logged.</summary>
        public ushort? NumberOfVisits { get; private set; }
        /// <summary>Gets the last visit timestamp.</summary>
        public DateTime? LastVisitTimestamp { get; private set; }
        /// <summary>Gets the authentication token.</summary>
        public byte[] AuthenticationToken { get; private set; } = new byte[0];
        /// <summary>Gets the message rate in Hz.
        /// This may be used to determine if the geocache is in low power mode and broadcasting at 0.5Hz or in active mode and broadcasting at 4Hz.</summary>
        /// <value>The message rate in Hz.</value>
        public double MessageRate { get; private set; }
        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; } = new CommonDataPages();
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(Geocache).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.Geocache.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="Geocache"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="timeout">Device offline timeout. The default is 8000 milliseconds.</param>
        /// <remarks>
        /// The geocache typically broadcasts its presence at 0.5Hz. The geocache changes its message rate to 4Hz upon
        /// receiving a request such as <see cref="RequestPinPage"/>. Set the timeout appropriate for your use case./>
        /// </remarks>
        public Geocache(ChannelId channelId, IAntChannel antChannel, int timeout = 8000) : base(channelId, antChannel, timeout)
        {
            lastMessageTime = DateTime.Now;
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);

            // determine message rate
            DateTime now = DateTime.Now;
            MessageRate = 1.0 / now.Subtract(lastMessageTime).TotalSeconds;
            lastMessageTime = now;
            RaisePropertyChange(nameof(MessageRate));

            // don't parse if programming geocache
            if (programmingGeocache)
            {
                return;
            }

            switch ((DataPage)dataPage[0])
            {
                case DataPage.TrackableId:
                    TrackableId = ParseId(dataPage);
                    RaisePropertyChange(nameof(TrackableId));
                    break;
                case DataPage.PIN:
                    uint pin = BitConverter.ToUInt32(dataPage, 2);
                    if (pin != 0xFFFFFFFF)
                    {
                        ProgrammingPIN = pin;
                    }
                    RaisePropertyChange(nameof(ProgrammingPIN));
                    if (dataPage[6] != 0xFF)
                    {
                        TotalPagesProgrammed = dataPage[6];
                    }
                    RaisePropertyChange(nameof(TotalPagesProgrammed));
                    break;
                case DataPage.AuthenticationPage:
                    if (authRequested)
                    {
                        authRequested = false;
                        AuthenticationToken = dataPage.Skip(1).ToArray();
                        RaisePropertyChange(nameof(AuthenticationToken));
                    }
                    break;
                default:
                    if (dataPage[0] >= 2 && dataPage[0] <= 31)
                    {
                        switch ((DataId)dataPage[1])
                        {
                            case DataId.Latitude:
                                NextStageLatitude = BitConverter.ToInt32(dataPage, 2);
                                RaisePropertyChange(nameof(NextStageLatitude));
                                break;
                            case DataId.Longitude:
                                NextStageLongitude = BitConverter.ToInt32(dataPage, 2);
                                RaisePropertyChange(nameof(NextStageLongitude));
                                break;
                            case DataId.Hint:
                                ParseHint(dataPage);
                                RaisePropertyChange(nameof(Hint));
                                break;
                            case DataId.LoggedVisits:
                                loggedVisitsPage = dataPage[0];
                                NumberOfVisits = BitConverter.ToUInt16(dataPage, 6);
                                RaisePropertyChange(nameof(NumberOfVisits));
                                if (NumberOfVisits > 0)
                                {
                                    LastVisitTimestamp = new DateTime(1989, 12, 31) + TimeSpan.FromSeconds(BitConverter.ToUInt32(dataPage, 2));
                                }
                                else { LastVisitTimestamp = null; }
                                RaisePropertyChange(nameof(LastVisitTimestamp));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        CommonDataPages.ParseCommonDataPage(dataPage);
                    }
                    break;
            }
        }

        private void ParseHint(byte[] dataPage)
        {
            if (firstHintPage == 0)
            {
                // initial hint received, could be any datapage
                firstHintPage = lastHintPage = dataPage[0];
                Hint = Encoding.UTF8.GetString(dataPage, 2, 6).TrimEnd((char)0);
            }
            else if (dataPage[0] < firstHintPage)
            {
                // start over if we received later hint pages first
                firstHintPage = lastHintPage = dataPage[0];
                Hint = Encoding.UTF8.GetString(dataPage, 2, 6).TrimEnd((char)0);
            }
            else if (dataPage[0] > lastHintPage)
            {
                // append subsequent hint pages
                lastHintPage = dataPage[0];
                Hint += Encoding.UTF8.GetString(dataPage, 2, 6).TrimEnd((char)0);
            }
        }

        private string ParseId(byte[] dataPage)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append((char)((dataPage[1] >> 2) + 0x20));
            stringBuilder.Append((char)((Utils.RotateLeft(BitConverter.ToUInt16(dataPage, 1), 4) & 0x3F) + 0x20));
            stringBuilder.Append((char)((Utils.RotateLeft(BitConverter.ToUInt16(dataPage, 2), 2) & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[3] & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[4] >> 2) + 0x20));
            stringBuilder.Append((char)((Utils.RotateLeft(BitConverter.ToUInt16(dataPage, 4), 4) & 0x3F) + 0x20));
            stringBuilder.Append((char)((Utils.RotateLeft(BitConverter.ToUInt16(dataPage, 5), 2) & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[6] & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[7] >> 2) + 0x20));
            return stringBuilder.ToString();
        }

        private byte[] FormatId(string id)
        {
            // pad ID with spaces if less than 9 characters
            if (id.Length < 9)
            {
                id = id.PadRight(9, ' ');
            }
            id = id.ToUpper();
            int ch1 = (id[0] - 0x20) << 18;
            ch1 |= ((id[1] - 0x20)) << 12;
            ch1 |= ((id[2] - 0x20)) << 6;
            ch1 |= (id[3] - 0x20);
            int ch2 = (id[4] - 0x20) << 26;
            ch2 |= (id[5] - 0x20) << 20;
            ch2 |= (id[6] - 0x20) << 14;
            ch2 |= (id[7] - 0x20) << 8;
            ch2 |= (id[8] - 0x20) << 2;
            return BitConverter.GetBytes(ch1).Take(3).Reverse().Concat(BitConverter.GetBytes(ch2).Reverse()).ToArray();
        }

        /// <summary> Requests the PIN page. Do this first before performing any other operations on the geocache.</summary>
        /// <returns>Status of the request. See <see cref="MessagingReturnCode"/></returns>
        public MessagingReturnCode RequestPinPage()
        {
            firstHintPage = lastHintPage = 0;
            Hint = string.Empty;
            return RequestDataPage(DataPage.PIN, 4000);
        }

        /// <summary>Requests the authentication.</summary>
        /// <param name="gpsSerialNumber">The GPS serial number.</param>
        /// <returns>Status of the request. See <see cref="MessagingReturnCode"/></returns>
        public MessagingReturnCode RequestAuthentication(uint gpsSerialNumber)
        {
            authRequested = true;
            Random random = new Random();
            byte[] nonce = new byte[2];
            random.NextBytes(nonce);
            byte[] msg = { (byte)DataPage.AuthenticationPage, 0xFF };
            msg = msg.Concat(nonce).Concat(BitConverter.GetBytes(gpsSerialNumber)).ToArray();
            return SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Updates the logged visits count and last visit timestamp.</summary>
        /// <exception cref="InvalidOperationException">The geocache has not been programmed.</exception>
        /// <returns>Status of the request. See <see cref="MessagingReturnCode"/></returns>
        public MessagingReturnCode UpdateLoggedVisits()
        {
            // check that a logged visits page has been programmed
            if (loggedVisitsPage == 0)
            {
                throw new InvalidOperationException("The geocache has not been programmed with a logged visits pages. Program the geocache; this will set a logged visits page.");
            }
            ushort addVisit = (ushort)(NumberOfVisits + 1);
            uint timestamp = (uint)(DateTime.UtcNow - new DateTime(1989, 12, 31)).TotalSeconds;
            return SendExtAcknowledgedMessage(new byte[] { loggedVisitsPage, (byte)DataId.LoggedVisits }.
                Concat(BitConverter.GetBytes(timestamp)).
                Concat(BitConverter.GetBytes(addVisit)).ToArray());
        }

        /// <summary>Programs the geocache.</summary>
        /// <param name="id">The trackable ID. Must be less than or equal to 9 characters.</param>
        /// <param name="pin">The programming PIN.</param>
        /// <param name="latitude">The latitude in semicircles.</param>
        /// <param name="longitude">The longitude in semicircles.</param>
        /// <param name="hint">The next stage hint or message.</param>
        /// <exception cref="ArgumentException">id is greater than 9 characters.</exception>
        /// <remarks>Latitude and longitude are expressed as semicircles.</remarks>
        public void ProgramGeocache(string id, uint pin, int? latitude, int? longitude, string hint)
        {
            programmingGeocache = true;
            byte page = 3;  // initial page number for optional pages
            byte[] msg;

            // validate ID length
            if (id.Length > 9)
            {
                throw new ArgumentException("The ID is longer than 9 characters.");
            }

            // program ID page
            msg = new byte[] { (byte)DataPage.TrackableId }.Concat(FormatId(id)).ToArray();
            SendExtAcknowledgedMessage(msg);

            // program logged visits - clear number of visits logged
            SendExtAcknowledgedMessage(new byte[] { 0x02, (byte)DataId.LoggedVisits, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 });

            // program latitude
            if (latitude is int lat)
            {
                msg = new byte[] { page++, (byte)DataId.Latitude }.
                        Concat(BitConverter.GetBytes(lat)).Concat(new byte[] { 0xFF, 0xFF }).ToArray();
                SendExtAcknowledgedMessage(msg);
            }

            // program longitude
            if (longitude is int lng)
            {
                msg = new byte[] { page++, (byte)DataId.Longitude }.
                        Concat(BitConverter.GetBytes(lng)).Concat(new byte[] { 0xFF, 0xFF }).ToArray();
                SendExtAcknowledgedMessage(msg);
            }

            // program hint pages - get hint and pad with null terminator
            Hint = string.Empty;
            firstHintPage = 0;
            if (hint?.Length > 0)
            {
                byte[] hnt = Encoding.UTF8.GetBytes(hint.PadRight(hint.Length + 6 - (hint.Length % 6), '\0'));
                while (hnt.Length > 0 && page < 32)
                {
                    msg = new byte[] { page++, (byte)DataId.Hint }.Concat(hnt.Take(6)).ToArray();
                    hnt = hnt.Skip(6).ToArray();
                    SendExtAcknowledgedMessage(msg);
                }
            }

            // program PIN page and total pages programmed
            msg = new byte[] { (byte)DataPage.PIN, 0xFF }.
                    Concat(BitConverter.GetBytes(pin)).
                    Concat(new byte[] { page }).ToArray();
            SendExtAcknowledgedMessage(msg);

            // clear remaining unused programmable pages
            msg = new byte[] { page, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            while (msg[0] < 32)
            {
                SendExtAcknowledgedMessage(msg);
                msg[0]++;
            }

            programmingGeocache = false;
        }
    }
}
