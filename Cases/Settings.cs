using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace TDM365_TESTS_UI.Cases
{
    public class Settings
    {
        public string ObjDevCode
        {
            get {
                DateTime now = DateTime.Now;
                DateTime startOfYear = new DateTime(now.Year, 1, 1);
                TimeSpan timeDifference = now.Subtract(startOfYear);
                int Minutes = (int)timeDifference.TotalMinutes;
                return $"x{Minutes}"; 
            }
            set { ObjDevCode = value; }
        }
        public string ObjDevName
        {
            get { return $"{DateTime.Now} Автотестовый объект"; }
            set { ObjDevName = value; }
        }

        public string Descr
        {
            get { return $"Создано автотестом"; }
            set { Descr = value; }
        }

        public string ProjectCode
        {
            get
            {
                DateTime now = DateTime.Now;
                DateTime startOfYear = new DateTime(now.Year, now.Month, now.Day);
                TimeSpan timeDifference = now.Subtract(startOfYear);
                int Minutes = (int)timeDifference.TotalMinutes;
                return $"x{Minutes}";
            }
            set { ProjectCode = value; }
        }
    }

}
