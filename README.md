[![](https://img.shields.io/nuget/v/SharpPcap.svg?label=NuGet&logo=nuget)](https://www.nuget.org/packages/SharpPcap/)
[![.NET Core](https://github.com/dotpcap/sharppcap/actions/workflows/dotnet-core.yml/badge.svg)](https://github.com/dotpcap/sharppcap/actions/workflows/dotnet-core.yml)
[![](https://img.shields.io/appveyor/ci/chmorgan/sharppcap/master.svg?label=AppVeyor&logo=appveyor)](https://ci.appveyor.com/project/chmorgan/sharppcap/branch/master)
[![](https://dev.azure.com/dotpcap/dotpcap/_apis/build/status/dotpcap.sharppcap)](https://dev.azure.com/dotpcap/dotpcap/_build/latest?definitionId=2&branchName=master)
[![](https://circleci.com/gh/dotpcap/sharppcap/tree/master.svg?style=svg)](https://circleci.com/gh/dotpcap/sharppcap/tree/master)
[![](https://codecov.io/gh/dotpcap/sharppcap/branch/master/graph/badge.svg?token=znqv9q5lVA)](https://codecov.io/gh/dotpcap/sharppcap)
[![](https://badges.gitter.im/SharpPcap/community.svg)](https://gitter.im/SharpPcap/community)

# sharppcap
Fully managed, cross platform (Windows, Mac, Linux) .NET library for capturing packets from live and file based devices

The official SharpPcap repository.

# Table of Contents
1. [Features](#features)
2. [Examples](#examples)
   - [Listing Devices](#listing-devices)
   - [Capturing Packets](#capturing-packets)
   - [Reading from a Capture File](#reading-from-a-capture-file)
   - [Writing to a Capture File](#writing-to-a-capture-file)
3. [CI Support](#ci-support)
4. [Releases](#releases)
5. [Platform Specific Notes](#platform-specific-notes)
6. [Migration from 5.x to 6.0](#migration-from-5x-to-60)

<a name="features"></a>

# Features
For packet dissection and creation, see [Packet.Net](https://github.com/chmorgan/packetnet).

* On Linux, support for [libpcap](http://www.tcpdump.org/manpages/pcap.3pcap.html)

* On Windows, support for:
  * Npcap (formerly WinPcap) extensions, see [Npcap API guide](https://nmap.org/npcap/guide/npcap-devguide.html#npcap-api)
  * WinDivert (https://www.reqrypt.org/windivert.html)

* On all platforms:
  * Live device lists
  * Statistics
  * Reading packets from Live Devices (actual network devices) and Offline Devices (Capture files)
  * Support for [Berkeley Packet Filters](https://www.tcpdump.org/manpages/pcap-filter.7.html)
  * Dumping packets to Pcap files.
  * Pcap and pcap-ng format (when using libpcap >=1.1.0 or npcap)
  * ReadOnlySpan<> is used to avoid memory allocation and copying inside of SharpPcap and provide the best performance.
    * Helper methods are provided to convert to object instances if it is desired to persist captured packets in memory.

* NativeLibrary support
  * Capture library resolution operates smoothly across Linux, OSX, and Windows
  * Cleanly loads libpcap on Linux whether the distro has a symlink to libpcap.so or not.

* .NET Core 3 and .NET Framework support

# Publishing to nuget package

Change the following lines in the config.yml file to automatically trigger a publish of the nuget package:

Change the version here => 
	nuget-output-file:
    type: string
    default: "SharpPcap/bin/Release/Lansweeper.SharpPcap.*.*.*.nupkg"
And here => 
	$env:package_version = "*.*.*"
# Examples

See the [Examples](https://github.com/chmorgan/sharppcap/tree/master/Examples) folder for a range of full example projects using SharpPcap

<a name="listing-devices"></a>

## Listing devices
   ```cs
   var devices = CaptureDeviceList.Instance;
   foreach (var dev in devices)
       Console.WriteLine("{0}\n", dev.ToString());
   ```

<a name="capturing-packets"></a>

## Capturing packets
   ```cs
   void Device_OnPacketArrival(object s, PacketCapture e)
   {
       Console.WriteLine(e.GetPacket());
   }

   using var device = LibPcapLiveDeviceList.Instance[0];
   device.Open();
   device.OnPacketArrival += Device_OnPacketArrival;
   device.StartCapture();
   ```

<a name="reading-from-a-capture-file"></a>

## Reading from a capture file
   ```cs
   void Device_OnPacketArrival(object s, PacketCapture e)
   {
       Console.WriteLine(e.GetPacket());
   }

   using var device = new CaptureFileReaderDevice("filename.pcap");
   device.Open();
   device.OnPacketArrival += Device_OnPacketArrival;
   device.Capture();
   ```

<a name="writing-to-a-capture-file"></a>

## Writing to a capture file
   ```cs
   using var device = new CaptureFileWriterDevice("somefilename.pcap", System.IO.FileMode.Open);
   var bytes = new byte[] { 1, 2, 3, 4 };
   device.Write(bytes);
   ```

<a name="ci-support"></a>

# CI support
We have support for a number of CI systems for a few reasons:

* Diversity of CI systems in case one of them shuts down
* Examples in case you'd like to customize SharpPcap and make use of one of these CI systems for internal builds. Note that we assume you are following the license for the library.

<a name="releases"></a>

# Releases
SharpPcap is released via nuget

<a name="platform-specific-notes"></a>

# Platform specific notes
* OSX (at least as of 11.1) lacks libpcap with pcap_open

# Thanks

SharpPcap is where it is today because of a number of developers who have provided improvements and fixes
and users that have provided helpful feedback through issues and feature requests.

We are especially appreciative of a number of projects we build upon (as SharpPcap is a C# wrapper):

* libpcap - thank you so much for releasing 1.10
* npcap - for continuing packet capture support on Windows

<a name="migration-from-5x-to-60"></a>

# Migration from 5.x to 6.0

We hope that you'll find the 6.x api to be cleaner and easier to use.

6.0 brings a number of cleanups that have resulted in API breakage for 5.x users.

To aid with the migration from 5.x to 6.0 here is a list of some of the changes you'll have to make to your
SharpPcap usage.

The examples are also a great resource as they show working examples using the latest API.

* Packet data is returned via PacketCapture which makes use of ReadOnlySpan<>.
  * Conversion from ReadOnlySpan<> to RawCapture is performed by PacketCapture.GetPacket().
  * This avoids allocation of memory during packet capture.
  * By avoiding memory allocation and memory copying, raw capture performance may be up to 30% faster.
  * Span's are ideal for use cases where packets are being dumped to disk for later processing.
* NativeLibrary is used for improved capture library resolution
  * Improves library resolution situation on Linux distros where there is a libpcap.so.X.Y symlink but no libpcap.so symlink
  * Support for Mono DllMap has been removed as Mono supports NativeLibrary. See https://www.mono-project.com/news/2020/08/24/native-loader-net5/
* Devices are IDisposable
  * Remove calls to Close()
  * Switch 'var device = xxx;'to 'using device = xxx;'
* Rename OpenFlags -> DeviceModes
* Open() methods have been collapsed into fewer methods with default variables.
* DeviceMode has been replaced by DeviceModes as DeviceMode was not able to cover all of the combinations of ways you could open a device.
* NpcapDevice -> LibPcapLiveDevice
  * If you are using NpcapDevice, you should consider using LibPcapLiveDevice. The latest versions of Npcap come with
newer versions of libpcap that provide almost all of the functionality of Npcap native APIs.
  * The current gap here is statistics mode, currently only supported by Npcap.
  * There has been talk of a statistics mode wrapper that would provide similar functionality, albeit without
the same level of efficiency as if it were done in the kernel or driver as on Windows, for libpcap systems.
* WinPcap has been deprecated
  * We recommend switching to LibPcapLiveDevice
* Remote authentication
  * If you are using RemoteAuthentication some functionality has been folded into this class and the api changed
to remove usage of ICredentials and NetworkCredentials.
