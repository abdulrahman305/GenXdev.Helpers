// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : DateTime.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.308.2025
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
