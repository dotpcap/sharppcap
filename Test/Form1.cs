using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Tamir.IPLib;

namespace Test
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private PcapDeviceList devices;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);

		}
		#endregion

		private void Form1_Load(object sender, System.EventArgs e)
		{
		}
		
		private void Form1_Load2(object sender, System.EventArgs e)
		{
			//Run();
			devices = SharpPcap.GetAllDevices();
			foreach (PcapDevice device in devices)
			{
				device.PcapOpen();
			}
			Thread th = new Thread(new ThreadStart(Run));
			th.Start();
		}

		private void Run()
		{
			string s = "";
			foreach (PcapDevice device in devices)
			{
				s += device.PcapDescription;
				s += "\n";
			}
			MessageBox.Show(s);
			
			foreach (PcapDevice device in devices)
			{
				device.PcapOnPacketArrival += new Tamir.IPLib.SharpPcap.PacketArrivalEvent(device_PcapOnPacketArrival);
				device.PcapSetFilter("ip and tcp");
				device.PcapStartCapture();
			}
		}

		private void device_PcapOnPacketArrival(object sender, Tamir.IPLib.Packets.Packet packet)
		{
			Console.WriteLine(packet);
		}
	}
}
