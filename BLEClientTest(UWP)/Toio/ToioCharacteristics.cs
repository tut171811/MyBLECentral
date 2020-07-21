using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BLEClientTest_UWP_.Toio
{
    static class ToioCharacteristicExtends
    {
        private static readonly Guid IdUuid = new Guid("10B20101-5B3B-4571-9508-CF3EFCD7BBAE");
        public static Guid GetGuid(this ToioCharacteristics tc)
        {
            switch (tc) 
            {
                case ToioCharacteristics.IDInfomation:
                    return IdUuid;
                default:
                    throw new ArgumentException("The characteristic no exists.");
            }
        }
    }

    enum ToioCharacteristics
    {
        IDInfomation,
        SensorInformation,
        ButttonInformation,
        BatteryInformation,
        MotorControl,
        LightControl,
        SoundControl,
        Configuration
    }
}
