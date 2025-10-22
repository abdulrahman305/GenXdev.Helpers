// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Serialization.cs
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

        public static string RemoveJSONComments(string[] jsonLines)
        {
            return RemoveJSONComments(string.Join("\r\n", jsonLines));
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
            if (string.IsNullOrEmpty(JSON))
                return JSON;

            var result = new StringBuilder();
            bool inString = false;
            bool inSingleLineComment = false;
            bool inMultiLineComment = false;
            char stringChar = '"';

            for (int i = 0; i < JSON.Length; i++)
            {
                char current = JSON[i];
                char next = (i + 1 < JSON.Length) ? JSON[i + 1] : '\0';

                // Handle string literals - don't process comments inside strings
                if (!inSingleLineComment && !inMultiLineComment)
                {
                    if (!inString && (current == '"' || current == '\''))
                    {
                        inString = true;
                        stringChar = current;
                        result.Append(current);
                        continue;
                    }
                    else if (inString && current == stringChar)
                    {
                        // Check for escaped quotes
                        bool escaped = false;
                        int backslashCount = 0;
                        for (int j = i - 1; j >= 0 && JSON[j] == '\\'; j--)
                        {
                            backslashCount++;
                        }
                        escaped = backslashCount % 2 == 1;

                        if (!escaped)
                        {
                            inString = false;
                        }
                        result.Append(current);
                        continue;
                    }
                    else if (inString)
                    {
                        result.Append(current);
                        continue;
                    }
                }

                // Handle comments
                if (!inString)
                {
                    // Start of single-line comment
                    if (!inMultiLineComment && current == '/' && next == '/')
                    {
                        inSingleLineComment = true;
                        i++; // Skip the second '/'
                        continue;
                    }

                    // Start of multi-line comment
                    if (!inSingleLineComment && current == '/' && next == '*')
                    {
                        inMultiLineComment = true;
                        i++; // Skip the '*'
                        continue;
                    }

                    // End of multi-line comment
                    if (inMultiLineComment && current == '*' && next == '/')
                    {
                        inMultiLineComment = false;
                        i++; // Skip the '/'
                        continue;
                    }

                    // End of single-line comment (newline)
                    if (inSingleLineComment && (current == '\r' || current == '\n'))
                    {
                        inSingleLineComment = false;
                        result.Append(current);
                        continue;
                    }

                    // Skip characters inside comments
                    if (inSingleLineComment || inMultiLineComment)
                    {
                        continue;
                    }
                }

                // Add non-comment characters
                result.Append(current);
            }

            return result.ToString();
        }
    }
}
