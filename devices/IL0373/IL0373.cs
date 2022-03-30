// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Device.Spi;

namespace Iot.Device.IL0373
{
    /// <summary>
    /// IL0373 LED matrix driver
    /// </summary>
    public partial class IL0373 : IDisposable
    {
        private SpiDevice _spiDevice;
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
        /// Creates a IL0373 Device given a <see paramref="spiDevice" /> to communicate over and the
        /// number of devices that are cascaded.
        /// </summary>
        public IL0373(SpiDevice spiDevice,int width, int height,GpioPin BusyPin = null, SpiDevice ramBuffer = null, RotationType rotation = RotationType.None)
        {
            _spiDevice = spiDevice ?? throw new ArgumentNullException(nameof(spiDevice));
            _sramSpiDevice = ramBuffer;
            Rotation = rotation;
            uint buffer_size = ((uint)width * (uint)height) / 8;

            mainBuffer = ecBuffer = new byte[buffer_size];
        }

        /// <summary>
        /// Wait for busy signal to end.
        /// </summary>
        void busy_wait()
        {
            // Serial.print("Waiting...");
            if (_busy_pin != null)
            {
                while (_busy_pin.Read() == PinValue.High)
                {
                    System.Threading.Thread.Sleep(10); // wait for busy high
                }
            }
            else
            {
                System.Threading.Thread.Sleep(BusyWaitTime);
            }
            // Serial.println("OK!");
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
            _spiDevice.Write(IL0398DefaultInitCode);

        }

        /// <summary>
        /// Oh wait, this one makes pancakes~
        /// </summary>
        public void DrawAPancake()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  begin communication with and set up the display.
        /// </summary>
        /// <param name="reset">if true the reset pin will be toggled.</param>

        public void begin(bool reset)
        {

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
