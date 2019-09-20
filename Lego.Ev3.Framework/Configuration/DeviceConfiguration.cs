using Lego.Ev3.Framework.Configuration;
using Lego.Ev3.Framework.Internals;
using System;
using System.Collections.Generic;

namespace Lego.Ev3.Framework
{
    internal static class DeviceConfiguration
    {
        static DeviceConfiguration() { }

        public static void Connect(IOPort brickPorts, List<DeviceOptions> devices)
        {
            foreach (DeviceOptions device in devices)
            {
                ChainLayer layer = device.Layer;

                switch (device.Type)
                {
                    case DeviceType.MediumMotor:
                        {
                            OutputPortName port = (OutputPortName)Enum.Parse(typeof(OutputPortName), device.Port, true);
                            int portNumber = port.AbsolutePortNumber(layer);
                            LoadMediumMotor(device.Id).Connect(brickPorts.Output.Ports[portNumber]);
                            break;
                        }
                    case DeviceType.LargeMotor:
                        {
                            OutputPortName port = (OutputPortName)Enum.Parse(typeof(OutputPortName), device.Port, true);
                            int portNumber = port.AbsolutePortNumber(layer);
                            LoadLargeMotor(device.Id).Connect(brickPorts.Output.Ports[portNumber]);
                            break;
                        }
                    case DeviceType.TouchSensor:
                        {
                            InputPortName port = (InputPortName)Enum.Parse(typeof(InputPortName), device.Port, true);
                            int portNumber = port.AbsolutePortNumber(layer);
                            LoadTouchSensor(device.Id).Connect(brickPorts.Input.Ports[portNumber]);
                            break;
                        }
                    case DeviceType.ColorSensor:
                        {
                            InputPortName port = (InputPortName)Enum.Parse(typeof(InputPortName), device.Port, true);
                            int portNumber = port.AbsolutePortNumber(layer);
                            LoadColorSensor(device.Id).Connect(brickPorts.Input.Ports[portNumber]);
                            break;
                        }
                    case DeviceType.GyroscopeSensor:
                        {
                            InputPortName port = (InputPortName)Enum.Parse(typeof(InputPortName), device.Port, true);
                            int portNumber = port.AbsolutePortNumber(layer);
                            LoadGyroscopeSensor(device.Id).Connect(brickPorts.Input.Ports[portNumber]);
                            break;
                        }
                    case DeviceType.InfraredSensor:
                        {
                            InputPortName port = (InputPortName)Enum.Parse(typeof(InputPortName), device.Port, true);
                            int portNumber = port.AbsolutePortNumber(layer);
                            LoadInfraredSensor(device.Id).Connect(brickPorts.Input.Ports[portNumber]);
                            break;
                        }
                    case DeviceType.UltrasonicSensor:
                        {
                            InputPortName port = (InputPortName)Enum.Parse(typeof(InputPortName), device.Port, true);
                            int portNumber = port.AbsolutePortNumber(layer);
                            LoadUltrasonicSensor(device.Id).Connect(brickPorts.Input.Ports[portNumber]);
                            break;
                        }
                }
            }
        }

        public static DeviceOptions LoadDevice(string id, DeviceType type)
        {
            DeviceOptions device = Brick.Options.Devices.Find(t => t.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            if (device == null) throw new ArgumentException("No device found for given id");
            if (type != device.Type) throw new ArgumentException("Device type is invalid");
            return device;
        }


        #region input devices
        public static TouchSensor LoadTouchSensor(string id)
        {
            DeviceOptions device = LoadDevice(id, DeviceType.TouchSensor);
            TouchSensor obj = (device.Mode != null && Enum.IsDefined(typeof(TouchSensorMode), device.Mode)) ? new TouchSensor((TouchSensorMode)Enum.Parse(typeof(TouchSensorMode), device.Mode, true)) : new TouchSensor();
            obj.Id = id;
            return obj;
        }

        public static ColorSensor LoadColorSensor(string id)
        {
            DeviceOptions device = LoadDevice(id, DeviceType.ColorSensor);
            return new ColorSensor { Id = id};
        }

        public static GyroscopeSensor LoadGyroscopeSensor(string id)
        {
            DeviceOptions device = LoadDevice(id, DeviceType.GyroscopeSensor);
            return new GyroscopeSensor { Id = id };
        }

        public static InfraredSensor LoadInfraredSensor(string id)
        {
            DeviceOptions device = LoadDevice(id, DeviceType.InfraredSensor);
            return new InfraredSensor { Id = id };
        }

        public static UltrasonicSensor LoadUltrasonicSensor(string id)
        {
            DeviceOptions device = LoadDevice(id, DeviceType.UltrasonicSensor);
            return new UltrasonicSensor { Id = id };
        }
        #endregion

        #region output devices

        public static LargeMotor LoadLargeMotor(string id)
        {
            DeviceOptions device = LoadDevice(id, DeviceType.LargeMotor);
            LargeMotor motor = (device.Mode != null && Enum.IsDefined(typeof(Polarity), device.Mode)) ? new LargeMotor((Polarity)Enum.Parse(typeof(Polarity), device.Mode, true)) : new LargeMotor();
            motor.Id = id;
            return motor;
        }

        public static MediumMotor LoadMediumMotor(string id)
        {
            DeviceOptions device = LoadDevice(id, DeviceType.MediumMotor);
            MediumMotor motor = (device.Mode != null && Enum.IsDefined(typeof(Polarity), device.Mode)) ? new MediumMotor((Polarity)Enum.Parse(typeof(Polarity), device.Mode, true)) : new MediumMotor();
            motor.Id = id;
            return motor;
        }

        #endregion
    }
}
