// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Environment.cs
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



using System.Reflection;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Provides utility methods for retrieving environment and assembly-related paths.
    /// </para>
    ///
    /// <para type="description">
    /// The Environment class contains static methods that help determine the root directories
    /// of applications and assemblies, as well as retrieve assembly metadata such as company names.
    /// These methods are designed to work in various .NET execution contexts, including PowerShell
    /// modules and standalone applications.
    /// </para>
    /// </summary>
    public static class Environment
    {
        /// <summary>
        /// <para type="synopsis">
        /// Gets the root directory of the application.
        /// </para>
        ///
        /// <para type="description">
        /// Retrieves the directory path where the application's entry assembly is located.
        /// If no entry assembly is available (common in library contexts), it falls back
        /// to the calling assembly's directory.
        /// </para>
        /// </summary>
        /// <returns>The absolute path to the application root directory.</returns>
        public static String GetApplicationRootDirectory()
        {
            // Retrieve the entry assembly for the application
            var callingAssembly = System.Reflection.Assembly.GetEntryAssembly();

            // If no entry assembly exists, use the calling assembly as fallback
            if (callingAssembly == null)
                return GetAssemblyRootDirectory();

            // Return the directory containing the entry assembly
            return Path.GetDirectoryName(callingAssembly.Location)!;
        }

        /// <summary>
        /// <para type="synopsis">
        /// Gets the root directory of the calling assembly.
        /// </para>
        ///
        /// <para type="description">
        /// Retrieves the directory path where the assembly that called this method is located.
        /// This is useful for finding resources relative to the assembly's location.
        /// </para>
        /// </summary>
        /// <returns>The absolute path to the calling assembly's root directory.</returns>
        public static String GetAssemblyRootDirectory()
        {
            // Get the directory of the calling assembly's location
            return Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)!;
        }

        /// <summary>
        /// <para type="synopsis">
        /// Gets the location of the entry assembly.
        /// </para>
        ///
        /// <para type="description">
        /// Retrieves the full path to the entry assembly's file. If no entry assembly
        /// is available, it uses the calling assembly as a fallback.
        /// </para>
        /// </summary>
        /// <returns>The full path to the entry assembly file.</returns>
        public static string GetEntryAssemblyLocation()
        {
            // Attempt to get the entry assembly
            var assembly = System.Reflection.Assembly.GetEntryAssembly();

            // If no entry assembly, use the calling assembly
            if (assembly == null)
                assembly = System.Reflection.Assembly.GetCallingAssembly();

            // Return the assembly's location
            return assembly.Location;
        }

        /// <summary>
        /// <para type="synopsis">
        /// Gets the company name from an assembly's metadata.
        /// </para>
        ///
        /// <para type="description">
        /// Retrieves the company name attribute from the specified assembly's metadata.
        /// If no company attribute is found, returns "Unknown".
        /// </para>
        /// </summary>
        /// <param name="assembly">The assembly to examine for company information.</param>
        /// <returns>The company name if available, otherwise "Unknown".</returns>
        public static string GetAssemblyCompanyName(Assembly assembly)
        {
            // Check if assembly is provided
            if (assembly != null)
            {
                // Retrieve company name attributes from the assembly
                var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

                // If company attribute exists, return its value
                if (attributes.Length > 0)
                {
                    var attribute = attributes[0] as AssemblyCompanyAttribute;
                    return attribute.Company;
                }
            }

            // Return default value if no company found
            return "Unknown";
        }
    }
}