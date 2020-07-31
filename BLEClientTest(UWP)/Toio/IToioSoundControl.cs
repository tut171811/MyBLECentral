using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLEClientTest_UWP_.Toio
{
    interface IToioSoundControl
    {
        void PlaySoundEffect(SoundEffect se);
    }

    public enum SoundEffect
    {
        Enter = 0,
        Selected = 1,
        Cancel = 2,
        Cursor = 3,
        MatIn = 4,
        MatOut = 5,
        Get1 = 6,
        Get2 = 7,
        Get3 = 8,
        Effect1 = 9,
        Effect2 = 10
    }
}
