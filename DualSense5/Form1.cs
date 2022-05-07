using System;
using System.Linq;
using Device.Net;
using Hid.Net.Windows;
using Usb.Net.Windows;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace DualSense5
{
    public partial class Form1 : Form
    {
        private DualSense_Base dualSense;
        private ConnectedDeviceDefinition[] DualSenseList;
        private int DualSenseCount;

        public Form1()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                // Register controller 
                WindowsUsbDeviceFactory.Register(null, null);
                WindowsHidDeviceFactory.Register(null, null);

                // Search DualSense Controller
                var DeviceSearcherTask = DeviceManager.Current.GetConnectedDeviceDefinitionsAsync(new FilterDeviceDefinition { DeviceType = DeviceType.Hid, VendorId = 1356, ProductId = 3302 });
                DeviceSearcherTask.Wait();

                // List all DualSense
                DualSenseList = DeviceSearcherTask.Result.ToArray();
                DualSenseCount = DualSenseList.Length;

                if (DualSenseCount <= 0)
                    throw new Exception("Unable to find Dual Sense 5!");

                var DualSenseDevice = DeviceManager.Current.GetDevice(DualSenseList[0]);
                if (DualSenseList[0].ReadBufferSize == 64)
                    dualSense = new DualSense_USB(DualSenseDevice);
                else if (DualSenseList[0].ReadBufferSize == 78)
                    dualSense = new DualSense_Bluetooth(DualSenseDevice);
                else
                    throw new NotImplementedException("This application only supports the DS5 controller connected via USB");

                // Set default trigger (normal)
                dualSense.SetLeftAdaptiveTrigger(DualSense_Base.NormalTrigger);
                dualSense.SetRightAdaptiveTrigger(DualSense_Base.NormalTrigger);
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button4.Text = $"Vibrate {trackBar1.Value}Hz";
            label1.Text = $"min: {trackBar1.Minimum}; current: {trackBar1.Value}; max: {trackBar1.Maximum}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                dualSense.SetLeftAdaptiveTrigger(DualSense_Base.HardestTrigger);
                dualSense.SetRightAdaptiveTrigger(DualSense_Base.HardestTrigger);
            });            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                dualSense.SetLeftAdaptiveTrigger(DualSense_Base.VeryHardTrigger);
                dualSense.SetRightAdaptiveTrigger(DualSense_Base.VeryHardTrigger);
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                dualSense.SetLeftAdaptiveTrigger(DualSense_Base.HardTrigger);
                dualSense.SetRightAdaptiveTrigger(DualSense_Base.HardTrigger);
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                dualSense.SetLeftAdaptiveTrigger(DualSense_Base.RigidTrigger);
                dualSense.SetRightAdaptiveTrigger(DualSense_Base.RigidTrigger);
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                dualSense.SetLeftAdaptiveTrigger(DualSense_Base.NormalTrigger);
                dualSense.SetRightAdaptiveTrigger(DualSense_Base.NormalTrigger);
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int value = trackBar1.Value;
            Task.Run(() =>
            {
                dualSense.SetLeftAdaptiveTrigger(DualSense_Base.VibrateTrigger((byte)value));
                dualSense.SetRightAdaptiveTrigger(DualSense_Base.VibrateTrigger((byte)value));
            });
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            button4.Text = $"Vibrate {trackBar1.Value}Hz";
            label1.Text = $"min: {trackBar1.Minimum}; current: {trackBar1.Value}; max: {trackBar1.Maximum}";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Task.Run(() =>
            {
                dualSense.SetLeftAdaptiveTrigger(DualSense_Base.NormalTrigger);
                dualSense.SetRightAdaptiveTrigger(DualSense_Base.NormalTrigger);
            });
            Environment.Exit(1);
        }
    }
}
