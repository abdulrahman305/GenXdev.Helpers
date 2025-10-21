// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Environment.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.304.2025
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
    public static class Environment
    {
        public static String GetApplicationRootDirectory()
        {
            var callingAssembly = System.Reflection.Assembly.GetEntryAssembly();

            if (callingAssembly == null)
                return GetAssemblyRootDirectory();

            return Path.GetDirectoryName(callingAssembly.Location)!;
        }

        public static String GetAssemblyRootDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)!;
        }

        public static string GetEntryAssemblyLocation()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();

            if (assembly == null)
                assembly = System.Reflection.Assembly.GetCallingAssembly();

            return assembly.Location;
        }

        public static string GetAssemblyCompanyName(Assembly assembly)
        {
            if (assembly != null)
            {
                // get companyname
                var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length > 0)
                {
                    var attribute = attributes[0] as AssemblyCompanyAttribute;
                    return attribute.Company;
                }
            }

            return "Unknown";
        }
    }
}