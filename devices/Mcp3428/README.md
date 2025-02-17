﻿# Mcp3428 - Analog to Digital Converter (I2C)

The library implements the Microchip MCP3428 16 bit A/D converter with I2C interface. It has hardware configurable I2C address and software configurable resolution and gain. Can work in continuous and on-demand conversion modes.

## Documentation

"The   MCP3426,   MCP3427   and   MCP3428   devices (MCP3426/7/8)  are  the  low  noise  and  high  accuracy 16 Bit  Delta-Sigma  Analog-to-Digital  (ΔΣ  A/D)  Converter  family  members  of  the  MCP342X  series  from Microchip Technology Inc. These devices can convert analog inputs to digital codes with up to 16 bits of resolution." - Datasheet

The  3 devices differ only in addressing capability and channel number. The library implements all of them.

* MCP3428 [datasheet](http://ww1.microchip.com/downloads/en/DeviceDoc/22226a.pdf)

## Board

![MCU Breadboard diagram](rpi_led_adc_i2c.png)

## Usage

Simple example for measuring LED forward voltage using the MCP3428 ADC and a MCU.

```csharp
// I2C addres based on pin configuration
var addr = Mcp3428.AddressFromPins(PinState.Low, PinState.Low);
var options = new I2cConnectionSettings(1, addr);

using (var dev = new UnixI2cDevice(options))
using (var adc = new Mcp3428(dev, ModeEnum.OneShot, ResolutionEnum.Bit16, GainEnum.X1))
{
    var ch1 = adc.ReadChannel(0);

    Debug.WriteLine($"LED forward voltage value: {ch1} V");
}
```

On the MCP3428 you can select 8 different I2C addresses that the device answers on. It's done by connecting two pins, Adr0 and Adr1 to supply voltage or ground or leaving then floating. The library has a helper method to choose the address based on pin states.

With this instantiating the device and reading the first channel is done like this:

```csharp
var options = new I2cConnectionSettings(1,
    Mcp3428.AddressFromPins(PinState.Low, PinState.Floating));
using (var dev = new UnixI2cDevice(options))
using (var adc = new Mcp3428(dev)) // Default settings
{
    var ch1 = adc.ReadChannel(0);

    Debug.WriteLine($"ADC Channel value: {ch1} V");
}
```

The library provides an async API as reading with 16 bit resolution can take up to 60-80ms. It's in a separate class called `Mcp3428Async`.
