
namespace Lego.Ev3.Framework
{

    /// <summary>
    /// Device Types that can be connected to the brick inputs and/or outputs
    /// </summary>
    public enum DeviceType
    {
        // NXT devices

        /// <summary>
        /// NXT Touch sensor
        /// </summary>
        NxtTouchSensor = 1,
        /// <summary>
        /// NXT Light sensor
        /// </summary>
        NxtLightSensor = 2,
        /// <summary>
        /// NXT Sound sensor
        /// </summary>
        NxtSoundSensor = 3,
        /// <summary>
        /// NXT Color sensor
        /// </summary>
        NxtColorSensor = 4,
        /// <summary>
        /// NXT Ultrasonic sensor
        /// </summary>
        NxtUltrasonicSensor = 5,
        /// <summary>
        ///  NXT Temperature sensor
        /// </summary>
        NxtTemperatureSensor = 6,

        //Ev3 Devices
        /// <summary>
        /// EV3 Large Motor
        /// </summary>
        LargeMotor = 7,
        /// <summary>
        /// EV3 Medium Motor
        /// </summary>
        MediumMotor = 8,

        //9 - 13 are FREE

        /// <summary>
        /// Custom output device
        /// </summary>
        Output3rdPartyDevice = 14,

        //15 = FREE

        /// <summary>
        /// EV3 Touch sensor
        /// </summary>
        TouchSensor = 16,

        //17-20 = FREE
        /// <summary>
        /// Test Device
        /// </summary>
        Test = 21,
        //22-27 = FREE

        /// <summary>
        /// Custom input device
        /// </summary>
        Input3rdPartyDevice = 28,

        /// <summary>
        /// EV3 Color sensor
        /// </summary>
        ColorSensor = 29,
        /// <summary>
        /// EV3 Ultrasonic sensor
        /// </summary>
        UltrasonicSensor = 30,
        /// <summary>
        /// EV3 Gyroscope sensor
        /// </summary>
        GyroscopeSensor = 32,
        /// <summary>
        /// EV3 IR sensor
        /// </summary>
        InfraredSensor = 33,
      

        //34 -98 = FREE

        /// <summary>
        /// Energy Meter
        /// </summary>
        EnergyMeter = 99,
        /// <summary>
        /// IIC
        /// </summary>
        IIC = 100,
        /// <summary>
        /// NXT Test device
        /// </summary>
        NxtTest = 101

        //102-120 = FREE
    }
}
