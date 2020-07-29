using BLEClientTest_UWP_.BLEPeripheral_Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BLEClientTest_UWP_.Toio
{
    class ToioService
    {
        public event EventHandler OnAvailable;
        public bool IsAvailable { private set; get; } = false;

        private readonly Guid serviceUuid = new Guid("10B20100-5B3B-4571-9508-CF3EFCD7BBAE");

        private BLEPeripheral blePeripheral;

        public ToioService()
        {
            _ = Task.Factory.StartNew(async () =>
            {
                this.blePeripheral = new BLEPeripheral(serviceUuid);
                if (await blePeripheral.Initialize())
                {
                    Debug.WriteLine("device available");
                    this.IsAvailable = true;
                    this.OnAvailable.Invoke(this, EventArgs.Empty);
                }
            });
        }

        public async Task<IdInformation> GetIdInformationAsync()
        {
            if(!IsAvailable)
            {

            }

            var chara = await this.blePeripheral.GetCharacteristicAsync(ToioCharacteristics.IDInfomation.GetGuid()); ;
            return new IdInformation(chara);
        }
    }
}
