using BLEClientTest_UWP_.BLEPeripheral_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace BLEClientTest_UWP_
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //private readonly Guid serviceUuid = new Guid("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
        private readonly Guid serviceUuid = new Guid("3773c59c-bf25-46d6-9d23-eeb34f87997e");
        //private readonly Guid charaUuid = new Guid("beb5483e-36e1-4688-b7f5-ea07361b26a8");
        private readonly Guid charaUuid = new Guid("f7a6736c-7644-4daa-be85-29cb174b6df2");

        private GattCharacteristic chara;

        public MainPage()
        {
            this.InitializeComponent();

            this.NotifyToggleButton.Click += async (s, e) =>
            {
                if (this.NotifyToggleButton.IsChecked.HasValue)
                    await ToggleNotifyAsync(this.NotifyToggleButton.IsChecked.Value);
            };
            this.ReadButton.Click += async (s, e) =>
            {
                await ReadAsync();
            };
            this.WriteButton.Click += async (a, e) =>
            {
                await WriteAsync(this.WriteBox.Text);
            };

            _ = Task.Factory.StartNew(async () =>
            {
                var blePeripheral = new BLEPeripheral(serviceUuid);
                await blePeripheral.Initialize();
                this.chara = await blePeripheral.GetCharacteristicAsync(charaUuid);
            });
        }

        private async Task AppendLog(string str)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.LogBox.Text += str + "\n";
            });
        }

        private string ReadDataToString(IBuffer buf)
        {
            var reader = DataReader.FromBuffer(buf);
            byte[] input = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(input);
            return Encoding.UTF8.GetString(input);
        }

        // Notifyできるようにする
        private async Task ToggleNotifyAsync(bool isChecked)
        {
            //var chara = await GetCharacteristic(serviceUuid, charaUuid);
            var properties = chara.CharacteristicProperties;
            if (properties.HasFlag(GattCharacteristicProperties.Notify))
            {


                await AppendLog($"[Notify " + (isChecked ? "on" : "off") + "]");
                if (isChecked)
                {
                    Debug.WriteLine("event add");
                    chara.ValueChanged += Chara_ValueChanged;
                }
                else
                {
                    Debug.WriteLine("event remove");
                    chara.ValueChanged -= Chara_ValueChanged;
                }
            }
        }

        // 即座にNotifyの値を取得する？
        private async Task ReadAsync()
        {
            //var chara = await GetCharacteristic(serviceUuid, charaUuid);
            var properties = chara.CharacteristicProperties;
            if (properties.HasFlag(GattCharacteristicProperties.Read))
            {
                await AppendLog("[NotifyImmediate]");
                //GattReadResult r = chara.ReadValueAsync().AsTask<GattReadResult>().Result;
                var r = await chara.ReadValueAsync(BluetoothCacheMode.Uncached);

                if (r.Status == GattCommunicationStatus.Success)
                {
                    await AppendLog(ReadDataToString(r.Value));
                }
                else
                {
                    await AppendLog("Failed to read value.");
                }
            }
        }

        private async Task WriteAsync(string str)
        {
            //var chara = await GetCharacteristic(serviceUuid, charaUuid);
            var properties = chara.CharacteristicProperties;
            if (properties.HasFlag(GattCharacteristicProperties.Write))
            {
                await AppendLog($"[Write]: {str}");
                var writer = new DataWriter();
                writer.ByteOrder = ByteOrder.LittleEndian;
                writer.WriteString(str);
                var r = await chara.WriteValueAsync(writer.DetachBuffer());
                if (r == GattCommunicationStatus.Success)
                {
                    await AppendLog("Success.");
                }
            }
        }

        #region event

        

        private async void Chara_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            await AppendLog($"Value: {ReadDataToString(args.CharacteristicValue)}");
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            this.LogBox.Text = "[Reset]\n";
        }

        #endregion
    }
}
