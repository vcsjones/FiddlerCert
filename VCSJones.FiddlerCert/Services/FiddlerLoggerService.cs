using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCSJones.FiddlerCert.Services
{
    public interface IFiddlerLoggerService
    {
        void Log(string str);
    }

    public class FiddlerLoggerService : IFiddlerLoggerService
    {
        public void Log(string str) => Fiddler.FiddlerApplication.Log.LogString(str);
    }
}
