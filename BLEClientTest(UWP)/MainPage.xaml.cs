using BLEClientTest_UWP_.SimpleString;
using System;
using Windows.ApplicationModel.Chat;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace BLEClientTest_UWP_
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly SimpleStringService simpleString;

        public MainPage()
        {
            this.InitializeComponent();
            ToggleControlEnabled(false);

            this.simpleString = new SimpleStringService();
            this.simpleString.ValueChanged += SimpleString_ValueChanged;
            this.simpleString.OnAvailable += async (s, e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ToggleControlEnabled(true);
                });
            };

            this.NotifyToggleButton.Click += (s, e) =>
            {
                var isChecked = this.NotifyToggleButton.IsChecked.Value;
                AppendLog($"[Notify " + (isChecked ? "on" : "off") + "]");

                if (this.NotifyToggleButton.IsChecked.HasValue)
                    this.simpleString.SetNotify(isChecked);
            };
            this.ReadButton.Click += async (s, e) =>
            {
                AppendLog("[NotifyImmediate]");

                var str = await this.simpleString.ReadAsync();
                AppendLog(str);
            };
            this.WriteButton.Click += async (a, e) =>
            {
                var str = this.WriteBox.Text;
                AppendLog($"[Write]: {str}");

                if (await this.simpleString.WriteAsync(str))
                {
                    AppendLog("Success.");
                }
            };
        }

        private void ToggleControlEnabled(bool isEnabled)
        {
            this.ReadButton.IsEnabled = isEnabled;
            this.WriteButton.IsEnabled = isEnabled;
            this.NotifyToggleButton.IsEnabled = isEnabled;
        }

        private void AppendLog(string str)
        {
            this.LogBox.Text += str + "\n";
        }

        #region event

        private async void SimpleString_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AppendLog($"Value: {this.simpleString.BufferToString(args.CharacteristicValue)}");
            });
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            this.LogBox.Text = "[Reset]\n";
        }

        #endregion
    }
}
