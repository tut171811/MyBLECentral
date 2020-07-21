using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace BLEClientTest_UWP_.BLEPeripheral_Core
{
    interface IBLEDeviceGetter
    {
        BluetoothLEDevice Get(Guid serviceUuid);
    }
}
