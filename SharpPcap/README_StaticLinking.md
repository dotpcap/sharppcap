# Lansweeper SharpPcap

This is a fork of the original SharpPcap project, which is a cross-platform packet capture framework for the .NET environment.
The original project can be found [here](https://github.com/dotpcap/sharppcap)

This fork is maintained by Lansweeper and can be found [here](https://github.com/Lansweeper/sharppcap)

> This is the a special version to support static linking on UNIX. For the default version with dynamic linking, see `Lansweeper.SharpPcap`.

## Changes to the original project
- Allow for AoT compilation
- Added support for ARM64
- Better abstractions and better testability
- Removed netstandard2.0 support

SharpPcap is a cross-platform (Windows, Mac, Linux) packet capture framework for the .NET environment. 
It provides an API for capturing, injecting, analyzing, and building packets using any .NET language such as C# and VB.NET.

## Features

- Cross-platform support (Windows, Mac, Linux)
- Capture and inject packets
- Analyze and build packets
- Easy-to-use API

## Installation

You can install the package via NuGet Package Manager:


## Usage

Here is a simple example of how to use SharpPcap:

```cs
using SharpPcap; 
using PacketDotNet;


// List available devices 
var devices = CaptureDeviceList.Instance; 
foreach (var dev in devices) { 
    Console.WriteLine($"{dev.Name} - {dev.Description}"); 
}

// Open the first device
var device = devices[0];
device.Open();

// Capture packets
device.OnPacketArrival += new PacketArrivalEventHandler(Device_OnPacketArrival);
device.StartCapture();

Console.WriteLine("Press any key to stop...");
Console.ReadKey();

device.StopCapture();
device.Close();


static void Device_OnPacketArrival(object sender, CaptureEventArgs e)
{
    var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
    Console.WriteLine(packet.ToString());
}

```

## Documentation

For more detailed documentation, please visit the [project's GitHub page](https://github.com/Lansweeper/sharppcap).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## TODO
- Nullability
- replace [DllImport] with [ImportLibrary]