using System;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Management;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Media;
using NativeWifi;
using System.Text;
using System.Net.NetworkInformation;

namespace pcdiagnostic
{
    public partial class Form1 : Form
    {
        private static WlanClient client;

        public Form1()
        {
            InitializeComponent();

            ManagementObjectSearcher myModelObject = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            foreach (ManagementObject obj in myModelObject.Get())
            {
                String modelText = obj["Model"].ToString();
                modelLabel.Text = modelText;
            }

            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                String cpuText = obj["Name"].ToString() + " " + obj["CurrentClockSpeed"].ToString() + "GHz";
                cpuLabel.Text = $"{cpuText}";
            }

            ManagementObjectSearcher myMemoryObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject obj in myMemoryObject.Get())
            {
                String memoryAmountInKB = obj["TotalVisibleMemorySize"].ToString();
                String memoryText = (Convert.ToSingle(memoryAmountInKB) / 1048576).ToString("0.0");

                memoryLabel.Text = $"{memoryText} GB";
            }


            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in myVideoObject.Get())
            {
                String gpuText = obj["Name"].ToString();
                gpuLabel.Text += $"{gpuText} \n";

                displayLabel.Text = obj["CurrentHorizontalResolution"].ToString() + " x " + obj["CurrentVerticalResolution"].ToString();
            }


            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Fixed)
                {
                    storageLabel.Text += (d.TotalSize / 1000000000).ToString() + "GB \n";

                }
            }

            try
            {
                BatteryInformation cap = BatteryInfo.GetBatteryInformation();
                batteryLabel.Text = (Convert.ToDouble(cap.FullChargeCapacity) / Convert.ToDouble(cap.DesignedMaxCapacity) * 100).ToString("#.#") + "%";
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                batteryLabel.Text = "Not found";
            }

            ManagementObjectSearcher myNetworkObject = new ManagementObjectSearcher("select * from Win32_NetworkAdapterConfiguration");

            bluetoothLabel.Text = "Bluetooth not found";
            bluetoothLabel.ForeColor = Color.Red;

            foreach (ManagementObject obj in myNetworkObject.Get())
            {

                if ((obj["Caption"].ToString()).Contains("Bluetooth"))
                {
                    bluetoothLabel.Text = "Bluetooth found";
                    bluetoothLabel.ForeColor = Color.Green;
                }

            }
        }

        private void CheckNetworkStatus()
        {
            try
            {
                Ping p1 = new Ping();
                PingReply pr = p1.Send("8.8.8.8");

                if (pr.Status.ToString().Equals("Success"))
                {
                    connectionLabel.Text = "Wi-Fi Connected";
                    connectionLabel.ForeColor = Color.Green;
                }
                else
                {
                    connectionLabel.Text = "Wi-Fi Disconnected";
                    connectionLabel.ForeColor = Color.Red;
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error);
                connectionLabel.Text = "Wi-Fi Error";
                connectionLabel.ForeColor = Color.Red;
            }

            

            /*
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                try
                {              
                    if (wlanIface.InterfaceState == Wlan.WlanInterfaceState.Connected)
                    {
                        connectionLabel.Text = "WiFi Connected";
                        connectionLabel.ForeColor = Color.Green;
                    }
                    else
                    {
                        connectionLabel.Text = "WiFi Disconnected";
                        connectionLabel.ForeColor = Color.Red;
                    }
                } catch (Exception error)
                {
                    System.Diagnostics.Debug.WriteLine(error);
                    connectionLabel.Text = "WiFi Error";
                    connectionLabel.ForeColor = Color.Red;
                }
                
                
            }
            */
        }

        private void wifiButton_Click(object sender, EventArgs e)
        {
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    if (GetStringForSSID(network.dot11Ssid) == "NETGEAR26-5G")
                    {
                        
                        string ssid = "NETGEAR26-5G";
                        byte[] ssidBytes = System.Text.Encoding.Default.GetBytes(ssid);
                        string ssidHex = BitConverter.ToString(ssidBytes);
                        ssidHex = ssidHex.Replace("-", "");
                        string profileName = "NETGEAR26-5G";
                        string mac = ssidHex;
                        string key = "icyvalley851";
                        string profile = string.Format("<?xml version=\"1.0\"?><WLANProfile   xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name>   <SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig>  <connectionType>ESS</connectionType><MSM><security><authEncryption><authentication>WPA2PSK</authentication><encryption>AES</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>passPhrase</keyType><protected>false</protected><keyMaterial>{2}</keyMaterial></sharedKey><keyIndex>0</keyIndex></security></MSM></WLANProfile>",
                        profileName, mac, key);
                        wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profile, true);
                        wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);

                    }
                }
            }
        }

        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }


        private void checkButton_Click(object sender, EventArgs e)
        {
            CheckNetworkStatus();
        }

        private static FilterInfoCollection filterInfoCollection;
        private static VideoCaptureDevice videoCaptureDevice;

        private void Form1_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
            {
                cboCamera.Items.Add(filterInfo.Name);
            }

            cboCamera.SelectedIndex = 0;

            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);

            client = new WlanClient();

            CheckNetworkStatus();

            backgroundWorker1.RunWorkerAsync();

           
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice.IsRunning == false)
            {
                videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
                videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                videoCaptureDevice.Start();
                picboxCamera.Show();
            }
            else
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();
                picboxCamera.Hide();
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (picboxCamera.Image != null)
            {
                picboxCamera.Image.Dispose();
            }

            picboxCamera.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCaptureDevice.IsRunning == true)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();

            }
        }

        private void soundButton_Click(object sender, EventArgs e)
        {
            string fullPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "testsound.wav");
            SoundPlayer testSound = new SoundPlayer(fullPath);                                   
            testSound.Play();
        }

        private void spotButton_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void keyboardButton_Click(object sender, EventArgs e)
        {
            new KeyboardTest().Show();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ManagementObjectSearcher myWindowsObject = new ManagementObjectSearcher("select * from SoftwareLicensingProduct WHERE LicenseStatus = 1");
            using (ManagementObjectCollection obj = myWindowsObject.Get())
            {
                e.Result = obj.Count > 0;
            }
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (Convert.ToBoolean(e.Result))
            {
                windowsLabel.Text = "Activated";
                windowsLabel.ForeColor = Color.Green;
            } else
            {
                windowsLabel.Text = "Not Activated";
                windowsLabel.ForeColor = Color.Red;
            }
        }
        

    }
}
