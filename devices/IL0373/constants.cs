using System.Device.Spi;

namespace Iot.Device.IL0373
{
    ///<summary>Byte commands for IL0370</summary>
    public static class IL0373Commands
    {
        public static byte PANEL_SETTING = 0x00;
        public static byte POWER_SETTING = 0x01;
        /// <summary>Turns the controller off</summary>
        public static byte POWER_OFF = 0x02;
        public static byte POWER_OFF_SEQUENCE = 0x03;
        public static byte POWER_ON = 0x04;
        public static byte POWER_ON_MEASURE = 0x05;
        public static byte BOOSTER_SOFT_START = 0x06;
        public static byte DEEP_SLEEP = 0x07;
        public static byte DTM1 = 0x10;
        public static byte DATA_STOP = 0x11;
        public static byte DISPLAY_REFRESH = 0x12;
        public static byte DTM2 = 0x13;
        public static byte PDTM1 = 0x14;
        public static byte PDTM2 = 0x15;
        public static byte PDRF = 0x16;
        public static byte LUT1 = 0x20;
        public static byte LUTWW = 0x21;
        public static byte LUTBW = 0x22;
        public static byte LUTWB = 0x23;
        public static byte LUTBB = 0x24;
        public static byte PLL = 0x30;
        public static byte CDI = 0x50;
        public static byte RESOLUTION = 0x61;
        public static byte VCM_DC_SETTING = 0x82;
        public static byte PARTIAL_WINDOW = 0x90;
        public static byte PARTIAL_ENTER = 0x91;
        public static byte PARTIAL_EXIT = 0x92;
    }
    public partial class IL0373
    {
        /// <summary>
        /// IL0373 Spi Clock Frequency
        /// </summary>
        public const int SpiClockFrequency = 4000000;

        /// <summary>
        /// Wait timer for when busy pin not used
        /// </summary>
        private const int BusyWaitTime = 500;

        /// <summary>
        /// IL0373 SPI Mode
        /// </summary>
        public const SpiMode SpiMode = System.Device.Spi.SpiMode.Mode0;

        /// <summary>
        /// The Default startup code from Adafruit's EPD library
        /// </summary>
        public static byte[] IL0398DefaultInitCode =
        {
            IL0373Commands.POWER_SETTING, 5, 0x03, 0x00, 0x2b, 0x2b, 0x09,
            IL0373Commands.BOOSTER_SOFT_START, 3, 0x17, 0x17, 0x17,
            IL0373Commands.POWER_ON, 0, 0xFF, 200,
            IL0373Commands.PANEL_SETTING, 1, 0xCF,
            IL0373Commands.CDI, 1, 0x37,
            IL0373Commands.PLL, 1, 0x29,
            IL0373Commands.VCM_DC_SETTING, 1, 0x0A, 0xFF, 20,
            0xFE
        };
    }
}