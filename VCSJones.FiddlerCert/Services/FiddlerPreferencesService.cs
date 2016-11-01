using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCSJones.FiddlerCert.Services
{
    public interface IFiddlerPreferencesService
    {
        T GetPref<T>(string preferenceName, T defaultValue) where T : IConvertible;
        bool TryGetPref<T>(string preferenceName, out T value) where T : IConvertible;
        void SetPref<T>(string preferenceName, T value) where T : IConvertible;
    }

    public class FiddlerPreferencesService : IFiddlerPreferencesService
    {
        public T GetPref<T>(string preferenceName, T defaultValue) where T : IConvertible
        {
            T value;
            if (TryGetPref(preferenceName, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public bool TryGetPref<T>(string preferenceName, out T value) where T : IConvertible
        {
            var str = Fiddler.FiddlerApplication.Prefs.GetStringPref(preferenceName, null);
            if (str == null)
            {
                value = default(T);
                return false;
            }
            value = (T)Convert.ChangeType(str, typeof(T));
            return true;
        }

        public void SetPref<T>(string preferenceName, T value) where T : IConvertible
        {
            Fiddler.FiddlerApplication.Prefs.SetStringPref(preferenceName, (string)Convert.ChangeType(value, typeof(string)));
        }
    }
}
