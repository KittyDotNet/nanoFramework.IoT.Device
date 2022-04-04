// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Spi;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.IL0373;
using nanoFramework.Hardware.Esp32;

string message = "Hello World from IL0373!";

Debug.WriteLine(message);
var gpioController = new GpioController();
//////////////////////////////////////////////////////////////////////
// when connecting to an ESP32 device, need to configure the SPI GPIOs
// used for the bus
// This is for Adafruit Huzzah32
Configuration.SetPinFunction(23, DeviceFunction.I2C1_DATA);
Configuration.SetPinFunction(22, DeviceFunction.I2C2_CLOCK);
Configuration.SetPinFunction(18, DeviceFunction.SPI1_MOSI);
Configuration.SetPinFunction(19, DeviceFunction.SPI1_MISO);
Configuration.SetPinFunction(5, DeviceFunction.SPI1_CLOCK);
// Make sure as well you are using the right chip select
SpiConnectionSettings connectionSettings = new(1,27)
{
    ClockFrequency =  IL0373.SpiClockFrequency,
    Mode = IL0373.SpiMode
};
using SpiDevice spi = SpiDevice.Create(connectionSettings);
using GpioPin DataCommandPin = gpioController.OpenPin(33);
using GpioPin DisplaySelectPin = gpioController.OpenPin(15);
using IL0373 devices = new(spi,DataCommandPin, DisplaySelectPin, 296, 128);
// initialize the devices
devices.Init();

// reinitialize the devices
Debug.WriteLine("Init");

// write a smiley to devices buffer
var smiley = new byte[]
{
    0b00111100,
    0b01000010,
    0b10100101,
    0b10000001,
    0b10100101,
    0b10011001,
    0b01000010,
    0b00111100
};


// flush the smiley to the devices using a different rotation each iteration.
//foreach (RotationType rotation in Enum.GetValues(typeof(RotationType)))

Debug.WriteLine($"Send Smiley using rotation {devices.Rotation}.");
devices.Rotation = RotationType.None;
devices.Flush();
Thread.Sleep(1000);
Debug.WriteLine($"Send Smiley using rotation {devices.Rotation}.");
devices.Rotation = RotationType.Right;
devices.Flush();
Thread.Sleep(1000);
Debug.WriteLine($"Send Smiley using rotation {devices.Rotation}.");
devices.Rotation = RotationType.Half;
devices.Flush();
Thread.Sleep(1000);
Debug.WriteLine($"Send Smiley using rotation {devices.Rotation}.");
devices.Rotation = RotationType.Left;
devices.Flush();
Thread.Sleep(1000);

// reinitialize device and show message using the matrix graphics
devices.Init();
devices.Rotation = RotationType.Right;


RotationType ReadRotation(char c) => c switch
{
    'l' => RotationType.Left,
    'r' => RotationType.Right,
    'n' => RotationType.None,
    'h' => RotationType.Half,
    _ => RotationType.None,
};
