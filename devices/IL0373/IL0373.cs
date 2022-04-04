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
    public partial class IL0373 : EPD, IDisposable
    {
        public override void WriteRAMCommand(byte index)
        {
            if (index == 0)
            {
                SendCommand(IL0373Commands.DTM1, new byte[1]{ 0});
            }
            if (index == 1)
            {
                SendCommand(IL0373Commands.DTM2, new byte[1] { 0 });
            }
        }
        public override void SetRAMAddress(int x,int y)
        {
            //do nothing on this
        }
        public override void Update()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Creates a IL0373 Device on <see paramref="spiDevice" /> to communicate over.
        /// </summary>
        /// <param name="spiDevice">The main IL0373 spi device, do not include CS pin</param>
        /// <param name="DisplaySelectPin">The Display Cable Select pin Gpio device</param>
        /// <param name="DataCommandPin">The Command/Data transfer select pin Gpio device</param>
        /// <param name="BusyPin">The Busy/Wait pin Gpio device</param>
        public IL0373(SpiDevice spiDevice,GpioPin DisplaySelectPin, GpioPin DataCommandPin, int width, int height, RotationType rotation = RotationType.None)
        {
            base._spiDevice = spiDevice ?? throw new ArgumentNullException(nameof(spiDevice));
            _ecs = DisplaySelectPin ?? throw new ArgumentNullException(nameof(DisplaySelectPin)); ;
            _dc = DataCommandPin ?? throw new ArgumentNullException(nameof(DataCommandPin));
            _width = width;
            _height = height;
            Rotation = rotation;
            uint buffer_size = ((uint)width * (uint)height) / 8;

            mainBuffer = ecBuffer = new byte[buffer_size];
        }
        public override void PowerDown()
        {
            // power off
            SendCommand(IL0373Commands.CDI, new byte[1] { 0x17 });

            SendCommand(IL0373Commands.VCM_DC_SETTING, new byte[1] { 0x00 });

            SendCommand(IL0373Commands.POWER_OFF, new byte[0]);
        }

/// <summary>
/// Initializes the device
/// </summary>
        public void Init()
        {
            _spiDevice.ConnectionSettings.ClockFrequency = IL0373.SpiClockFrequency;
            _spiDevice.ConnectionSettings.Mode = IL0373.SpiMode;
            _ecs.SetPinMode(PinMode.Output);
            _dc.SetPinMode(PinMode.Output);
            commandList(IL0398DefaultInitCode);

            var buf = new byte[3];

            buf[0] = (byte)(129 & 0xFF);
            buf[1] = (byte)((296 >> 8) & 0xFF);
            buf[2] = (byte)(296 & 0xFF);
            
            SendCommand((byte)IL0373Commands.RESOLUTION, buf);

            SendCommand((byte)IL0373Commands.DISPLAY_REFRESH, new byte[0]);
        }
        
   
        public override void PowerUp()
        {
            Debug.WriteLine("Power up");
            HardwareReset();

            if (_epd_init_code.Length > 0)
                commandList(_epd_init_code);
            else
                commandList(IL0398DefaultInitCode);
            Debug.WriteLine("End command list");
            if (_epd_lut_code.Length > 0)
            {
                commandList(_epd_lut_code);
            }
            var buf = new byte[3];

            buf[0] = (byte)(129 & 0xFF);
            buf[1] = (byte)((296 >> 8) & 0xFF);
            buf[2] = (byte)(296 & 0xFF);
            Debug.WriteLine("post buff");
            SendCommand(IL0373Commands.RESOLUTION, buf);

            SendCommand((byte)IL0373Commands.DISPLAY_REFRESH, new byte[0]);
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
        public int Length => BlackBuffer.Length;

 
        /// <summary>
        /// External SRAM buffer
        /// </summary>
        public byte[] ecBuffer { get; private set; }
        /// <summary>
        /// Custom Display initialization Code
        /// </summary>
        public byte[] _epd_init_code { get; private set; } = new byte[0];
        /// <summary>
        /// optional Post-InitCode
        /// </summary>
        public byte[] _epd_lut_code { get; private set; } = new byte[0];
        


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
        }
    }
}
