using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SharpPcap.AirPcap;

namespace AirPcapLedControl
{
    class Program
    {
        private static bool shouldRun;

        static void Main(string[] args)
        {
            var devices = AirPcapDeviceList.Instance;

            if (devices.Count == 0)
            {
                Console.WriteLine("No devices found, are you running as admin(if in Windows), or root(if in Linux/Mac)?");
                return;
            }

            Console.WriteLine("Available AirPcap devices:");
            for (var i = 0; i < devices.Count; i++)
            {
                Console.WriteLine("[{0}] - {1}", i, devices[i].Name);
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            var devIndex = int.Parse(Console.ReadLine());

            var device = devices[devIndex];

            device.Open();

            // start up a thread for scrolling the leds
            shouldRun = true;
            var t = new Thread(new ParameterizedThreadStart(ScrollingThread));
            t.Start(device);

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            shouldRun = false;

        }

        private static void ScrollingThread(object obj)
        {
            var device = (AirPcapDevice)obj;

            var numLeds = device.LedCount;
            Console.WriteLine("{0} leds available on this adapter", numLeds);

            // scroll through the leds during them on and off until the user interrupts
            int ledIndex = 0;
            while(shouldRun)
            {
                // turn the led on
                device.Led(ledIndex, AirPcapDevice.LedState.On);

                // wait for a little while
                Thread.Sleep(500);

                // turn the led off
                device.Led(ledIndex, AirPcapDevice.LedState.Off);

                // wait for a little while
                Thread.Sleep(500);

                // go to the next led
                ledIndex++;

                // wrap around if the index is past the end of the led array
                if(ledIndex == numLeds)
                {
                    ledIndex = 0;
                }
            }
        }
    }
}
