using System.Reflection;
using GenXdev.Helpers;

namespace GenXdev.Configuration
{
    public class ConfigurationBase
    {
        public event EventHandler<OnPropertyChangedEventArgs> OnPropertyChanged;

        protected object padLock = new object();

        protected void TriggerOnPropertyChanged(string PropertyName, object Value)
        {
            var handler = OnPropertyChanged;

            if (handler != null)
            {
                try
                {
                    handler(this, new OnPropertyChangedEventArgs(PropertyName, Value));
                }
                catch { }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConfigurationBase(string filePath = null)
        {
            LoadConfigurationFromDisk(filePath);
        }

        protected virtual bool LoadConfigurationFromDisk(string configurationPath = null)
        {
            try
            {
                configurationPath = !String.IsNullOrWhiteSpace(configurationPath) && File.Exists(configurationPath) ? configurationPath :
                       Path.Combine(
                           GenXdev.Helpers.Environment.GetApplicationRootDirectory(),
                           this.GetType().Name + ".cfg");

                if (File.Exists(configurationPath))
                {
                    // read configuration lines
                    var Lines = File.ReadAllLines(configurationPath);

                    // get properties
                    var Props = this.GetType().GetProperties();

                    foreach (var Line in Lines)
                    {
                        var line = Line.Trim();
                        if ((!line.StartsWith("#")) && line.Contains("=") && (!line.StartsWith("[")) && (!line.StartsWith(";")))
                        {
                            try
                            {
                                // split
                                var nameValue = line.Split('=');
                                var name = nameValue[0].Trim().ToLower();
                                var value = nameValue[1].Trim();

                                // find property
                                PropertyInfo prop = (from p in Props
                                                     where p.Name.ToLower() == name
                                                     select p).FirstOrDefault<PropertyInfo>();
                                // found?
                                if ((prop != null) && (prop.CanWrite) && (prop.PropertyType.IsPublic))
                                {
                                    if (prop.PropertyType.IsEnum)
                                    {
                                        SetEnumProperty(value, prop);
                                    }
                                    else
                                        switch (prop.PropertyType.Name)
                                        {
                                            case "int":
                                                SetIntProperty(value, prop);
                                                break;
                                            case "Int32":
                                                SetIntProperty(value, prop);
                                                break;
                                            case "long":
                                                SetInt64Property(value, prop);
                                                break;
                                            case "Int64":
                                                SetInt64Property(value, prop);
                                                break;
                                            case "String":
                                            case "string":
                                                SetStringProperty(value, prop);
                                                break;
                                            case "String[]":
                                            case "string[]":
                                                SetStringsProperty(value, prop);
                                                break;
                                            case "Int32[]":
                                            case "int[]":
                                                SetIntsProperty(value, prop);
                                                break;
                                            case "Double":
                                                SetDoubleProperty(value, prop);
                                                break;
                                            case "bool":
                                                SetBoolProperty(value, prop);
                                                break;
                                            case "Boolean":
                                                SetBoolProperty(value, prop);
                                                break;
                                        }
                                }
                            }

                            catch { }
                        }
                    }

                    return true;
                }
            }
            catch
            {
                System.Threading.Thread.Sleep(2000);
            }

            return false;
        }

        void SetEnumProperty(string value, PropertyInfo prop)
        {
            value = value.Trim();
            try
            {
                if (value.Contains(",") || value.Contains(";"))
                {
                    int result = 0;

                    var list = value.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var newList = (from q in list select q.Trim()).ToArray<string>();

                    foreach (var item in newList)
                    {
                        result |= (int)Enum.Parse(prop.PropertyType, value, true);
                    }

                    prop.SetValue(this, result);
                }
                else
                {
                    prop.SetValue(this, Enum.Parse(prop.PropertyType, value, true));
                }
            }
            catch { }
        }

        void SetIntsProperty(string value, PropertyInfo prop)
        {
            var parts = value.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new List<int>();

            foreach (var part in parts)
            {
                if (Int32.TryParse(part, out int port))
                {
                    result.Add(port);
                }
            }

            prop.SetValue(this, result.Distinct<int>().ToArray<int>());
        }

        void SetStringsProperty(string value, PropertyInfo prop)
        {
            var list = value.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

            var newValue = (from q in list select q.Trim()).ToArray<string>();

            prop.SetValue(this, newValue);
        }

        void SetInt64Property(string value, PropertyInfo prop)
        {
            if (Int64.TryParse(value, out long v))
            {
                prop.SetValue(this, v, null);
            }
        }

        void SetUInt64Property(string value, PropertyInfo prop)
        {
            if (UInt64.TryParse(value, out ulong v))
            {
                prop.SetValue(this, v, null);
            }
        }

        void SetBoolProperty(string value, PropertyInfo prop)
        {
            bool v = (value.ToLower() == "true") || (value == "1") || (value.ToLower() == "yes") || (value.ToLower() == "ja");

            prop.SetValue(this, v, null);
        }

        void SetStringProperty(string value, PropertyInfo prop)
        {
            prop.SetValue(this, value, null);
        }

        void SetUIntProperty(string value, PropertyInfo prop)
        {
            if (UInt32.TryParse(value, out uint v))
            {
                prop.SetValue(this, v, null);
            };
        }

        void SetIntProperty(string value, PropertyInfo prop)
        {
            if (Int32.TryParse(value, out int v))
            {

                prop.SetValue(this, v, null);
            };
        }

        void SetDoubleProperty(string value, PropertyInfo prop)
        {
            if (Double.TryParse(value, out double v))
            {
                prop.SetValue(this, v, null);
            }
        }
    }
}
