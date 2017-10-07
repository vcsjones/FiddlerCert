using System;

namespace VCSJones.FiddlerCert
{
    internal static class FiddlerPreferenceExtensions
    {
        public static T GetPref<T>(this Fiddler.IFiddlerPreferences preferences, string preferenceName, T defaultValue) where T : IConvertible
        {
            if (preferences.TryGetPref(preferenceName, out T value))
            {
                return value;
            }
            return defaultValue;
        }

        public static bool TryGetPref<T>(this Fiddler.IFiddlerPreferences preferences, string preferenceName, out T value) where T : IConvertible
        {
            var str = preferences.GetStringPref(preferenceName, null);
            if (str == null)
            {
                value = default(T);
                return false;
            }
            value = (T)Convert.ChangeType(str, typeof(T));
            return true;
        }

        public static void SetPref<T>(this Fiddler.IFiddlerPreferences preferences, string preferenceName, T value) where T : IConvertible
        {
            preferences.SetStringPref(preferenceName, (string)Convert.ChangeType(value, typeof(string)));
        }
    }
}
