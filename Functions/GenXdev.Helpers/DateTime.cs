// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : DateTime.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.284.2025
// ################################################################################
// MIT License
//
// Copyright 2021-2025 GenXdev
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ################################################################################



using System.Globalization;
using System.Text;

namespace GenXdev.Helpers
{
    public static class DateTime
    {
        public static string DateToRfc822Date(DateTimeOffset dateTime)
        {
            if (dateTime.Offset == TimeSpan.Zero)
            {
                return dateTime.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss Z", CultureInfo.InvariantCulture);
            }
            StringBuilder builder = new StringBuilder(dateTime.ToString("ddd, dd MMM yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture));
            builder.Remove(builder.Length - 3, 1);
            return builder.ToString();
        }

        public static string UtcDateToString(System.DateTime Date)
        {
            return Date.ToString("yyyy'-'MM'-'dd HH':'mm':'ss'Z'");
        }

        public static System.DateTime StringToUtcDate(String Date)
        {
            try
            {
                return System.DateTime.Parse(Date);
            }
            catch
            {
                return System.DateTime.MinValue;
            }
        }

        public static System.DateTime? StringToUtcDateOrNull(String Date)
        {
            if (String.IsNullOrWhiteSpace(Date))
                return null;

            try
            {
                return System.DateTime.Parse(Date);
            }
            catch
            {
                return null;
            }
        }

        public static System.DateTime Max(System.DateTime D1, System.DateTime D2)
        {
            if (D1 > D2)
                return D1;
            return D2;
        }

        public static System.DateTime? Max(System.DateTime? D1, System.DateTime? D2)
        {
            if (!D1.HasValue)
                return D2;
            if (!D2.HasValue)
                return D1;
            if (D1.Value > D2.Value)
                return D1.Value;
            return D2.Value;
        }

        public static System.DateTime GetDateTimeFromJavascriptLong(long jsMiliSeconds)
        {
            return new System.DateTime(((jsMiliSeconds * 10000) + 621355968000000000));
        }

        public static long GetJavascriptLongFromDateTime(System.DateTime date)
        {
            return (date.Ticks - 621355968000000000) / 10000;// (((jsMiliSeconds * 10000) + 621355968000000000));
        }

        public static string DateToAPOPDateThingy(System.DateTime dateTime)
        {
            return (dateTime.Ticks / 100000000000000d).ToString("0.00000000000");
        }
    }
}
