using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BLEClientTest_UWP_.BLEPeripheral_Core
{
    class BLEPeripheral
    {
        private readonly Guid serviceUuid;
        private GattDeviceService service;
        private int trial = 0;

        public BLEPeripheral(Guid serviceUuid)
        {
            this.serviceUuid = serviceUuid;
        }

        public async Task<bool> Initialize()
        {
            IBLEDeviceGetter deviceGetter = new BLEDeviceGetterFromDeviceInformation();
            //IBLEDeviceGetter deviceGetter = new BLEDeviceGetterFromAdvertisePacket();

            var bleDevice = deviceGetter.Get(this.serviceUuid);

            Debug.WriteLine($"bleDevice found: {bleDevice.DeviceInformation.Name}");

            // get service (listで返ってくるが、uuidを指定しているため、ひとつのみ返ってくる)
            var services = await bleDevice.GetGattServicesForUuidAsync(this.serviceUuid);
            if(services.Status == GattCommunicationStatus.Success)
                this.service = services.Services.First();
            else
            {
                Debug.WriteLine("cannot connect bleDevice");
                Debug.WriteLine("try to connect again");
                this.trial++;
                if (this.trial < 3)
                    await Initialize();
                else 
                    return false;
            }

            return true;
        }

        public async Task<GattCharacteristic> GetCharacteristicAsync(Guid characteristicUuid) 
        {
            if(service == null)
            {
                await Initialize();
            }

            // get characteristic (listで返ってくるが、uuidを指定しているため、ひとつのみ返ってくる)
            var characteristics = await this.service.GetCharacteristicsForUuidAsync(characteristicUuid);
            Debug.WriteLine($"# of characteristics: {characteristics.Characteristics.Count}");

            var chara = characteristics.Characteristics.First();
            var r = await chara.ReadValueAsync();
            Debug.WriteLine(r.Status);
            

            // 初回Connect時にディスクリプタの値はNone
            // notifyのために，ディスクリプタの値をNotifyにする必要がある
            if(chara.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                await chara.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            }
            else if(chara.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
            {
                await chara.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
            }

            return chara;
        }
    }
}
