// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : ConsoleReader.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 2.1.2025
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
    /// <summary>
    /// <para type="synopsis">
    /// Provides static methods for reading text content from the Windows console screen buffer.
    /// </para>
    ///
    /// <para type="description">
    /// The ConsoleReader class offers functionality to extract text from specific regions of the console
    /// screen buffer using Windows API calls. It includes methods for reading rectangular areas of text
    /// and retrieving console buffer information.
    /// </para>
    /// </summary>
    public static class ConsoleReader
    {
        /// <summary>
        /// <para type="synopsis">
        /// Reads a rectangular region of text from the console screen buffer.
        /// </para>
        ///
        /// <para type="description">
        /// This method extracts text content from a specified rectangular area of the console screen buffer
        /// using the Windows ReadConsoleOutput API. It returns each line of the region as a string.
        /// The method performs validation on input parameters and adjusts the region if it extends beyond
        /// the buffer boundaries.
        /// </para>
        ///
        /// <param name="x">
        /// The left coordinate (column) of the rectangular region to read from the console buffer.
        /// Must be non-negative and within the buffer width.
        /// </param>
        /// <param name="y">
        /// The top coordinate (row) of the rectangular region to read from the console buffer.
        /// Must be non-negative and within the buffer height.
        /// </param>
        /// <param name="width">
        /// The width (number of columns) of the rectangular region to read.
        /// Must be positive and will be adjusted if it exceeds buffer boundaries.
        /// </param>
        /// <param name="height">
        /// The height (number of rows) of the rectangular region to read.
        /// Must be positive and will be adjusted if it exceeds buffer boundaries.
        /// </param>
        ///
        /// <returns>
        /// An enumerable collection of strings, where each string represents one line of text
        /// from the specified rectangular region of the console buffer.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when width or height are not positive, or when coordinates are negative,
        /// or when the adjusted region has invalid dimensions.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the console handle cannot be obtained.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// Thrown when Windows API calls fail, with the corresponding error code.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// Thrown when memory allocation for the buffer fails.
        /// </exception>
        /// </summary>
        public static IEnumerable<string> ReadFromBuffer(short x, short y, short width, short height)
        {

            // Validate input parameters to ensure they are within acceptable ranges
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive values");

            if (x < 0 || y < 0)
                throw new ArgumentException("X and Y coordinates must be non-negative");

            // Obtain the handle to the standard output console
            var consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero || consoleHandle == new IntPtr(-1))
                throw new InvalidOperationException("Cannot get console handle");

            // Retrieve information about the console screen buffer
            CONSOLE_SCREEN_BUFFER_INFO csbi;
            if (!GetConsoleScreenBufferInfo(consoleHandle, out csbi))
            {
                int code = Marshal.GetLastWin32Error();
                throw new Win32Exception(code, "Cannot get console screen buffer info");
            }

            // Check that the starting coordinates are within the buffer boundaries
            if (x >= csbi.dwSize.X || y >= csbi.dwSize.Y)
                throw new ArgumentOutOfRangeException(
                    $"Coordinates ({x},{y}) are outside console buffer. " +
                    $"Buffer size: {csbi.dwSize.X}x{csbi.dwSize.Y}");

            // Adjust the width and height to not exceed buffer boundaries
            if (x + width > csbi.dwSize.X)
                width = (short)(csbi.dwSize.X - x);
            if (y + height > csbi.dwSize.Y)
                height = (short)(csbi.dwSize.Y - y);

            // Verify that after adjustments, we still have a valid region to read
            if (width <= 0 || height <= 0)
                throw new ArgumentException(
                    $"Invalid read region after bounds checking. " +
                    $"Adjusted size: {width}x{height}");

            // Allocate unmanaged memory for the character information buffer
            IntPtr buffer = Marshal.AllocHGlobal(
                width * height * Marshal.SizeOf(typeof(CHAR_INFO)));
            if (buffer == IntPtr.Zero)
                throw new OutOfMemoryException();

            try
            {

                // Set up coordinates for the buffer and screen region
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

                // Read the console output into the allocated buffer
                if (!ReadConsoleOutput(consoleHandle, buffer, size, coord, ref rc))
                {
                    int code = Marshal.GetLastWin32Error();
                    throw new Win32Exception(code,
                        $"ReadConsoleOutput failed. Error code: {code}");
                }

                // Iterate through each row of the buffer
                IntPtr ptr = buffer;
                for (int h = 0; h < height; h++)
                {

                    // Build a string for the current row
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(width);
                    for (short w = 0; w < width; w++)
                    {

                        // Extract the character information from the buffer
                        CHAR_INFO ci = (CHAR_INFO)Marshal.PtrToStructure(
                            ptr, typeof(CHAR_INFO));

                        // Append the Unicode character to the string builder
                        sb.Append(ci.UnicodeChar);

                        // Move to the next character in the buffer
                        ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(CHAR_INFO)));
                    }

                    // Return the completed row string
                    yield return sb.ToString();
                }
            }
            finally
            {

                // Free the allocated unmanaged memory
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// <para type="synopsis">
        /// Retrieves information about the current console screen buffer.
        /// </para>
        ///
        /// <para type="description">
        /// This method obtains detailed information about the console screen buffer using the
        /// Windows GetConsoleScreenBufferInfo API. It returns a formatted string containing
        /// buffer size, window coordinates, cursor position, and maximum window size.
        /// </para>
        ///
        /// <returns>
        /// A string containing formatted console buffer information including size, window
        /// coordinates, cursor position, and maximum window size. Returns an error message
        /// if the console handle cannot be obtained or buffer info retrieval fails.
        /// </returns>
        /// </summary>
        public static string GetConsoleInfo()
        {

            // Attempt to get the handle for the standard output console
            var consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero || consoleHandle == new IntPtr(-1))
                return "Cannot get console handle";

            // Retrieve the console screen buffer information
            CONSOLE_SCREEN_BUFFER_INFO csbi;
            if (!GetConsoleScreenBufferInfo(consoleHandle, out csbi))
            {
                int code = Marshal.GetLastWin32Error();
                return $"Cannot get console screen buffer info. Error: {code}";
            }

            // Format and return the buffer information as a string
            return $"Buffer Size: {csbi.dwSize.X}x{csbi.dwSize.Y}, " +
                   $"Window: ({csbi.srWindow.Left},{csbi.srWindow.Top})-" +
                   $"({csbi.srWindow.Right},{csbi.srWindow.Bottom}), " +
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