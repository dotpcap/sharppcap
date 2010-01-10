using System;
using System.Collections.Generic;

namespace SharpPcap.Test.Example1
{
    /// <summary>
    /// Obtaining the device list
    /// </summary>
    public class IfListAdv
    {
        /// <summary>
        /// Obtaining the device list
        /// </summary>
        public static void Main(string[] args)
        {
            string ver = SharpPcap.Version.VersionString;
            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}, Example1.IfList.cs", ver);

            /* Retrieve the device list */
            PcapDeviceList devices = PcapDeviceList.Instance;

            /*If no device exists, print error */
            if(devices.Count<1)
            {
                Console.WriteLine("No device found on this machine");
                return;
            }
            
            Console.WriteLine("\nThe following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------\n");

            /* Scan the list printing every entry */
            foreach(PcapDevice dev in devices)
                Console.WriteLine("{0}\n",dev.ToString());
            
            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }
    }
}
