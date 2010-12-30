/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/* 
 * Copyright 2010-2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace SharpPcap.AirPcap
{
    public class ChannelRadioTapField : RadioTapField
    {
        public UInt16 FrequencyMHz { get; set; }

        /// <summary>
        /// Channel number derived from frequency
        /// </summary>
        public int Channel { get; set; }

        public AirPcapRadioTapChannelFlags Flags;

        public static int ChannelFromFrequencyMHz(int frequencyMHz)
        {
            switch (frequencyMHz)
            {
                case 2412:
                    return 1;
                case 2417:
                    return 2;
                case 2422:
                    return 3;
                case 2427:
                    return 4;
                case 2432:
                    return 5;
                case 2437:
                    return 6;
                case 2442:
                    return 7;
                case 2447:
                    return 8;
                case 2452:
                    return 9;
                case 2457:
                    return 10;
                case 2462:
                    return 11;
                case 2467:
                    return 12;
                case 2472:
                    return 13;
                case 2484:
                    return 14;
                default:
                    throw new System.NotImplementedException("Unknown frequencyMHz " + frequencyMHz);
            };
        }

        public ChannelRadioTapField(BinaryReader br)
        {
            FrequencyMHz = br.ReadUInt16();
            Channel = ChannelFromFrequencyMHz(FrequencyMHz);
            Flags = (AirPcapRadioTapChannelFlags)br.ReadUInt16();
        }

        public override string ToString()
        {
            return string.Format("FrequencyMHz {0}, Channel {1}, Flags {2}",
                                 FrequencyMHz, Channel, Flags);
        }
    }

    public class FhssRadioTapField : RadioTapField
    {
        public ushort ChannelHoppingSet { get; set; }
        public ushort Pattern { get; set; }

        public FhssRadioTapField(BinaryReader br)
        {
            var u16 = br.ReadUInt16();

            ChannelHoppingSet = (ushort)(u16 & 0xff);
            Pattern = (ushort)((u16 >> 8) & 0xff);
        }

        public override string ToString()
        {
            return string.Format("ChannelHoppingSet {0}, Pattern {1}",
                                 ChannelHoppingSet, Pattern);
        }
    }

    public class FlagsRadioTapField : RadioTapField
    {
        public AirPcapRadioTapFlags Flags;

        public FlagsRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            Flags = (AirPcapRadioTapFlags)u8;
        }

        public override string ToString()
        {
            return string.Format("Flags {0}", Flags);
        }
    }

    public class RateRadioTapField : RadioTapField
    {
        public double RateMbps { get; set; }

        public RateRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            RateMbps = (0.5 * (u8 & 0x7f));
        }

        public override string ToString()
        {
            return string.Format("RateMbps {0}", RateMbps);
        }
    }

    public class DbAntennaSignalRadioTapField : RadioTapField
    {
        public byte SignalStrengthdB { get; set; }

        public DbAntennaSignalRadioTapField(BinaryReader br)
        {
            SignalStrengthdB = br.ReadByte();
        }

        public override string ToString()
        {
            return string.Format("SignalStrengthdB {0}", SignalStrengthdB);
        }
    }

    public class DbAntennaNoiseRadioTapField : RadioTapField
    {
        public byte AntennaNoisedB { get; set; }

        public DbAntennaNoiseRadioTapField(BinaryReader br)
        {
            AntennaNoisedB = br.ReadByte();
        }

        public override string ToString()
        {
            return string.Format("AntennaNoisedB {0}", AntennaNoisedB);
        }
    }

    public class AntennaRadioTapField : RadioTapField
    {
        public byte Antenna { get; set; }

        public AntennaRadioTapField(BinaryReader br)
        {
            Antenna = br.ReadByte();
        }

        public override string ToString()
        {
            return string.Format("Antenna {0}", Antenna);
        }
    }

    public class DbmAntennaSignalRadioTapField : RadioTapField
    {
        public sbyte AntennaSignalDbm { get; set; }

        public DbmAntennaSignalRadioTapField(BinaryReader br)
        {
            AntennaSignalDbm = br.ReadSByte();
        }

        public override string ToString()
        {
            return string.Format("AntennaSignalDbm {0}", AntennaSignalDbm);
        }
    }

    public class DbmAntennaNoiseRadioTapField : RadioTapField
    {
        public sbyte AntennaNoisedBm { get; set; }

        public DbmAntennaNoiseRadioTapField(BinaryReader br)
        {
            AntennaNoisedBm = br.ReadSByte();
        }

        public override string ToString()
        {
            return string.Format("AntennaNoisedBm {0}", AntennaNoisedBm);
        }
    }

    public class LockQualityRadioTapField : RadioTapField
    {
        public UInt16 SignalQuality { get; set; }

        public LockQualityRadioTapField(BinaryReader br)
        {
            SignalQuality = br.ReadUInt16();
        }

        public override string ToString()
        {
            return string.Format("SignalQuality {0}", SignalQuality);
        }
    }

    public class TsftRadioTapField : RadioTapField
    {
        public UInt64 TimestampUsec { get; set; }

        public TsftRadioTapField(BinaryReader br)
        {
            TimestampUsec = br.ReadUInt64();
        }

        public override string ToString()
        {
            return string.Format("TimestampUsec {0}", TimestampUsec);
        }
    }

    public class FcsRadioTapField : RadioTapField
    {
        public UInt32 FrameCheckSequence { get; set; }

        public FcsRadioTapField(BinaryReader br)
        {
            FrameCheckSequence = (UInt32)System.Net.IPAddress.HostToNetworkOrder(br.ReadInt32());
        }

        public override string ToString()
        {
            return string.Format("FrameCheckSequence {0}", FrameCheckSequence);
        }
    }

    public class TxAttenuationRadioTapField : RadioTapField
    {
        public int TxPower { get; set; }

        public TxAttenuationRadioTapField(BinaryReader br)
        {
            TxPower = -(int)br.ReadUInt16();
        }

        public override string ToString()
        {
            return string.Format("TxPower {0}", TxPower);
        }
    }

    public class DbTxAttenuationRadioTapField : RadioTapField
    {
        public int TxPowerdB { get; set; }

        public DbTxAttenuationRadioTapField(BinaryReader br)
        {
            TxPowerdB = -(int)br.ReadByte();
        }

        public override string ToString()
        {
            return string.Format("TxPowerdB {0}", TxPowerdB);
        }
    }

    public class DbmTxPowerRadioTapField : RadioTapField
    {
        public sbyte TxPowerdBm { get; set; }

        public DbmTxPowerRadioTapField(BinaryReader br)
        {
            TxPowerdBm = br.ReadSByte();
        }

        public override string ToString()
        {
            return string.Format("TxPowerdBm {0}", TxPowerdBm);
        }
    }

    public class RadioTapField
    {
        public AirPcapRadioTapType Type;

        public static RadioTapField Parse(int bitIndex, BinaryReader br)
        {
            switch ((AirPcapRadioTapType)bitIndex)
            {
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_FLAGS:
                    return new FlagsRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_RATE:
                    return new RateRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_DB_ANTSIGNAL:
                    return new DbAntennaSignalRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_DB_ANTNOISE:
                    return new DbAntennaNoiseRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_ANTENNA:
                    return new AntennaRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_DBM_ANTSIGNAL:
                    return new DbmAntennaSignalRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_DBM_ANTNOISE:
                    return new DbmAntennaNoiseRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_CHANNEL:
                    return new ChannelRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_FHSS:
                    return new FhssRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_LOCK_QUALITY:
                    return new LockQualityRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_TX_ATTENUATION:
                    return new TxAttenuationRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_DB_TX_ATTENUATION:
                    return new DbTxAttenuationRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_DBM_TX_POWER:
                    return new DbmTxPowerRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_TSFT:
                    return new TsftRadioTapField(br);
                case AirPcapRadioTapType.IEEE80211_RADIOTAP_FCS:
                    return new FcsRadioTapField(br);
                default:
                    throw new System.NotImplementedException("Unknown bitIndex of " + bitIndex);
            }
        }
    };
}
