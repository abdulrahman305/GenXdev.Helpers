/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
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