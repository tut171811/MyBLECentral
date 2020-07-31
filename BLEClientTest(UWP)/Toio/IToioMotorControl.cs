using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Automation.Peers;

namespace BLEClientTest_UWP_.Toio
{
    interface IToioMotorControl
    {
        //TODO
        //加減速指定付きモーター制御は未実装

        #region Write

        /// <summary>
        /// Write motor control information.
        /// Move motor with specified velocity until write next motor control information.
        /// </summary>
        /// <param name="leftMotorRotationDirection"></param>
        /// <param name="leftMotorVelocity"></param>
        /// <param name="rightMotorRotationDirection"></param>
        /// <param name="rightMotorVelocity"></param>
        void WriteMotorControl(MotorRotationDirection leftMotorRotationDirection, byte leftMotorVelocity, MotorRotationDirection rightMotorRotationDirection, byte rightMotorVelocity);

        /// <summary>
        /// Write motor control information.
        /// Move motor for [duration] milli seconds.
        /// </summary>
        /// <param name="leftMotorRotationDirection"></param>
        /// <param name="leftMotorVelocity"></param>
        /// <param name="rightMotorRotationDirection"></param>
        /// <param name="rightMotorVelocity"></param>
        /// <param name="duration">[ms]. 0 means move forever</param>
        void WriteMotorControl(MotorRotationDirection leftMotorRotationDirection, byte leftMotorVelocity, MotorRotationDirection rightMotorRotationDirection, byte rightMotorVelocity, byte duration);

        /// <summary>
        /// Move to destination and rotation.
        /// </summary>
        /// <param name="controlId">This value include motor control response.</param>
        /// <param name="timeOut">[s]. 0 means 10s. cannot set no timeout.</param>
        /// <param name="movingType"></param>
        /// <param name="maxMotorVelocity"></param>
        /// <param name="accelerationType"></param>
        /// <param name="writeMode"></param>
        /// <param name="destinationX">255 means the same as when writing.</param>
        /// <param name="destinationY">255 means the same as when writing.</param>
        /// <param name="angleInfo"></param>
        /// <param name="destinationEularAngle">Toio angle after moved to destination. Use eular angle.</param>
        void WriteDestination(byte controlId, byte timeOut, MovementType movementType, byte maxMotorVelocity, MotorAccelerationType accelerationType, byte destinationX, byte destinationY, WriteMode writeMode = WriteMode.Append, AngleInformation angleInfo = AngleInformation.NoRotation, byte destinationTheta = 0);

        /// <summary>
        /// Move to destination and rotation if it needs.
        /// </summary>
        /// <param name="controlId">This value include motor control response.</param>
        /// <param name="timeOut">[s]. 0 means 10s. cannot set no timeout.</param>
        /// <param name="movingType"></param>
        /// <param name="maxMotorVelocity"></param>
        /// <param name="accelerationType"></param>
        /// <param name="writeMode"></param>
        /// <param name="dest"></param>
        void WriteDestination(byte controlId, byte timeout, MovementType movementType, byte maxMotorVelocity, MotorAccelerationType accelerationType, Destination dest, WriteMode writeMode = WriteMode.Append);

        /// <summary>
        /// Move to destination and rotation if it needs.
        /// </summary>
        /// <param name="controlId">This value include motor control response.</param>
        /// <param name="timeOut">[s]. 0 means 10s. cannot set no timeout.</param>
        /// <param name="movementTypr"></param>
        /// <param name="maxMotorVelocity"></param>
        /// <param name="accelerationType"></param>
        /// <param name="writeMode"></param>
        /// <param name="dest"></param>
        /// <param name="chunk">[1-29]. # of destinations to send at the same time.</param>
        void WriteDestination(byte controlId, byte timeout, MovementType movementType, byte maxMotorVelocity, MotorAccelerationType accelerationType, List<Destination> dest, WriteMode writeMode = WriteMode.Append, int chunk = 3);

        #endregion

        #region Read/Notify

        event EventHandler<MotorControlReponse> ResponseReceived;

        MotorControlReponse ReadResponse();

        #endregion
    }

    public enum MotorRotationDirection {
        Forward = 1,
        Back = 2
    }

    public enum MovementType
    {
        MoveWhileRotatingWithGoBack = 0,
        MoveWhileRotatingOnlyAdvance = 1,
        MoveAfterRotating = 2
    }

    public enum MotorAccelerationType
    {
        Constant = 0,
        AccelerateToDestination = 1,
        SlowDownToDestination = 2,
        AccelerateToMidpobyteAndThenSlowDownToDestination = 3
    }

    public enum AngleInformation
    {
        AbsoluteAngleToDirectionWithLessRotation = 0,
        AbsoluteAngleToPositiveDirection = 1,
        AbsoluteAngleToNegativeDirection = 2,
        RelativeAngleToPositiveDirection = 3,
        RelativeAngleToNegativeDirection = 4,
        NoRotation = 5,
        SameAsWhenWritingToDirectionWithLessRotation = 6
    }

    public enum WriteMode {
        Overwrite = 0,
        Append = 1
    }

    public class Destination
    {
        public byte X { set; get; }
        public byte Y { set; get; }
        public AngleInformation AngleInfo { set; get; }
        public int Angle { set; get; }
        public Destination(byte x, byte y, AngleInformation ai, int angle)
        {
            this.X = x;
            this.Y = y;
            this.AngleInfo = ai;
            this.Angle = angle;
        }
        public Destination(byte x, byte y) : this(x, y, AngleInformation.NoRotation, 0) { }
    }

    public enum ControlType
    {
        MotorControlWithDestination = 83,
        MotorControlWithSomeDestinations = 84,
    }


    public enum ResponseType
    {
        /// <summary>
        /// 目標に到達したとき。モーターは停止する。
        /// </summary>
        Success,
        /// <summary>
        /// 指定したタイムアウト時間を経過したとき。モーターは停止する。
        /// </summary>
        Timeout,
        /// <summary>
        /// Toio IDがない場所にキューブが置かれたとき。モーターは停止する。
        /// </summary>
        ToioIdMissed,
        /// <summary>
        /// X, Y, 角度のすべてが現在と同じだったとき。書き込み操作は破棄される。
        /// </summary>
        IllegalParameter,
        /// <summary>
        /// 電源を切られたとき。応答が通知された後，電源が切れる。
        /// </summary>
        IllegalState,
        /// <summary>
        /// 目標指定付きモーター制御以外のモーター制御が書き込まれたとき。実行中のモーター制御は終了し，新たに書き込まれたモーター制御が実行される。
        /// </summary>
        ReceiveOtherWriting,
        /// <summary>
        /// 指定したモーターの最大速度指示値が8未満のとき。書き込み操作は破棄される。
        /// </summary>
        NotSupported,
        /// <summary>
        /// 書き込み操作の追加ができないとき。書き込み操作は破棄される。
        /// </summary>
        CannotAppend
    }

    public class MotorControlReponse : EventArgs
    {
        public ControlType ControlType { private set; get; }
        public byte ControlId { private set; get; }
        public ResponseType ResponseType { private set; get; }
        public MotorControlReponse(ControlType ct, byte id, ResponseType rt)
        {
            this.ControlType = ct;
            this.ControlId = id;
            this.ResponseType = rt;
        }
    }
}
