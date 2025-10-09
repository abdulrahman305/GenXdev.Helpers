// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Serialization.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.300.2025
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
