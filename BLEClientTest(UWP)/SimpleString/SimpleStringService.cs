using BLEClientTest_UWP_.BLEPeripheral_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace BLEClientTest_UWP_.SimpleString
{
    class SimpleStringService
    {
        public event TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> ValueChanged;
        public event EventHandler OnAvailable;
        public bool IsAvailable { private set; get; } = false;

        //private readonly Guid serviceUuid = new Guid("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
        private readonly Guid serviceUuid = new Guid("3773c59c-bf25-46d6-9d23-eeb34f87997e");
        //private readonly Guid charaUuid = new Guid("beb5483e-36e1-4688-b7f5-ea07361b26a8");
        private readonly Guid charaUuid = new Guid("f7a6736c-7644-4daa-be85-29cb174b6df2");

        private GattCharacteristic chara;

        public SimpleStringService()
        {
            this.OnAvailable += (s, e) =>
            {
                Debug.WriteLine("Available", this);
            };

            _ = Task.Factory.StartNew(async () =>
            {
                var blePeripheral = new BLEPeripheral(serviceUuid);
                if (await blePeripheral.Initialize())
                {
                    this.chara = await blePeripheral.GetCharacteristicAsync(charaUuid);
                    Debug.WriteLine("device available");
                    this.IsAvailable = true;
                    this.OnAvailable.Invoke(this, EventArgs.Empty);
                }
            });
        }

        // Notifyできるようにする
        public void SetNotify(bool isActivate)
        {
            //var chara = await GetCharacteristic(serviceUuid, charaUuid);
            var properties = chara.CharacteristicProperties;
            if (properties.HasFlag(GattCharacteristicProperties.Notify))
            {
                if (isActivate)
                {
                    Debug.WriteLine("event add");
                    chara.ValueChanged += this.ValueChanged;
                }
                else
                {
                    Debug.WriteLine("event remove");
                    chara.ValueChanged -= this.ValueChanged;
                }
            }
            else
            {
                throw new NotSupportedException("Notify is not supported.");
            }
        }

        // 即座にNotifyの値を取得する？
        public async Task<string> ReadAsync()
        {
            var properties = chara.CharacteristicProperties;
            if (properties.HasFlag(GattCharacteristicProperties.Read))
            {
                //GattReadResult r = chara.ReadValueAsync().AsTask<GattReadResult>().Result;
                var r = await chara.ReadValueAsync(BluetoothCacheMode.Uncached);

                if (r.Status == GattCommunicationStatus.Success)
                {
                    var s = BufferToString(r.Value);
                    Debug.WriteLine($"Read: {s}");
                    return s;
                }
                else
                {
                    return "Failed to read.";
                }
            }
            else
            {
                throw new NotSupportedException("Read is not supported.");
            }
        }

        public async Task<bool> WriteAsync(string str)
        {
            //var chara = await GetCharacteristic(serviceUuid, charaUuid);
            var properties = chara.CharacteristicProperties;
            if (properties.HasFlag(GattCharacteristicProperties.Write))
            {
                var writer = new DataWriter();
                writer.ByteOrder = ByteOrder.LittleEndian;
                writer.WriteString(str);
                var r = await chara.WriteValueAsync(writer.DetachBuffer());

                return r == GattCommunicationStatus.Success;
            }
            else
            {
                throw new NotSupportedException("Write is not supported.");
            }
        }

        public string BufferToString(IBuffer buf)
        {
            var reader = DataReader.FromBuffer(buf);
            byte[] input = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(input);
            return Encoding.UTF8.GetString(input);
        }
    }
}
