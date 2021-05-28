// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Iot.Device.Graphics
{
    /// <summary>
    /// The specific PCD8544 font for Nokia 5110
    /// </summary>
    public class Font5x8 : BdfFont
    {
        /// <summary>
        /// ASCII Font specific to the PCD8544 Nokia 5110 screen but can be used as a generic 5x8 font.
        /// Font characters are column bit mask.
        /// Font size is 5 pixels width and 8 pixels height. Each byte represent a vertical column for the character.
        /// </summary>
        private static readonly byte[][] Ascii = new byte[][]
        {
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, // 20
            new byte[] { 0x00, 0x00, 0x5f, 0x00, 0x00 }, // 21 !
            new byte[] { 0x00, 0x07, 0x00, 0x07, 0x00 }, // 22 "
            new byte[] { 0x14, 0x7f, 0x14, 0x7f, 0x14 }, // 23 #
            new byte[] { 0x24, 0x2a, 0x7f, 0x2a, 0x12 }, // 24 $
            new byte[] { 0x23, 0x13, 0x08, 0x64, 0x62 }, // 25 %
            new byte[] { 0x36, 0x49, 0x55, 0x22, 0x50 }, // 26 &
            new byte[] { 0x00, 0x05, 0x03, 0x00, 0x00 }, // 27 '
            new byte[] { 0x00, 0x1c, 0x22, 0x41, 0x00 }, // 28 (
            new byte[] { 0x00, 0x41, 0x22, 0x1c, 0x00 }, // 29 )
            new byte[] { 0x14, 0x08, 0x3e, 0x08, 0x14 }, // 2a *
            new byte[] { 0x08, 0x08, 0x3e, 0x08, 0x08 }, // 2b +
            new byte[] { 0x00, 0x50, 0x30, 0x00, 0x00 }, // 2c ,
            new byte[] { 0x08, 0x08, 0x08, 0x08, 0x08 }, // 2d -
            new byte[] { 0x00, 0x60, 0x60, 0x00, 0x00 }, // 2e .
            new byte[] { 0x20, 0x10, 0x08, 0x04, 0x02 }, // 2f /
            new byte[] { 0x3e, 0x51, 0x49, 0x45, 0x3e }, // 30 0
            new byte[] { 0x00, 0x42, 0x7f, 0x40, 0x00 }, // 31 1
            new byte[] { 0x42, 0x61, 0x51, 0x49, 0x46 }, // 32 2
            new byte[] { 0x21, 0x41, 0x45, 0x4b, 0x31 }, // 33 3
            new byte[] { 0x18, 0x14, 0x12, 0x7f, 0x10 }, // 34 4
            new byte[] { 0x27, 0x45, 0x45, 0x45, 0x39 }, // 35 5
            new byte[] { 0x3c, 0x4a, 0x49, 0x49, 0x30 }, // 36 6
            new byte[] { 0x01, 0x71, 0x09, 0x05, 0x03 }, // 37 7
            new byte[] { 0x36, 0x49, 0x49, 0x49, 0x36 }, // 38 8
            new byte[] { 0x06, 0x49, 0x49, 0x29, 0x1e }, // 39 9
            new byte[] { 0x00, 0x36, 0x36, 0x00, 0x00 }, // 3a :
            new byte[] { 0x00, 0x56, 0x36, 0x00, 0x00 }, // 3b ;
            new byte[] { 0x08, 0x14, 0x22, 0x41, 0x00 }, // 3c <
            new byte[] { 0x14, 0x14, 0x14, 0x14, 0x14 }, // 3d =
            new byte[] { 0x00, 0x41, 0x22, 0x14, 0x08 }, // 3e >
            new byte[] { 0x02, 0x01, 0x51, 0x09, 0x06 }, // 3f ?
            new byte[] { 0x32, 0x49, 0x79, 0x41, 0x3e }, // 40 @
            new byte[] { 0x7e, 0x11, 0x11, 0x11, 0x7e }, // 41 A
            new byte[] { 0x7f, 0x49, 0x49, 0x49, 0x36 }, // 42 B
            new byte[] { 0x3e, 0x41, 0x41, 0x41, 0x22 }, // 43 C
            new byte[] { 0x7f, 0x41, 0x41, 0x22, 0x1c }, // 44 D
            new byte[] { 0x7f, 0x49, 0x49, 0x49, 0x41 }, // 45 E
            new byte[] { 0x7f, 0x09, 0x09, 0x09, 0x01 }, // 46 F
            new byte[] { 0x3e, 0x41, 0x49, 0x49, 0x7a }, // 47 G
            new byte[] { 0x7f, 0x08, 0x08, 0x08, 0x7f }, // 48 H
            new byte[] { 0x00, 0x41, 0x7f, 0x41, 0x00 }, // 49 I
            new byte[] { 0x20, 0x40, 0x41, 0x3f, 0x01 }, // 4a J
            new byte[] { 0x7f, 0x08, 0x14, 0x22, 0x41 }, // 4b K
            new byte[] { 0x7f, 0x40, 0x40, 0x40, 0x40 }, // 4c L
            new byte[] { 0x7f, 0x02, 0x0c, 0x02, 0x7f }, // 4d M
            new byte[] { 0x7f, 0x04, 0x08, 0x10, 0x7f }, // 4e N
            new byte[] { 0x3e, 0x41, 0x41, 0x41, 0x3e }, // 4f O
            new byte[] { 0x7f, 0x09, 0x09, 0x09, 0x06 }, // 50 P
            new byte[] { 0x3e, 0x41, 0x51, 0x21, 0x5e }, // 51 Q
            new byte[] { 0x7f, 0x09, 0x19, 0x29, 0x46 }, // 52 R
            new byte[] { 0x46, 0x49, 0x49, 0x49, 0x31 }, // 53 S
            new byte[] { 0x01, 0x01, 0x7f, 0x01, 0x01 }, // 54 T
            new byte[] { 0x3f, 0x40, 0x40, 0x40, 0x3f }, // 55 U
            new byte[] { 0x1f, 0x20, 0x40, 0x20, 0x1f }, // 56 V
            new byte[] { 0x3f, 0x40, 0x38, 0x40, 0x3f }, // 57 W
            new byte[] { 0x63, 0x14, 0x08, 0x14, 0x63 }, // 58 X
            new byte[] { 0x07, 0x08, 0x70, 0x08, 0x07 }, // 59 Y
            new byte[] { 0x61, 0x51, 0x49, 0x45, 0x43 }, // 5a Z
            new byte[] { 0x00, 0x7f, 0x41, 0x41, 0x00 }, // 5b [
            new byte[] { 0x02, 0x04, 0x08, 0x10, 0x20 }, // 5c ¥
            new byte[] { 0x00, 0x41, 0x41, 0x7f, 0x00 }, // 5d ]
            new byte[] { 0x04, 0x02, 0x01, 0x02, 0x04 }, // 5e ^
            new byte[] { 0x40, 0x40, 0x40, 0x40, 0x40 }, // 5f _
            new byte[] { 0x00, 0x01, 0x02, 0x04, 0x00 }, // 60 `
            new byte[] { 0x20, 0x54, 0x54, 0x54, 0x78 }, // 61 a
            new byte[] { 0x7f, 0x48, 0x44, 0x44, 0x38 }, // 62 b
            new byte[] { 0x38, 0x44, 0x44, 0x44, 0x20 }, // 63 c
            new byte[] { 0x38, 0x44, 0x44, 0x48, 0x7f }, // 64 d
            new byte[] { 0x38, 0x54, 0x54, 0x54, 0x18 }, // 65 e
            new byte[] { 0x08, 0x7e, 0x09, 0x01, 0x02 }, // 66 f
            new byte[] { 0x0c, 0x52, 0x52, 0x52, 0x3e }, // 67 g
            new byte[] { 0x7f, 0x08, 0x04, 0x04, 0x78 }, // 68 h
            new byte[] { 0x00, 0x44, 0x7d, 0x40, 0x00 }, // 69 i
            new byte[] { 0x20, 0x40, 0x44, 0x3d, 0x00 }, // 6a j
            new byte[] { 0x7f, 0x10, 0x28, 0x44, 0x00 }, // 6b k
            new byte[] { 0x00, 0x41, 0x7f, 0x40, 0x00 }, // 6c l
            new byte[] { 0x7c, 0x04, 0x18, 0x04, 0x78 }, // 6d m
            new byte[] { 0x7c, 0x08, 0x04, 0x04, 0x78 }, // 6e n
            new byte[] { 0x38, 0x44, 0x44, 0x44, 0x38 }, // 6f o
            new byte[] { 0x7c, 0x14, 0x14, 0x14, 0x08 }, // 70 p
            new byte[] { 0x08, 0x14, 0x14, 0x18, 0x7c }, // 71 q
            new byte[] { 0x7c, 0x08, 0x04, 0x04, 0x08 }, // 72 r
            new byte[] { 0x48, 0x54, 0x54, 0x54, 0x20 }, // 73 s
            new byte[] { 0x04, 0x3f, 0x44, 0x40, 0x20 }, // 74 t
            new byte[] { 0x3c, 0x40, 0x40, 0x20, 0x7c }, // 75 u
            new byte[] { 0x1c, 0x20, 0x40, 0x20, 0x1c }, // 76 v
            new byte[] { 0x3c, 0x40, 0x30, 0x40, 0x3c }, // 77 w
            new byte[] { 0x44, 0x28, 0x10, 0x28, 0x44 }, // 78 x
            new byte[] { 0x0c, 0x50, 0x50, 0x50, 0x3c }, // 79 y
            new byte[] { 0x44, 0x64, 0x54, 0x4c, 0x44 }, // 7a z
            new byte[] { 0x00, 0x08, 0x36, 0x41, 0x00 }, // 7b
            new byte[] { 0x00, 0x00, 0x7f, 0x00, 0x00 }, // 7c |
            new byte[] { 0x00, 0x41, 0x36, 0x08, 0x00 }, // 7d
            new byte[] { 0x10, 0x08, 0x08, 0x10, 0x08 }, // 7e ←
            new byte[] { 0x78, 0x46, 0x41, 0x46, 0x78 }, // 7f →
        };

        /// <summary>
        /// Constructor for Font 5x8
        /// </summary>
        public Font5x8()
        {
            Width = 5;
            Height = 8;
            XDisplacement = 0;
            YDisplacement = 0;
            DefaultChar = 0x20;
            CharsCount = Ascii.Length;
            GlyphMapper = new Dictionary<int, int>();
            GlyphUshortData = new ushort[CharsCount * Height];
            for (int i = 0; i < CharsCount; i++)
            {
                var font8 = LcdCharacterEncodingFactory.ConvertFont5to8bytes(Ascii[i]);
                for (int j = 0; j < 8; j++)
                {
                    GlyphUshortData[i * 8 + j] = font8[j];
                }

                GlyphMapper.Add(i + DefaultChar, i * 8);
            }
        }
    }
}
