// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;

namespace Iot.Device.IL0373
{
    /// <summary>
    /// IL0373 LED matrix driver
    /// </summary>
    public partial class IL0373 : IDisposable
    {
        private SpiDevice _spiDevice;
        private GpioPin _ecs;
        private GpioPin _dc;
        private SpiDevice _sramSpiDevice;
        private GpioPin _busy_pin;


        /// <summary>
        /// The Rotation to be applied (when modules are assembled rotated way)
        /// </summary>
        public RotationType Rotation { get; set; } = RotationType.None;

        /// <summary>
        /// Sets whether the Black portion of the displayise inverted
        /// </summary>
        public bool InvertBlack { get; set; } = true;

        /// <summary>
        /// Sets whether the Red portion of the displayise inverted
        /// </summary>
        public bool InvertRed { get; set; } = true;

        /// <summary>
        /// Creates a IL0373 Device on <see paramref="spiDevice" /> to communicate over.
        /// </summary>
        /// <param name="spiDevice">The main IL0373 spi device, do not include CS pin</param>
        /// <param name="DisplaySelectPin">The Display Cable Select pin Gpio device</param>
        /// <param name="DataCommandPin">The Command/Data transfer select pin Gpio device</param>
        /// <param name="BusyPin">The Busy/Wait pin Gpio device</param>
        public IL0373(SpiDevice spiDevice,GpioPin DisplaySelectPin, GpioPin DataCommandPin, int width, int height,GpioPin BusyPin = null, SpiDevice ramBuffer = null, RotationType rotation = RotationType.None)
        {
            _spiDevice = spiDevice ?? throw new ArgumentNullException(nameof(spiDevice));
            _ecs = DisplaySelectPin ?? throw new ArgumentNullException(nameof(DisplaySelectPin)); ;
            _dc = DataCommandPin ?? throw new ArgumentNullException(nameof(DataCommandPin));
            _busy_pin = BusyPin;
            _sramSpiDevice = ramBuffer;
            Rotation = rotation;
            uint buffer_size = ((uint)width * (uint)height) / 8;

            mainBuffer = ecBuffer = new byte[buffer_size];
        }

        /// <summary>
        /// Wait for busy signal to end.
        /// </summary>
        /// <param name="WaitTime">Wait for busy pin, or timeout. 0 = use default</param>
        void WaitForCompletion(int WaitTime = 0)
        {
            Debug.WriteLine("Waiting...");
            if (_busy_pin != null)
            {
                while (_busy_pin.Read() == PinValue.High)
                {
                    System.Threading.Thread.Sleep(10); // wait for busy high
                }
            }
            else if(WaitTime > 0)
                Thread.Sleep(WaitTime);
            else
                System.Threading.Thread.Sleep(BusyWaitTime);
            Debug.WriteLine("OK!");
        }
#if DEBUG
        /// <summary>
        /// It says "Init" i bet it makes pancakes..
        /// </summary>
#else
/// <summary>
/// Initializes the device
/// </summary>
#endif
        public void Init()
        {
            _spiDevice.ConnectionSettings.ClockFrequency = IL0373.SpiClockFrequency;
            _spiDevice.ConnectionSettings.Mode = IL0373.SpiMode;
            _ecs.SetPinMode(PinMode.Output);
            _dc.SetPinMode(PinMode.Output);
            commandList(IL0398DefaultInitCode);
        }

        /// <summary>
        /// Send a list of commands to the Controller.
        /// </summary>
        /// <param name="init_code">Byte array containing command list and parameters.
        /// Format: Command, Arguement Count, Arguements... Ex: IL0373Commands.PANEL_SETTING, 1, 0xCF, IL0373Commands.CDI, 1, 0x37
        /// 0xFF is "WaitForCompletion" Command with the next byte being a delay, for if the busy pin is not set
        /// 0xFE Optionally signals the end of list, for Arduino_EPD library compatibility
        /// </param>
        private void commandList(byte[] init_code)
        {
            for (int index = 0;index < init_code.Length;index++)
            {
                byte cmd = init_code[index];
                if (cmd == 0xFE) 
                    break;
                byte num_args = init_code[++index];
                if (cmd == 0xFF)
                {
                    WaitForCompletion(num_args);
                    continue;
                }

                var buf = new byte[num_args];
                for (int i = 0; i < num_args; i++)
                    buf[i] = init_code[++index];

                SendCommand(cmd, buf);
            }
        }
        /// <summary>
        /// Sends a command to the display
        /// </summary>
        /// <param name="command">The byte command to send. IL0373Commands has presets</param>
        /// <param name="data">The command arguements to send</param>
        private void SendCommand(byte command, byte[] data)
        {
            Debug.WriteLine("Sending Command: 0x" + command.ToString("X"));
            _ecs.Write(PinValue.High);
            _dc.Write(PinValue.Low);
            _ecs.Write(PinValue.Low);
            _spiDevice.WriteByte(command);
            _dc.Write(PinValue.High);
            _spiDevice.Write(data);
            _ecs.Write(PinValue.High);
        }

        /// <summary>
        /// Oh wait, this one makes pancakes~
        /// </summary>
        public void DrawAPancake()
        {
            Debug.WriteLine("(((((~) < Pancakes with Syrup");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write data do the spi device.
        /// </summary>
        /// <remarks>
        /// The size of the data should be 2 * cascaded devices.
        /// </remarks>
        private void Write(SpanByte data) => _spiDevice.Write(data);



        /// <summary>
        /// Gets the total number of digits (cascaded devices * num digits)
        /// </summary>
        public int Length => mainBuffer.Length;

        /// <summary>
        /// External SRAM buffer is enabled
        /// </summary>
        public bool UseSram { get => _sramSpiDevice != null; }
        /// <summary>
        /// Internal RAM buffer
        /// </summary>
        public byte[] mainBuffer { get; private set; }
        /// <summary>
        /// External SRAM buffer
        /// </summary>
        public byte[] ecBuffer { get; private set; }


        /// <summary>
        /// Writes all the Values to the devices.
        /// </summary>
        public void Flush() => WriteBuffer(new byte[0][]);

        /// <summary>
        /// Writes a two dimensional buffer containing all the values to the devices.
        /// </summary>
        public void WriteBuffer(byte[][] buffer)
        {
            switch (Rotation)
            {
                case RotationType.None:
                    WriteBufferWithoutRotation(buffer);
                    break;
                case RotationType.Half:
                    WriteBufferRotateHalf(buffer);
                    break;
                case RotationType.Right:
                    WriteBufferRotateRight(buffer);
                    break;
                case RotationType.Left:
                    WriteBufferRotateLeft(buffer);
                    break;
            }
        }

        /// <summary>
        /// Writes a two dimensional buffer containing all the values to the devices without roation
        /// </summary>
        private void WriteBufferWithoutRotation(byte[][] buffer)
        {
            
        }

        /// <summary>
        /// Writes a two dimensional buffer containing all the values to the devices
        /// rotating values by 180 degree.
        /// </summary>
        private void WriteBufferRotateHalf(byte[][] buffer)
        {
            
        }

        /// <summary>
        /// Writes a two dimensional buffer containing all the values to the devices
        /// rotating values to the right.
        /// </summary>
        private void WriteBufferRotateRight(byte[][] buffer)
        {
            
        }

        /// <summary>
        /// Writes a two dimensional buffer containing all the values to the devices
        /// rotating values to the left.
        /// </summary>
        private void WriteBufferRotateLeft(byte[][] buffer)
        {
            
        }

        /// <summary>
        /// Clears the buffer from the given start to end (exclusive) and flushes
        /// </summary>
        public void Clear(int start, int end, bool flush = true)
        {
           
            if (flush)
            {
                Flush();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _spiDevice?.Dispose();
            _spiDevice = null!;
            _sramSpiDevice?.Dispose();
            _sramSpiDevice = null!;
        }
    }
}
