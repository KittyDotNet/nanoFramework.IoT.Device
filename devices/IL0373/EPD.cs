using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;

namespace Iot.Device.IL0373
{
    public abstract class EPD
    {
        public abstract void PowerUp();
        public abstract void PowerDown();
        public abstract void Update();
        public abstract void SetRAMAddress(int x, int y);
        public abstract void WriteRAMCommand(byte index);

        protected SpiDevice _spiDevice;
        protected GpioPin _ecs;
        protected GpioPin _dc;
        protected int _width;
        protected int _height;
        protected GpioPin _srcs;
        /// <summary>
        /// External SRAM buffer is enabled
        /// </summary>
        public bool UseSRAM { get => RamBuffer != null; }
        /// <summary>
        /// Wait timer for when busy pin not used
        /// </summary>
        protected const int BusyWaitTime = 500;
        /// <summary>
        /// The GpioPin to reset the display
        /// </summary>
        public GpioPin ResetPin { get; set; }
        /// <summary>
        /// GpioPin for an optional ram buffer CS pin, using main SPI device
        /// </summary>
        public GpioPin RamBuffer { get; set; }
        /// <summary>
        /// The GpioPin to receive the busy signal
        /// </summary>
        public GpioPin BusyPin { get; set; }
        /// <summary>
        /// Sets whether the Black portion of the displayise inverted
        /// </summary>
        public bool InvertBlack { get; set; } = true;

        /// <summary>
        /// Sets whether the Red portion of the displayise inverted
        /// </summary>
        public bool InvertRed { get; set; } = true;
        /// <summary>
        /// The Rotation to be applied (when modules are assembled rotated way)
        /// </summary>
        public RotationType Rotation { get; set; } = RotationType.None;
        public bool BlackInverted { get; private set; }
        public bool ColorInverted { get; private set; }
        public byte[] BlackBuffer { get; private set; }
        public byte[] ColorBuffer { get; private set; }

        public void ClearDisplay()
        {
            ClearBuffer();
            Display();
            Thread.Sleep(100);
            Display();
        }
        protected void HardwareReset()
        {
            if (ResetPin != null)
            {
                // Setup reset pin direction
                ResetPin.SetPinMode(PinMode.Output);
                // VDD (3.3V) goes high at start, lets just chill for a ms
                ResetPin.Write(PinValue.High);
                Thread.Sleep(10);
                // bring reset low
                ResetPin.Write(PinValue.Low);
                // wait 10ms
                Thread.Sleep(10);
                // bring out of reset
                ResetPin.Write(PinValue.High);
                Thread.Sleep(10);
            }
        }
        /// <summary>
        /// Send a list of commands to the Controller.
        /// </summary>
        /// <param name="init_code">Byte array containing command list and parameters.
        /// Format: Command, Arguement Count, Arguements... Ex: IL0373Commands.PANEL_SETTING, 1, 0xCF, IL0373Commands.CDI, 1, 0x37
        /// 0xFF is "WaitForCompletion" Command with the next byte being a delay, for if the busy pin is not set
        /// 0xFE Optionally signals the end of list, for Arduino_EPD library compatibility
        /// </param>
        internal void commandList(byte[] init_code)
        {
            for (int index = 0; index < init_code.Length; index++)
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
        /// Wait for busy signal to end.
        /// </summary>
        /// <param name="WaitTime">Wait for busy pin, or timeout. 0 = use default</param>
        void WaitForCompletion(int WaitTime = 0)
        {
            Debug.WriteLine("Waiting...");
            if (BusyPin != null)
            {
                while (BusyPin.Read() == PinValue.High)
                {
                    System.Threading.Thread.Sleep(10); // wait for busy high
                }
            }
            else if (WaitTime > 0)
                Thread.Sleep(WaitTime);
            else
                System.Threading.Thread.Sleep(BusyWaitTime);
            Debug.WriteLine("OK!");
        }
        /// <summary>
        /// Sends a command to the display
        /// </summary>
        /// <param name="command">The byte command to send. IL0373Commands has presets</param>
        /// <param name="data">The command arguements to send</param>
        internal void SendCommand(IL0373Commands command, byte[] data)
        => SendCommand(command, data);
        internal void SendCommand(byte command, byte[] data)
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

        void writeSRAMFramebufferToEPD(byte SRAM_buffer_addr,
                                             int buffer_size,
                                             byte EPDlocation,
                                             bool invertdata)
        {
            throw new NotImplementedException();

            //(void)invertdata;
            //byte c;
            //// use SRAM
            //sram.csLow();
            //_isInTransaction = true;
            //// send read command
            //SPItransfer(MCPSRAM_READ);
            //// send address
            //SPItransfer(SRAM_buffer_addr >> 8);
            //SPItransfer(SRAM_buffer_addr & 0xFF);

            //// first data byte from SRAM will be transfered in at the same time
            //// as the EPD command is transferred out
            //c = writeRAMCommand(EPDlocation);

            //dcHigh();
            //for (uint32_t i = 0; i < buffer_size; i++)
            //{
            //    c = SPItransfer(c);
            //    /*
            //    Serial.print("0x"); Serial.print((byte)c, HEX); Serial.print(", ");
            //    if (i % 32 == 31) {
            //      Serial.println();
            //      Serial.print("$");
            //      Serial.print(i, HEX);
            //      Serial.print(": ");
            //    }
            //    */
            //}
            //csHigh();
            //sram.csHigh();
            //_isInTransaction = false;
        }
        
        void Display(bool sleep = false)
        {

            Debug.WriteLine("  Powering Up");
            PowerUp();

            Debug.WriteLine("  Set RAM address");

            // Set X & Y ram counters
            SetRAMAddress(0, 0);

            if (UseSRAM)
            {
                Debug.WriteLine("  Write SRAM buff to EPD");
                writeSRAMFramebufferToEPD(buffer1_addr, buffer1_size, 0);
            }
            else
            {
                Debug.WriteLine("  Write RAM buff to EPD");
                writeRAMFramebufferToEPD(BlackBuffer, 0);
            }

            if ((ColorBuffer != null && ColorBuffer.Length != 0) || buffer2_size > 0)
            {
                // oh there's another buffer eh?
                Thread.Sleep(2);

                // Set X & Y ram counters
                SetRAMAddress(0, 0);

                if (UseSRAM)
                {
                    writeSRAMFramebufferToEPD(buffer2_addr, buffer2_size, 1);
                }
                else
                {
                    writeRAMFramebufferToEPD(ColorBuffer, 1);
                }
            }

            Debug.WriteLine("  Update");
            Update();
            partialsSinceLastFullUpdate = 0;

            if (sleep)
            {
                Debug.WriteLine("  Powering Down");
                PowerDown();
            }
        }
        void writeRAMFramebufferToEPD(byte[] framebuffer,
                                            byte EPDlocation,
                                            bool invertdata = false)
        {
            // write image
            WriteRAMCommand(EPDlocation);
            _dc.Write(PinValue.High);
            // Serial.printf("Writing from RAM location %04x: \n", &framebuffer);

            for (int i = 0; i < framebuffer.Length; i++)
            {
                byte d = framebuffer[i];
                if (invertdata)
                    d = (byte)~d;

                /*
                Serial.printf("%02x", d);
                if ((i+1) % (WIDTH/8) == 0)
                  Serial.println();
                */

                _spiDevice.WriteByte(d);
            }
            //  Serial.println();
            _ecs.Write(PinValue.High);
            return;
        }
        void ClearBuffer()
        {
            if (RamBuffer != null)
            {
                throw new NotImplementedException();
                //if (BlackInverted)
                //{
                //    sram.erase(blackbuffer_addr, buffer1_size, 0xFF);
                //}
                //else
                //{
                //    sram.erase(blackbuffer_addr, buffer1_size, 0x00);
                //}
                //if (ColorInverted)
                //{
                //    sram.erase(colorbuffer_addr, buffer2_size, 0xFF);
                //}
                //else
                //{
                //    sram.erase(colorbuffer_addr, buffer2_size, 0x00);
                //}
            }
            else
            {
                if (BlackBuffer != null)
                {
                    if (BlackInverted)
                    {
                        for (int i = 0; i < BlackBuffer.Length; i++)
                            BlackBuffer[i] = 0xFF;
                    }
                    else
                    {
                        for (int i = 0; i < BlackBuffer.Length; i++)
                            BlackBuffer[i] = 0x00;
                    }
                }
                if (ColorBuffer != null)
                {
                    if (ColorInverted)
                    {
                        for (int i = 0; i < ColorBuffer.Length; i++)
                            ColorBuffer[i] = 0xFF;
                    }
                    else
                    {
                        for (int i = 0; i < ColorBuffer.Length; i++)
                            ColorBuffer[i] = 0x00;

                    }
                }
            }
        }
    }
}
