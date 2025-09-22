// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Serialization.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.278.2025
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



using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GenXdev.Helpers
{
    public static class Serialization
    {
        public static String ToJson(object obj, bool indent = false)
        {
            if (indent)
            {
                return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }

            var serializer = new DataContractJsonSerializer(obj.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, obj);

            ms.Flush();
            ms.Position = 0;

            using (StreamReader sr = new StreamReader(ms, new UTF8Encoding()))
            {
                return sr.ReadToEnd();
            }
        }

        public static bool ToJsonFile(object obj, string filePath, bool indent = false)
        {
            try
            {
                FileSystem.ForciblyPrepareTargetFilePath(filePath);

                using (var stream = new FileStream(
                        filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                        1024 * 32, FileOptions.None))
                {
                    var bytes = new UTF8Encoding(false).GetBytes(ToJson(obj, indent));

                    stream.Write(bytes, 0, bytes.Length);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static String ToJsonAnonymous(object obj)
        {
            return ToJson(obj);
        }

        public static bool ToJsonAnonymous(object obj, string filePath)
        {
            try
            {
                using (var stream = new FileStream(
                        filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                        1024 * 32, FileOptions.None))
                {
                    var bytes = new UTF8Encoding(false).GetBytes(ToJsonAnonymous(obj));

                    stream.Write(bytes, 0, bytes.Length);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static T FromJson<T>(String JSON)
        {
            return JsonConvert.DeserializeObject<T>(JSON);

            //var serializer = new DataContractJsonSerializer(typeof(T));

            //MemoryStream ms = new MemoryStream();
            //using (StreamWriter sw = new StreamWriter(ms, new UTF8Encoding()))
            //{
            //    sw.Write(RemoveJSONComments(JSON));
            //    sw.Flush();
            //    ms.Flush();
            //    ms.Position = 0;

            //    return (T)serializer.ReadObject(ms);
            //}
        }

        public static string RemoveJSONComments(String JSON)
        {
            // init
            StringBuilder sb = new StringBuilder();

            // split up into lines
            var lines = JSON.Replace("\n", "").Split('\r');

            foreach (var line in lines)
            {
                // remove whitespaces
                var trimmedLine = line.Trim();

                // not a comment?
                if (!trimmedLine.StartsWith("\\\\"))
                {
                    // then keep it
                    sb.Append(line + " ");
                }
            }

            return sb.ToString();
        }
    }
}
