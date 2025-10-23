// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Serialization.cs
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



using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GenXdev.Helpers
{
    /// <summary>
    /// Provides static utility methods for JSON serialization and deserialization,
    /// including support for removing comments from JSON strings and writing JSON
    /// to files. This class uses Newtonsoft.Json for primary serialization and
    /// DataContractJsonSerializer as a fallback for certain operations.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Serializes an object to a JSON string using DataContractJsonSerializer.
        /// If indent is true, uses Newtonsoft.Json with indented formatting.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="indent">If true, formats the JSON with indentation.</param>
        /// <returns>A JSON string representation of the object.</returns>
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

        /// <summary>
        /// Serializes an object to a JSON file. Prepares the target file path forcibly
        /// and writes the JSON content. Returns true if successful, false otherwise.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="filePath">The file path where the JSON will be written.</param>
        /// <param name="indent">If true, formats the JSON with indentation.</param>
        /// <returns>True if the file was written successfully, false otherwise.</returns>
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

        /// <summary>
        /// Serializes an object to a JSON string without type information.
        /// Uses the same implementation as ToJson.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static String ToJsonAnonymous(object obj)
        {

            return ToJson(obj);

        }

        /// <summary>
        /// Serializes an object to a JSON file without type information.
        /// Writes the JSON content to the specified file path.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="filePath">The file path where the JSON will be written.</param>
        /// <returns>True if the file was written successfully, false otherwise.</returns>
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

        /// <summary>
        /// Removes comments from an array of JSON lines and returns the cleaned JSON string.
        /// </summary>
        /// <param name="jsonLines">An array of JSON strings, each representing a line.</param>
        /// <returns>A JSON string with comments removed.</returns>
        public static string RemoveJSONComments(string[] jsonLines)
        {

            return RemoveJSONComments(string.Join("\r\n", jsonLines));

        }

        /// <summary>
        /// Deserializes a JSON string to an object of type T using Newtonsoft.Json.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="JSON">The JSON string to deserialize.</param>
        /// <returns>An object of type T.</returns>
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

        /// <summary>
        /// Removes comments from a JSON string. Supports both single-line (//) and
        /// multi-line (/* */) comments, while preserving content inside string literals.
        /// </summary>
        /// <param name="JSON">The JSON string with potential comments.</param>
        /// <returns>A JSON string with comments removed.</returns>
        public static string RemoveJSONComments(String JSON)
        {

            if (string.IsNullOrEmpty(JSON))
                return JSON;

            var result = new StringBuilder();
            bool inString = false;
            bool inSingleLineComment = false;
            bool inMultiLineComment = false;
            char stringChar = '"';

            // Iterate through each character in the JSON string to process comments
            // while preserving string literals
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

                        // Check for escaped quotes by counting preceding backslashes
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
