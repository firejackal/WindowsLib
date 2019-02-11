using System;
using Microsoft.Win32;

namespace WindowsLib
{
    public class RegistryHelper
    {
        #region HKEYS
        public RegistryKey HKeyLocalMachine { get { return Registry.LocalMachine; } }

        public RegistryKey HKeyClassesRoot { get { return Registry.ClassesRoot; } }

        public RegistryKey HKeyCurrentUser { get { return Registry.CurrentUser; } }

        public RegistryKey HKeyUsers { get { return Registry.Users; } }

        public RegistryKey HKeyCurrentConfig { get { return Registry.CurrentConfig; } }

        public RegistryKey HKeyPerformanceData { get { return Registry.PerformanceData; } }
        #endregion HKEYS

        public bool CreateKey(RegistryKey parentKey, string subKey)
        {
            try
            {
                parentKey.CreateSubKey(subKey);
                return true;
            } catch
            {
                return false;
            }
        } //CreateKey

        public bool DeleteKey(RegistryKey parentKey, string subKey)
        {
            try
            {
                //deletes the subkey and returns a False if Subkey doesn't exist
                parentKey.DeleteSubKey(subKey, false);
                return true;
            } catch
            {
                return false;
            }
        } //DeleteKey

        public object GetValue(RegistryKey parentKey, string subKey, string valueName, object defaultValue = null)
        {
            try
            {
                //opens the given subkey
                RegistryKey Key = parentKey.OpenSubKey(subKey, true);
                if (Key == null) throw new Exception("The key does not exist.");

                //Gets the value
                object results = Key.GetValue(valueName);
                Key.Close();
                return results;
            } catch
            {
                return defaultValue;
            }
        } //GetValue

        public bool SetValue(RegistryKey ParentKey, string subKey, string valueName, object newValue)
        {
            try
            {
                //Opens the given subkey
                RegistryKey Key = ParentKey.OpenSubKey(subKey, true);
                if (Key == null) Key = ParentKey.CreateSubKey(subKey);

                //sets the value
                Key.SetValue(valueName, newValue);
                Key.Close();

                return true;
            } catch
            {
                return false;
            }
        } //SetValue

        public bool DeleteValue(RegistryKey parentKey, string subKey, string valueName)
        {
            try
            {
                //opens the given subkey
                RegistryKey Key = parentKey.OpenSubKey(subKey, true);

                //deletes the value
                if (Key != null)
                {
                    Key.DeleteValue(valueName);
                    Key.Close();
                    return true;
                } else
                {
                    return false;
                }
            } catch
            {
                return false;
            }
        } //DeleteValue

        public string[] EnumKeys(RegistryKey parentKey, string subKey)
        {
            try
            {
                //opens the given subkey
                RegistryKey Key = parentKey.OpenSubKey(subKey);
                if (Key != null)
                {
                    //get all the subKeys (child subkeys)
                    string[] results = Key.GetSubKeyNames();
                    Key.Close();
                    return results;
                } else
                {
                    return null;
                }
            } catch
            {
                return null;
            }
        } //EnumKeys

        public string[] EnumValues(RegistryKey parentKey, string subKey)
        {
            try
            {
                //opens the given subkey
                RegistryKey Key = parentKey.OpenSubKey(subKey);
                if (Key != null)
                {
                    //get all the subKeys (child subkeys)
                    string[] results = Key.GetValueNames();
                    Key.Close();
                    return results;
                } else
                {
                    return null;
                }
            } catch
            {
                return null;
            }
        } //EnumValues

        public bool KeyExists(RegistryKey parentKey, string subKey)
        {
            try
            {
                RegistryKey key = parentKey.OpenSubKey(subKey, false);
                if (key == null) throw new Exception("The key does not exist.");
                key.Close();
                return true;
            } catch
            {
                return false;
            }
        } //KeyExists

        public bool ValueExists(RegistryKey parentKey, string subKey, string valueName)
        {
            try
            {
                RegistryKey key = parentKey.OpenSubKey(subKey, false);
                if (key == null) throw new Exception("The key does not exist.");
                if (key.GetValue(valueName, null) == null) throw new Exception("The value does not exist.");
                key.Close();
                return true;
            } catch
            {
                return false;
            }
        } //ValueExists
    } //RegistryHelper Class
}
