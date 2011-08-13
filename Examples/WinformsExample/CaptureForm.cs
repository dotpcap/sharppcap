using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SharpPcap;
using PacketDotNet;

namespace WinformsExample
{
    public partial class CaptureForm : Form
    {
        /// <summary>
        /// When true the background thread will terminate
        /// </summary>
        /// <param name="args">
        /// A <see cref="System.String"/>
        /// </param>
        private bool BackgroundThreadStop;

        /// <summary>
        /// Object that is used to prevent two threads from accessing
        /// PacketQueue at the same time
        /// </summary>
        /// <param name="args">
        /// A <see cref="System.String"/>
        /// </param>
        private object QueueLock = new object();

        /// <summary>
        /// The queue that the callback thread puts packets in. Accessed by
        /// the background thread when QueueLock is held
        /// </summary>
        private List<RawCapture> PacketQueue = new List<RawCapture>();

        /// <summary>
        /// The last time PcapDevice.Statistics() was called on the active device.
        /// Allow periodic display of device statistics
        /// </summary>
        /// <param name="args">
        /// A <see cref="System.String"/>
        /// </param>
        private DateTime LastStatisticsOutput;

        /// <summary>
        /// Interval between PcapDevice.Statistics() output
        /// </summary>
        /// <param name="args">
        /// A <see cref="System.String"/>
        /// </param>
        private TimeSpan LastStatisticsInterval = new TimeSpan(0, 0, 2);

        private System.Threading.Thread backgroundThread;

        private DeviceListForm deviceListForm;
        private ICaptureDevice device;

        public CaptureForm()
        {
            InitializeComponent();
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
        }

        private void CaptureForm_Load(object sender, EventArgs e)
        {
            deviceListForm = new DeviceListForm();
            deviceListForm.OnItemSelected += new DeviceListForm.OnItemSelectedDelegate(deviceListForm_OnItemSelected);
            deviceListForm.OnCancel += new DeviceListForm.OnCancelDelegate(deviceListForm_OnCancel);
        }

        void deviceListForm_OnItemSelected(int itemIndex)
        {
            // close the device list form
            deviceListForm.Hide();

            StartCapture(itemIndex);
        }

        void deviceListForm_OnCancel()
        {
            Application.Exit();
        }

        public class PacketWrapper
        {
            public RawCapture p;

            public int Count { get; private set; }
            public PosixTimeval Timeval { get { return p.Timeval; } }
            public LinkLayers LinkLayerType { get { return p.LinkLayerType; } }
            public int Length { get { return p.Data.Length; } }

            public PacketWrapper(int count, RawCapture p)
            {
                this.Count = count;
                this.p = p;
            }
        }

        private PacketArrivalEventHandler arrivalEventHandler;
        private CaptureStoppedEventHandler captureStoppedEventHandler;

        private void Shutdown()
        {
            if (device != null)
            {
                device.StopCapture();
                device.Close();
                device.OnPacketArrival -= arrivalEventHandler;
                device.OnCaptureStopped -= captureStoppedEventHandler;
                device = null;

                // ask the background thread to shut down
                BackgroundThreadStop = true;

                // wait for the background thread to terminate
                backgroundThread.Join();

                // switch the icon back to the play icon
                startStopToolStripButton.Image = global::WinformsExample.Properties.Resources.play_icon_enabled;
                startStopToolStripButton.ToolTipText = "Select device to capture from";
            }
        }

        private void StartCapture(int itemIndex)
        {
            packetCount = 0;
            device = CaptureDeviceList.Instance[itemIndex];
            packetStrings = new Queue<PacketWrapper>();
            bs = new BindingSource();
            dataGridView.DataSource = bs;
            LastStatisticsOutput = DateTime.Now;

            // start the background thread
            BackgroundThreadStop = false;
            backgroundThread = new System.Threading.Thread(BackgroundThread);
            backgroundThread.Start();

            // setup background capture
            arrivalEventHandler = new PacketArrivalEventHandler(device_OnPacketArrival);
            device.OnPacketArrival += arrivalEventHandler;
            captureStoppedEventHandler = new CaptureStoppedEventHandler(device_OnCaptureStopped);
            device.OnCaptureStopped += captureStoppedEventHandler;
            device.Open();

            // force an initial statistics update
            captureStatistics = device.Statistics;
            UpdateCaptureStatistics();

            // start the background capture
            device.StartCapture();

            // disable the stop icon since the capture has stopped
            startStopToolStripButton.Image = global::WinformsExample.Properties.Resources.stop_icon_enabled;
            startStopToolStripButton.ToolTipText = "Stop capture";
        }

        void device_OnCaptureStopped(object sender, CaptureStoppedEventStatus status)
        {
            if (status != CaptureStoppedEventStatus.CompletedWithoutError)
            {
                MessageBox.Show("Error stopping capture", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Queue<PacketWrapper> packetStrings;

        private int packetCount;
        private BindingSource bs;
        private ICaptureStatistics captureStatistics;
        private bool statisticsUiNeedsUpdate = false;

        void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            // print out periodic statistics about this device
            var Now = DateTime.Now; // cache 'DateTime.Now' for minor reduction in cpu overhead
            var interval = Now - LastStatisticsOutput;
            if (interval > LastStatisticsInterval)
            {
                Console.WriteLine("device_OnPacketArrival: " + e.Device.Statistics);
                captureStatistics = e.Device.Statistics;
                statisticsUiNeedsUpdate = true;
                LastStatisticsOutput = Now;
            }

            // lock QueueLock to prevent multiple threads accessing PacketQueue at
            // the same time
            lock (QueueLock)
            {
                PacketQueue.Add(e.Packet);
            }
        }

        private void CaptureForm_Shown(object sender, EventArgs e)
        {
            deviceListForm.Show();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (device == null)
            {
                deviceListForm.Show();
            }
            else
            {
                Shutdown();
            }
        }

        /// <summary>
        /// Checks for queued packets. If any exist it locks the QueueLock, saves a
        /// reference of the current queue for itself, puts a new queue back into
        /// place into PacketQueue and unlocks QueueLock. This is a minimal amount of
        /// work done while the queue is locked.
        ///
        /// The background thread can then process queue that it saved without holding
        /// the queue lock.
        /// </summary>
        private void BackgroundThread()
        {
            while (!BackgroundThreadStop)
            {
                bool shouldSleep = true;

                lock (QueueLock)
                {
                    if (PacketQueue.Count != 0)
                    {
                        shouldSleep = false;
                    }
                }

                if (shouldSleep)
                {
                    System.Threading.Thread.Sleep(250);
                }
                else // should process the queue
                {
                    List<RawCapture> ourQueue;
                    lock (QueueLock)
                    {
                        // swap queues, giving the capture callback a new one
                        ourQueue = PacketQueue;
                        PacketQueue = new List<RawCapture>();
                    }

                    Console.WriteLine("BackgroundThread: ourQueue.Count is {0}", ourQueue.Count);

                    foreach (var packet in ourQueue)
                    {
                        // Here is where we can process our packets freely without
                        // holding off packet capture.
                        //
                        // NOTE: If the incoming packet rate is greater than
                        //       the packet processing rate these queues will grow
                        //       to enormous sizes. Packets should be dropped in these
                        //       cases

                        var packetWrapper = new PacketWrapper(packetCount, packet);
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            packetStrings.Enqueue(packetWrapper);
                        }
                        ));

                        packetCount++;

                        var time = packet.Timeval.Date;
                        var len = packet.Data.Length;
                        Console.WriteLine("BackgroundThread: {0}:{1}:{2},{3} Len={4}",
                            time.Hour, time.Minute, time.Second, time.Millisecond, len);
                    }

                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        bs.DataSource = packetStrings.Reverse();
                    }
                    ));

                    if (statisticsUiNeedsUpdate)
                    {
                        UpdateCaptureStatistics();
                        statisticsUiNeedsUpdate = false;
                    }
                }
            }
        }

        private void UpdateCaptureStatistics()
        {
            captureStatisticsToolStripStatusLabel.Text = string.Format("Received packets: {0}, Dropped packets: {1}, Interface dropped packets: {2}",
                                                       captureStatistics.ReceivedPackets,
                                                       captureStatistics.DroppedPackets,
                                                       captureStatistics.InterfaceDroppedPackets);
        }

        private void CaptureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Shutdown();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count == 0)
                return;

            var packetWrapper = (PacketWrapper)dataGridView.Rows[dataGridView.SelectedCells[0].RowIndex].DataBoundItem;
            var packet = Packet.ParsePacket(packetWrapper.p.LinkLayerType, packetWrapper.p.Data);
            packetInfoTextbox.Text = packet.ToString(StringOutputType.VerboseColored);
        }
    }
}
