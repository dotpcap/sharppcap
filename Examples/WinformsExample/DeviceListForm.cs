using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpPcap;

namespace WinformsExample
{
    public partial class DeviceListForm : Form
    {
        public DeviceListForm()
        {
            InitializeComponent();
        }

        private void DeviceListForm_Load(object sender, EventArgs e)
        {
            foreach (var dev in CaptureDeviceList.Instance)
            {
                var str = String.Format("{0} {1}", dev.Name, dev.Description);
                deviceList.Items.Add(str);
            }
        }

        public delegate void OnItemSelectedDelegate(int itemIndex);
        public event OnItemSelectedDelegate OnItemSelected;

        public delegate void OnCancelDelegate();
        public event OnCancelDelegate OnCancel;

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (deviceList.SelectedItem != null)
            {
                OnItemSelected(deviceList.SelectedIndex);
            }
        }

        private void deviceList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (deviceList.SelectedItem != null)
            {
                OnItemSelected(deviceList.SelectedIndex);
            }
        }
    }
}
