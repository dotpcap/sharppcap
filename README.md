[![](https://img.shields.io/nuget/v/SharpPcap.svg?label=NuGet&logo=nuget)](https://www.nuget.org/packages/SharpPcap/)
![.NET Core](https://github.com/chmorgan/sharppcap/workflows/.NET%20Core/badge.svg)
[![](https://img.shields.io/appveyor/ci/chmorgan/sharppcap/master.svg?label=AppVeyor&logo=appveyor)](https://ci.appveyor.com/project/chmorgan/sharppcap/branch/master)
[![](https://dev.azure.com/chmorgan/chmorgan/_apis/build/status/chmorgan.sharppcap)](https://dev.azure.com/chmorgan/chmorgan/_build/latest?definitionId=1&branchName=master)
[![](https://img.shields.io/circleci/build/gh/chmorgan/sharppcap?label=CircleCI&logo=circleci)](https://circleci.com/gh/chmorgan/sharppcap)
[![](https://img.shields.io/travis/com/chmorgan/sharppcap/master?label=Travis%20CI&logo=travis)](https://travis-ci.com/chmorgan/sharppcap)
[![](https://codecov.io/gh/chmorgan/sharppcap/branch/master/graph/badge.svg)](https://codecov.io/gh/chmorgan/sharppcap)
[![](https://badges.gitter.im/SharpPcap/community.svg)](https://gitter.im/SharpPcap/community)

# sharppcap
Fully managed, cross platform (Windows, Mac, Linux) .NET library for capturing packets from live and file based devices

The official SharpPcap repository.

# Features
Note that packet dissection and creation was split from SharpPcap some years ago into a separate project, [Packet.Net](https://github.com/chmorgan/packetnet). See the Packet.Net page for a full list of supported packet formats.

* On Linux, support for [libpcap](http://www.tcpdump.org/manpages/pcap.3pcap.html)

* On Windows, support for:
  * Npcap (formerly WinPcap) extensions, see [Npcap API guide](https://nmap.org/npcap/guide/npcap-devguide.html#npcap-api)

* Live device lists
* Statistics
* Reading packets from Live Devices (actual network devices) and Offline Devices (Capture files)
* Support for Berkley Packet Filters
* Dumping packets to Pcap files.

# Examples
See the [Examples](https://github.com/chmorgan/sharppcap/tree/master/Examples) folder for a range of examples using SharpPcap

# CI support
We have support for a number of CI systems for a few reasons:

* Diversity of CI systems in case one of them shuts down
* Examples in case you'd like to customize SharpPcap and make use of one of these CI systems for internal builds. Note that we assume you are following the license for the library.

# Releases
SharpPcap is released via nuget

