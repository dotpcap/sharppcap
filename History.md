
## History

Tamir Gal started the SharpPcap project around 2004\. He wanted to use WinPcap in a .NET application while working on his final project for university. The project involved analyzing and decoding VoIP traffic and he wanted to keep coding simple with C# which has time saving features like garbage collection. Accessing the WinPcap API from .NET seemed to be quite a popular requirement, and he found some useful projects on CodeProject's website that let you do just that.

The projects available at the time did not lend themselves to being used to capture and analyze traffic. Some had mixed ui and capture code, others had reimplemented some of WinPcap's functions in C# and others lacked features such as offline file reading or source code.

And so, Tamir decided to start his own library for the task. Several versions in the 1.x series were released. Development slowed towards mid-2007 when the last version in the 1.x series was released, SharpPcap 1.6.2.

Chris Morgan took over development of SharpPcap in November of 2008\. Since then SharpPcap has had major internal rewrites and API improvements including Linux, Mac support.

In late February 2010 SharpPcap v3.0 was released. This release represents a rewrite of SharpPcap's packet parsers. Packet parsing functionality was broken out into a new library, [Packet.Net](http://packetnet.sf.net). SharpPcap takes care of interfacing with libpcap/winpcap and Packet.Net takes care of packet dissection and creation. The details of Packet.Net's architecture will be discussed later in the turotial.

SharpPcap v3.5 was released February 1st, 2011\. The 3.5 release contained significant API changes as well as WinPcap remote capture and AirPcap support.

SharpPcap v4.0 was released September 13, 2011\. The 4.0 release contains significant performance improvements due to contributions from Michael Giagnocavo. SharpPcap 4.0 is >50% faster than the last revision, v3.7\. It also contains API cleanup for reading and writing to capture files in the form of new CaptureFileWriterDevice and CaptureFileReaderDevice that replaces an older and much more confusing way of writing to capture files and makes reading and writing analogous.

SharpPcap v4.2 was released January 14th, 2013\. 4.2 adds support for IEEE 802.1Q vlan tags, corrects access modifiers in Packet to enable 3rd party assemblies to use this class and introduces Packet.Extract(). Packet.Extract() replaces per-packet GetEncapsulated() methods, now deprecated, with a single high level routine that is a part of the base class of all captured packets.

