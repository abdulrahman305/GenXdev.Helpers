// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : ConsoleReader.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.302.2025
// ################################################################################
// Copyright (c)  René Vaessen / GenXdev
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ################################################################################



using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace GenXdev.Helpers
{
    public static class ConsoleReader
    {
        public static IEnumerable<string> ReadFromBuffer(short x, short y, short width, short height)
        {
            // Validate parameters
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive values");

            if (x < 0 || y < 0)
                throw new ArgumentException("X and Y coordinates must be non-negative");

            // Get console screen buffer info to validate coordinates
            var consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero || consoleHandle == new IntPtr(-1))
                throw new InvalidOperationException("Cannot get console handle");

            CONSOLE_SCREEN_BUFFER_INFO csbi;
            if (!GetConsoleScreenBufferInfo(consoleHandle, out csbi))
            {
                int code = Marshal.GetLastWin32Error();
                throw new Win32Exception(code, "Cannot get console screen buffer info");
            }

            // Validate that the requested region is within buffer bounds
            if (x >= csbi.dwSize.X || y >= csbi.dwSize.Y)
                throw new ArgumentOutOfRangeException($"Coordinates ({x},{y}) are outside console buffer. Buffer size: {csbi.dwSize.X}x{csbi.dwSize.Y}");

            // Ensure we don't read beyond buffer bounds
            if (x + width > csbi.dwSize.X)
                width = (short)(csbi.dwSize.X - x);
            if (y + height > csbi.dwSize.Y)
                height = (short)(csbi.dwSize.Y - y);

            // Final validation that we have a valid region to read
            if (width <= 0 || height <= 0)
                throw new ArgumentException($"Invalid read region after bounds checking. Adjusted size: {width}x{height}");

            IntPtr buffer = Marshal.AllocHGlobal(width * height * Marshal.SizeOf(typeof(CHAR_INFO)));
            if (buffer == IntPtr.Zero)
                throw new OutOfMemoryException();

            try
            {
                COORD coord = new COORD { X = 0, Y = 0 };
                SMALL_RECT rc = new SMALL_RECT
                {
                    Left = x,
                    Top = y,
                    Right = (short)(x + width - 1),
                    Bottom = (short)(y + height - 1)
                };

                COORD size = new COORD
                {
                    X = width,
                    Y = height
                };

                if (!ReadConsoleOutput(consoleHandle, buffer, size, coord, ref rc))
                {
                    int code = Marshal.GetLastWin32Error();
                    throw new Win32Exception(code, $"ReadConsoleOutput failed. Error code: {code}");
                }

                IntPtr ptr = buffer;
                for (int h = 0; h < height; h++)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(width);
                    for (short w = 0; w < width; w++)
                    {
                        CHAR_INFO ci = (CHAR_INFO)Marshal.PtrToStructure(ptr, typeof(CHAR_INFO));
                        // Use the Unicode character directly
                        sb.Append(ci.UnicodeChar);
                        ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(CHAR_INFO)));
                    }
                    yield return sb.ToString();
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static string GetConsoleInfo()
        {
            var consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero || consoleHandle == new IntPtr(-1))
                return "Cannot get console handle";

            CONSOLE_SCREEN_BUFFER_INFO csbi;
            if (!GetConsoleScreenBufferInfo(consoleHandle, out csbi))
            {
                int code = Marshal.GetLastWin32Error();
                return $"Cannot get console screen buffer info. Error: {code}";
            }

            return $"Buffer Size: {csbi.dwSize.X}x{csbi.dwSize.Y}, " +
                   $"Window: ({csbi.srWindow.Left},{csbi.srWindow.Top})-({csbi.srWindow.Right},{csbi.srWindow.Bottom}), " +
                   $"Cursor: ({csbi.dwCursorPosition.X},{csbi.dwCursorPosition.Y}), " +
                   $"Max Window: {csbi.dwMaximumWindowSize.X}x{csbi.dwMaximumWindowSize.Y}";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHAR_INFO
        {
            [MarshalAs(UnmanagedType.U2)]
            public char UnicodeChar;
            [MarshalAs(UnmanagedType.U2)]
            public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleOutput(IntPtr hConsoleOutput, IntPtr lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpReadRegion);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        private const int STD_OUTPUT_HANDLE = -11;
    }
}