using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace BLEClientTest_UWP_.BLEPeripheral_Core
{
    class BLEDeviceGetterFromAdvertisePacket : IBLEDeviceGetter
    {
        private readonly BluetoothLEAdvertisementWatcher adbWatcher = new BluetoothLEAdvertisementWatcher();
        private readonly CountdownEvent condition = new CountdownEvent(1);
        private Guid targetUuid;
        private BluetoothLEDevice bleDevice;
        public BluetoothLEDevice Get(Guid serviceUuid)
        {
            this.targetUuid = serviceUuid;
            this.adbWatcher.Received += AdbWatcher_Received;
            this.adbWatcher.ScanningMode = BluetoothLEScanningMode.Passive;
            //this.adbWatcher.ScanningMode = BluetoothLEScanningMode.Active;
            this.adbWatcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromSeconds(2);
            this.adbWatcher.Start();
            Debug.WriteLine("scan start");

            this.condition.Wait();

            return this.bleDevice;
        }

        private async void AdbWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if (args.Advertisement.ServiceUuids.Contains(this.targetUuid))
            {
                Debug.WriteLine("detect");
                this.adbWatcher.Stop();
                this.adbWatcher.Received -= AdbWatcher_Received;

                this.bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress); // MAC Address -> BLEDevice
                this.condition.Signal();
                await Task.Delay(1000);
                this.condition.Reset();
            }
        }
    }
}
