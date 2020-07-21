using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace BLEClientTest_UWP_.BLEPeripheral_Core
{
    class BLEDeviceGetterFromDeviceInformation : IBLEDeviceGetter
    {
        private readonly CountdownEvent condition = new CountdownEvent(1);

        private DeviceWatcher dw;
        private BluetoothLEDevice bleDevice;
        public BluetoothLEDevice Get(Guid serviceUuid)
        {
            // Uuid = serviceUuidのデバイスをサーチ
            string selector = "(" + GattDeviceService.GetDeviceSelectorFromUuid(serviceUuid) + ")";
            dw = DeviceInformation.CreateWatcher(selector); 
            dw.Added += Watcher_DeviceAdded;
            dw.Start();
            Debug.WriteLine("scan start");

            condition.Wait();

            return this.bleDevice;
        }

        private async void Watcher_DeviceAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // デバイス情報更新時のハンドラを解除しウォッチャーをストップ
            dw.Added -= Watcher_DeviceAdded;
            dw.Stop();

            // get Bluetooth device 
            this.bleDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);

            condition.Signal();
            await Task.Delay(1000);
            condition.Reset();
        }
    }
}
