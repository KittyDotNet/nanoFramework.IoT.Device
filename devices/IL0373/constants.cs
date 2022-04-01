using System.Device.Spi;

namespace Iot.Device.IL0373
{
    ///<summary>Byte commands for IL0370</summary>
    public enum IL0373Commands :byte
    {
        PANEL_SETTING = 0x00,
         POWER_SETTING = 0x01,
        /// <summary>Turns the controller off</summary>
         POWER_OFF = 0x02,
         POWER_OFF_SEQUENCE = 0x03,
         POWER_ON = 0x04,
         POWER_ON_MEASURE = 0x05,
         BOOSTER_SOFT_START = 0x06,
         DEEP_SLEEP = 0x07,
         DTM1 = 0x10,
         DATA_STOP = 0x11,
         DISPLAY_REFRESH = 0x12,
         DTM2 = 0x13,
         PDTM1 = 0x14,
         PDTM2 = 0x15,
         PDRF = 0x16,
         LUT1 = 0x20,
         LUTWW = 0x21,
         LUTBW = 0x22,
         LUTWB = 0x23,
         LUTBB = 0x24,
         PLL = 0x30,
         CDI = 0x50,
         RESOLUTION = 0x61,
         VCM_DC_SETTING = 0x82,
         PARTIAL_WINDOW = 0x90,
         PARTIAL_ENTER = 0x91,
         PARTIAL_EXIT = 0x92,
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
        byte[] IL0398DefaultInitCode =
        {
            (byte)IL0373Commands.POWER_SETTING, 5, 0x03, 0x00, 0x2b, 0x2b, 0x09,
            (byte)IL0373Commands.BOOSTER_SOFT_START, 3, 0x17, 0x17, 0x17,
            (byte)IL0373Commands.POWER_ON, 0,
            0xFF, 200,
            (byte)IL0373Commands.PANEL_SETTING, 1, 0xCF,
            (byte)IL0373Commands.CDI, 1, 0x37,
            (byte)IL0373Commands.PLL, 1, 0x29,
            (byte)IL0373Commands.VCM_DC_SETTING, 1, 0x0A,
            0xFF, 20,
            (byte)IL0373Commands.DISPLAY_REFRESH,0,
            0xFF,100,
            0xFE
        };
    }
}